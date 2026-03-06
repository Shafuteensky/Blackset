using Extensions.Data;
using Extensions.Generics;
using Extensions.Helpers;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Служба сигнала обновления по данным прогресса игрока
    /// </summary>
    public abstract class PlayerProgressUpdater : InitializableMonoBehaviour
    {
        /// <summary>
        /// Данные об игроке
        /// </summary>
        public PlayerDataFacade PlayerData => playerData;
        
        [Header("Сохранение данных об обновлениях"), Space] 
        [SerializeField]
        protected string saveKey;
        
        [Header("Игровые данные"), Space]
        [SerializeField]
        protected PlayerDataFacade playerData;
        
        private int cachedDuelsPlayed;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(saveKey)) saveKey = IdGenerator.NewGuid();
        }

        /// <summary>
        /// Состояние сигнала об обновлении
        /// </summary>
        /// <returns>true если условия подачи сигнала соблюдены, иначе false</returns>
        protected virtual bool IsUpdateNeeded() => IsPlayerPlayedDuel();
        
        /// <summary>
        /// Сыграл ли игрок в новую дуэль с момента последнего запроса
        /// </summary>
        /// <returns>true если с момента последнего запроса игрок сыграл в дуэль, иначе false</returns>
        private bool IsPlayerPlayedDuel()
        {
            if (string.IsNullOrEmpty(saveKey) || playerData == null)
            {
                ServiceDebug.LogError("Идентификатор ключа сохранения не задан или данные об игроке отсутствуют");
                return false;
            }
            
            int duelsPlayed = playerData.ProgressData.Data.DuelsPlayed;
            cachedDuelsPlayed = JsonSaveLoad.Load(saveKey, -1);

            if (duelsPlayed == cachedDuelsPlayed) return false;
            
            cachedDuelsPlayed = duelsPlayed;
            JsonSaveLoad.Save(cachedDuelsPlayed, saveKey);
            return true;
        }
    }
}