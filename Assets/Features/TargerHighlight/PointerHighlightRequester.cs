using Extensions.Generics;
using Extensions.ScriptableValues;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.TargetHighlight
{
    /// <summary>
    /// Отправитель запроса на подсветку при наведении на объект
    /// </summary>
    public sealed class PointerHighlightRequester : InitializableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private BoolValue boolValue;

        private void Awake()
        {
            Initialize(boolValue != null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            boolValue.Value = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            boolValue.Value = false;
        }
    }
}
