using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Базовый переключатель для окна планирования
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public abstract class AbstractPlanningToggle : AbstractPlanningElement
    {
        /// <summary>
        /// Событие, вызываемое после изменения состояния переключателя
        /// </summary>
        public event Action<bool> onToggled;

        protected Toggle toggle;

        protected virtual void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.interactable = false;
        }

        protected virtual void OnEnable() => toggle.onValueChanged.AddListener(OnToggleAction);

        protected virtual void OnDisable() => toggle.onValueChanged.RemoveListener(OnToggleAction);

        protected virtual void OnToggleAction(bool state)
        {
            OnToggled(state);
            onToggled?.Invoke(state);
        }

        /// <summary>
        /// Код, выполняемый при изменении состояния переключателя
        /// </summary>
        public abstract void OnToggled(bool state);

        protected override void SetInteractable(bool state)
        {
            if (toggle != null)
                toggle.interactable = state;
        }
    }
}