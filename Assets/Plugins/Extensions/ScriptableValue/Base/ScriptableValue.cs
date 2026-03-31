using System;
using System.Collections.Generic;
using Extensions.Data;
using UnityEngine;

namespace Extensions.ScriptableValues
{
    /// <summary>
    /// Базовая абстракция ScriptableValue — хранилища значения
    /// </summary>
    public abstract class ScriptableValue<T> : BaseScriptableValue
    {
        /// <summary>
        /// Событие изменения значения
        /// </summary>
        public event Action<T> onValueChanged;

        /// <summary>
        /// Текущее значение
        /// </summary>
        public virtual T Value
        {
            get
            {
                LoadIfNeeded();
                return runtimeValue;
            }
            set => SetValue(value);
        }
        
        [Header("Хранимое значение"), Space]
        [SerializeField]
        [Tooltip("Дефолтное значение, используемое если нет сохранения или оно отключено")]
        protected T defaultValue = default;
        [SerializeField]
        [Tooltip("Сохранять ли значение между сессиями")]
        protected bool isSaveable = false;

        [NonSerialized]
        protected T runtimeValue;

        protected bool isLoaded = false;

        protected virtual void OnEnable()
        {
            isLoaded = false;
            runtimeValue = defaultValue;
        }

        /// <summary>
        /// Установка значения
        /// </summary>
        public virtual void SetValue(T newValue)
        {
            LoadIfNeeded();

            if (EqualityComparer<T>.Default.Equals(runtimeValue, newValue))
                return;

            runtimeValue = newValue;

            onValueChanged?.Invoke(runtimeValue);

            if (Application.isPlaying && isSaveable)
            {
                JsonSaveLoad.Save(runtimeValue, Id);
            }
        }
        
        /// <summary>
        /// Сброс значения к дефолтному
        /// </summary>
        public override void ResetToDefault() => SetValue(defaultValue);
        
        protected virtual void LoadIfNeeded()
        {
            if (!Application.isPlaying || isLoaded) return;
            isLoaded = true;

            // Если значение не сохраняемое — всегда используем дефолт
            if (!isSaveable)
            {
                runtimeValue = defaultValue;
                return;
            }

            T loadedValue = JsonSaveLoad.Load(Id, defaultValue);
            runtimeValue = loadedValue;
        }
    }
}
