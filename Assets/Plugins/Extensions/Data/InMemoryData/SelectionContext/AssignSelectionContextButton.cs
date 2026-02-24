using Extensions.Generics;
using Extensions.Log;
using UnityEngine;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>
    /// Кнопка назначения контекста данных
    /// </summary>
    /// <typeparam name="TData">Тип данных</typeparam>
    [RequireComponent(typeof(ContextIdHolder))]
    public abstract class AssignSelectionContextButton<TData> : AbstractButton
        where TData : InMemoryDataEntry
    {
        [SerializeField]
        private SelectionContext<TData> selectionContext;
        
        protected ContextIdHolder idHolder;
        
        protected override void Awake()
        {
            base.Awake();
            idHolder = GetComponent<ContextIdHolder>();
        }
        
        public override void OnButtonClick()
        {
            if (selectionContext == null || !idHolder.IsInitialized)
            {
                ServiceDebug.LogError("Инициализация не выполнена или не полностью выполнена");
                return;
            }

            selectionContext.Select(idHolder.Id);
        }
    }
}
