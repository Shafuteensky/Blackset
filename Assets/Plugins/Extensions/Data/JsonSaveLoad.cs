using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Extensions.Log;
using UnityEngine;

namespace Extensions.Data
{
    /// <summary>
    /// Сохранение/загрузка в JSON с шифрованием
    /// <remarks>
    /// Для хранения игровых данных и структур настроек
    /// </remarks>
    /// </summary>
    public static class JsonSaveLoad
    {
        #region Constants
        
        private const string SAVE_FOLDER = "SaveData";
        private const string FILE_EXTENSION = ".sav";
        private const string BACKUP_EXTENSION = ".bak";
        private const int CURRENT_VERSION = 1;
        private const string DEFAULT_PROFILE_NAME = "default";
        
        #endregion

        #region Events
        
        /// <summary>
        /// Событие перед началом процесса сохранения
        /// </summary>
        public static event Action<string> onBeforeSave;
        /// <summary>
        /// Событие после успешного завершения процесса сохранения
        /// </summary>
        public static event Action<string> onAfterSave;
        /// <summary>
        /// Событие ошибки процесса сохранения
        /// </summary>
        public static event Action<string, Exception> onSaveError;
        
        /// <summary>
        /// Событие перед началом процесса загрузки
        /// </summary>
        public static event Action<string> onBeforeLoad;
        /// <summary>
        /// Событие после успешного завершения процесса загрузки
        /// </summary>
        public static event Action<string> onAfterLoad;
        /// <summary>
        /// Событие ошибки процесса загрузки
        /// </summary>
        public static event Action<string, Exception> onLoadError;
        
        #endregion
        
        private static int savingCount = 0;

        /// <summary>
        /// Состояние прцоесса сохранения
        /// </summary>
        public static bool IsSaving => savingCount > 0;
        
        /// <summary>
        /// Активный профиль сохранения
        /// </summary>
        public static string CurrentProfile { get; set; } = DEFAULT_PROFILE_NAME;
        
        private static string SaveDirectory => Path.Combine(Application.persistentDataPath, SAVE_FOLDER);

        private static string CurrentProfileDirectory
        {
            get
            {
                string profile = string.IsNullOrEmpty(CurrentProfile) ? DEFAULT_PROFILE_NAME : CurrentProfile;
                string path = Path.Combine(SaveDirectory, profile);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        [Serializable]
        private class SaveContainer<T>
        {
            public int Version;
            public string Profile;
            public string DataType;
            public string Hash;
            public string TimestampUtc;
            public T Data;
        }

        static JsonSaveLoad()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
                ServiceDebug.LogWarning($"Путь «{SaveDirectory}» не существует, создана новая директория");
            }
        }

        /// <summary>
        /// Сохранить данные в файл сохранения
        /// </summary>
        /// <param name="data">Сохраняемые данные</param>
        /// <param name="fileName">Название файла сохранения</param>
        /// <returns></returns>
        public static bool Save<T>(T data, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ServiceDebug.LogError("Пустое имя файла, сохранение не выполнено");
                return false;
            }

            savingCount++;

            try
            {
                onBeforeSave?.Invoke(fileName);

                SaveContainer<T> container = new SaveContainer<T>
                {
                    Version = CURRENT_VERSION,
                    Profile = string.IsNullOrEmpty(CurrentProfile) ? DEFAULT_PROFILE_NAME : CurrentProfile,
                    DataType = typeof(T).AssemblyQualifiedName,
                    TimestampUtc = DateTime.UtcNow.ToString("o"),
                    Data = data
                };

                string payloadJson = JsonUtility.ToJson(data, false);
                string hash = ComputeHash(payloadJson);
                container.Hash = hash;

                string json = JsonUtility.ToJson(container, true);
                string encrypted = DataEncryptor.Encrypt(json, DataEncryptor.EncryptionMode);

                string filePath = Path.Combine(CurrentProfileDirectory, fileName + FILE_EXTENSION);
                string backupPath = filePath + BACKUP_EXTENSION;

                try
                {
                    File.WriteAllText(filePath, encrypted);
                    File.WriteAllText(backupPath, encrypted);

                    onAfterSave?.Invoke(fileName);
                }
                catch (Exception ex)
                {
                    ServiceDebug.LogError($"Ошибка сохранения файла «{fileName}»: {ex}");

                    onSaveError?.Invoke(fileName, ex);
                    return false;
                }

                return true;
            }
            finally
            {
                savingCount--;
                if (savingCount < 0)
                {
                    savingCount = 0;
                }
            }
        }

