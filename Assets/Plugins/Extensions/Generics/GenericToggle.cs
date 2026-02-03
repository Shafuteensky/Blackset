using System;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Generics
{
    /// <summary>
    /// Абстракция переключателя
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public abstract class GenericToggle : MonoBehaviour
    {
        /// <summary>
        /// Событие, вызываемое после изменения состояния переключателя
        /// </summary>
        public event Action onToggleChanged;
        
        protected Toggle toggle = default;

        protected virtual void Awake() => toggle = GetComponent<Toggle>();

        protected virtual void OnEnable() => toggle.onValueChanged.AddListener(OnToggleAction);

        protected virtual void OnDisable() => toggle.onValueChanged.RemoveListener(OnToggleAction);

        protected virtual void OnToggleAction(bool state)
        {
            OnToggleChanged(state);
            onToggleChanged?.Invoke();
        }
        
        /// <summary>
        /// Код, выполняемый при изменении состояния переключателя
        /// </summary>
        public abstract void OnToggleChanged(bool state);
    }
}
