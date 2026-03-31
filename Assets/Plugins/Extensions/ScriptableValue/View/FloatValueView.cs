using Extensions.ScriptableValues;
using TMPro;
using UnityEngine;

namespace Extensions.UI
{
    /// <summary>
    /// Отображение значения FloatValue
    /// </summary>
    public sealed class FloatValueView : MonoBehaviour
    {
        [Tooltip("Отображаемое значение")]
        [SerializeField] private FloatValue value;
        [Tooltip("Текст для вывода")]
        [SerializeField] private TMP_Text text;
        [Tooltip("Показывать в процентах")]
        [SerializeField] private bool showAsPercent;

        private void OnEnable()
        {
            if (value != null)
            {
                value.onValueChanged += OnValueChanged;
                UpdateText(value.Value);
            }
        }

        private void OnDisable()
        {
            if (value != null) value.onValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float v) => UpdateText(v);

        private void UpdateText(float v)
        {
            if (text == null) return;

            if (showAsPercent)
            {
                text.text = Mathf.RoundToInt(v * 100f) + "%";
                return;
            }

            text.text = v.ToString("0.##");
        }
    }
}