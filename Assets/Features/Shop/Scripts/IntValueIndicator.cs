using Extensions.Generics;
using Extensions.ScriptableValues;
using TMPro;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Вывод значения из IntValue
    /// </summary>
    public class IntValueIndicator : InitializableMonoBehaviour
    {
        [SerializeField]
        protected IntValue intValue;
        [SerializeField]
        protected TMP_Text text;

        protected void OnEnable()
        {
            Initialize(intValue != null || text != null);
            if (!IsInitialized) return;
            intValue.onValueChanged += AssignValue;
            AssignValue(intValue.Value);
        }

        protected void OnDisable()
        {
            if (!IsInitialized) return;
            intValue.onValueChanged -= AssignValue;
        }

        protected void AssignValue(int newValue) => text.text = newValue.ToString();
    }
}