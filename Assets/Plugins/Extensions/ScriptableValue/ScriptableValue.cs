using System;
using System.Collections.Generic;
using Extensions.Data;
using Extensions.Identification;
using UnityEngine;

namespace Extensions.ScriptableValues
{
    /// <summary>
    /// Базовая абстракция ScriptableValue — хранилища значения
    /// </summary>
    public abstract class ScriptableValue<T> : IdentifiableObject
    {
        /// <summary>
        /// Событие изменения значения
        /// </summary>
        /// <typeparam name="">Новое значение</typeparam>
        public event Action<T> onValueChanged;
        
        /// <summary>
        /// Текущее значение
        /// </summary>
        public virtual T Value
        {
            get
            {
                LoadIfNeeded();
                return value;
            }
            set => SetValue(value);
        }
        
        [Header("Хранимое значение"), Space]
        [SerializeField]
        protected T value = default;
        [SerializeField]
        [Tooltip("Сохранять ли значение между сессиями")]
        protected bool isSaveable = false;

        protected bool isLoaded = false;
        
        protected virtual void OnEnable() => isLoaded = false;
        
        /// <summary>
        /// Установка значения
        /// </summary>
        public virtual void SetValue(T newValue)
        {
            LoadIfNeeded();
            if (EqualityComparer<T>.Default.Equals(value, newValue)) return;

            value = newValue;

            onValueChanged?.Invoke(value);
            if (Application.isPlaying && isSaveable) JsonSaveLoad.Save(value, Id);
        }

        protected virtual void LoadIfNeeded()
        {
            if ( !Application.isPlaying || !isSaveable || isLoaded ) return;
            T newValue = JsonSaveLoad.Load(Id, value);
            if (newValue.Equals(null)) return;
            
            value = newValue;
            isLoaded = true;
        }
    }
}