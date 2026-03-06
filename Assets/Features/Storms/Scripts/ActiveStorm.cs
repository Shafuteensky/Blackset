using Blackset.Data.Registries;
using Extensions.Data;
using Extensions.Log;
using Extensions.ScriptableValues;
using UnityEngine;

namespace Blackset.Storms
{
    /// <summary>
    /// Хранилище активного шторма
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ActiveStorm),
        menuName = "Blackset/Storm/" + nameof(ActiveStorm))]
    public sealed class ActiveStorm : StringValue
    {
        /// <summary>
        /// Проверить, есть ли сохранённое состояние шторма (в том числе явное отсутствие)
        /// </summary>
        public bool IsActive() => JsonSaveLoad.Exists(Id) && !string.IsNullOrEmpty(Value);
        
        /// <summary>
        /// Получить активный шторм
        /// </summary>
        public bool TryGet(out Storm storm)
        {
            storm = null;
            if (!IsActive()) return false;

            storm = GameData.Instance.GetStorm(Value);
            return storm != null;
        }

        /// <summary>
        /// Задать новый шторм как активный
        /// </summary>
        public void Set(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ServiceDebug.LogError("Невалидный идентификатор шторма");
                return;
            }

            SetValue(id);
        }

        /// <summary>
        /// Очистить активный шторм (деактивировать шторм)
        /// </summary>
        public void Clear() => SetValue(string.Empty);
    }
}