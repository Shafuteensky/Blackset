using Extensions.Generics;
using Extensions.ScriptableValues;
using UnityEngine;

namespace Extensions.UI
{
    /// <summary>
    /// Кнопка сброса ScriptableValue к дефолтному значению
    /// </summary>
    public sealed class ScriptableValueResetButton : AbstractButton
    {
        [SerializeField]
        [Tooltip("Сбрасываемое значение")]
        private BaseScriptableValue value;

        /// <summary>
        /// Код, выполняемый по клику кнопки
        /// </summary>
        public override void OnButtonClick()
        {
            if (value == null) return;

            value.ResetToDefault();
        }
    }
}