        /// <summary>
        /// Загрузить данные из файла сохранения
        /// </summary>
        /// <param name="fileName">Название файла сохранения</param>
        /// <param name="defaultValue">Значение по-умолчанию (при отсутствии бэкапа)</param>
        /// <typeparam name="T">Загружаемые данные</typeparam>
        /// <returns></returns>
        public static T Load<T>(string fileName, T defaultValue = default)
        {
            onBeforeLoad?.Invoke(fileName);

            string filePath = Path.Combine(CurrentProfileDirectory, fileName + FILE_EXTENSION);

            if (!File.Exists(filePath))
            {
                ServiceDebug.LogWarning($"Файл «{fileName}» не найден, загружены значения по-умолчанию");
                onAfterLoad?.Invoke(fileName);
                return defaultValue;
            }

            try
            {
                string encrypted = File.ReadAllText(filePath);
                string json = DataEncryptor.Decrypt(encrypted, DataEncryptor.EncryptionMode);

                if (string.IsNullOrEmpty(json))
                {
                    T backupResult = TryLoadBackup(fileName, defaultValue);
                    onAfterLoad?.Invoke(fileName);
                    return backupResult;
                }

                SaveContainer<T> container = JsonUtility.FromJson<SaveContainer<T>>(json);
                if (container == null)
                {
                    T backupResult = TryLoadBackup(fileName, defaultValue);
                    onAfterLoad?.Invoke(fileName);
                    return backupResult;
                }

                ValidateVersion(container, fileName);

                if (!ValidateHash(container))
                {
                    ServiceDebug.LogError($"Хэш-подпись файла «{fileName}» не совпадает, попытка восстановления из бэкапа");
                    T backupResult = TryLoadBackup(fileName, defaultValue);
                    onAfterLoad?.Invoke(fileName);
                    return backupResult;
                }

                onAfterLoad?.Invoke(fileName);
                return container.Data;
            }
            catch (Exception ex)
            {
                ServiceDebug.LogError($"Ошибка загрузки файла «{fileName}», попытка восстановления из бэкапа: {ex}");
                onLoadError?.Invoke(fileName, ex);
                T backupResult = TryLoadBackup(fileName, defaultValue);
                onAfterLoad?.Invoke(fileName);
                return backupResult;
            }
        }

        /// <summary>
        /// Удалить файл сохранения
        /// </summary>
        /// <param name="fileName">Название файла сохранения</param>
        public static void DeleteSave(string fileName)
        {
            string filePath = Path.Combine(CurrentProfileDirectory, fileName + FILE_EXTENSION);
            string backupPath = filePath + BACKUP_EXTENSION;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }

        private static T TryLoadBackup<T>(string fileName, T defaultValue)
        {
            string backupPath = Path.Combine(CurrentProfileDirectory, fileName + FILE_EXTENSION + BACKUP_EXTENSION);

            if (!File.Exists(backupPath))
            {
                ServiceDebug.LogError($"Файл «{fileName}» поврежден, бэкап не найден");
                return defaultValue;
            }

            try
            {
                string encrypted = File.ReadAllText(backupPath);
                string json = DataEncryptor.Decrypt(encrypted, DataEncryptor.EncryptionMode);

                if (string.IsNullOrEmpty(json))
                {
                    ServiceDebug.LogError($"Файл «{fileName}» поврежден, восстановление из бэкапа не удалось (пустой JSON)");
                    return defaultValue;
                }

                SaveContainer<T> container = JsonUtility.FromJson<SaveContainer<T>>(json);
                if (container == null)
                {
                    ServiceDebug.LogError($"Файл «{fileName}» поврежден, восстановление из бэкапа не удалось (ошибка парсинга)");
                    return defaultValue;
                }

                ValidateVersion(container, fileName);

                if (!ValidateHash(container))
                {
                    ServiceDebug.LogError($"Файл «{fileName}» поврежден, восстановление из бэкапа не удалось (хэш не совпадает)");
                    return defaultValue;
                }

                return container.Data;
            }
            catch (Exception ex)
            {
                ServiceDebug.LogError($"Файл «{fileName}» поврежден, восстановление из бэкапа не удалось: {ex}");
                onLoadError?.Invoke(fileName, ex);
                return defaultValue;
            }
        }

        private static string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            byte[] dataBytes = Encoding.UTF8.GetBytes(input);

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(dataBytes);
                int length = hashBytes.Length;
                char[] chars = new char[length * 2];

                const string hex = "0123456789ABCDEF";

                for (int i = 0; i < length; i++)
                {
                    int b = hashBytes[i];
                    chars[i * 2] = hex[b >> 4];
                    chars[i * 2 + 1] = hex[b & 0xF];
                }

                string result = new string(chars);
                return result;
            }
        }

        private static bool ValidateHash<T>(SaveContainer<T> container)
        {
            if (container == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(container.Hash))
            {
                return true;
            }

            string payloadJson = JsonUtility.ToJson(container.Data, false);
            string hash = ComputeHash(payloadJson);

            bool isValid = string.Equals(hash, container.Hash, StringComparison.Ordinal);
            return isValid;
        }

        private static void ValidateVersion<T>(SaveContainer<T> container, string fileName)
        {
            if (container == null)
            {
                return;
            }

            if (container.Version != CURRENT_VERSION)
            {
                ServiceDebug.LogWarning($"Версия сохранения «{fileName}» ({container.Version}) не совпадает с текущей ({CURRENT_VERSION})");
            }
        }
    }
}
