using Extensions.Generics;
using Extensions.ScriptableValues;
using UnityEngine;

namespace Blackset.TargetHighlight
{
    /// <summary>
    /// Получатель запроса на подсветку
    /// </summary>
    public class HighlighRequestResponder : InitializableMonoBehaviour
    {
        [Header("Подсветка"), Space]
        [SerializeField]
        private BoolValue boolValue;
        [SerializeField]
        private Light lightObject;
        
        private void OnEnable()
        {
            Initialize(boolValue != null && lightObject != null);
            
            if (!IsInitialized) return;
            boolValue.onValueChanged += EnableLight;
            
            lightObject.enabled = boolValue.Value;
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;
            boolValue.onValueChanged -= EnableLight;
        }

        private void EnableLight(bool state)
        {
            if (!IsInitialized) return;
            lightObject.enabled = state;
        }
    }
}