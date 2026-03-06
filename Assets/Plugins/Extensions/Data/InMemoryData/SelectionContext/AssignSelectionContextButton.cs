using Extensions.Log;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>
    /// Кнопка назначения контекста данных
    /// </summary>
    /// <typeparam name="TData">Тип данных</typeparam>
    [RequireComponent(typeof(ContextIdHolder))]
    public abstract class AssignSelectionContextButton<TData> : MonoBehaviour, IPointerDownHandler
        where TData : InMemoryDataEntry
    {
        [SerializeField]
        private SelectionContext<TData> selectionContext;

        protected ContextIdHolder idHolder;

        protected virtual void Awake()
        {
            idHolder = GetComponent<ContextIdHolder>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (selectionContext == null || !idHolder.IsInitialized)
            {
                ServiceDebug.LogError("Инициализация не выполнена или не полностью выполнена");
                return;
            }

            selectionContext.Select(idHolder.EntryId);
        }
    }
}