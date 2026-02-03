using System;
using Extensions.Generics;
using Extensions.Log;
using UnityEngine;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>
    /// КНопка назначения контекста данных
    /// </summary>
    /// <typeparam name="TData">Тип данных</typeparam>
    [RequireComponent(typeof(ContextIdHolder))]
    public abstract class AssignSelectionContextButton<TData> : GenericButton
        where TData : InMemoryDataItem
    {
        [SerializeField]
        private SelectionContext<TData> selectionContext;
        [SerializeField]
        private InMemoryDataContainer<TData> container;

        protected ContextIdHolder idHolder;
        
        protected override void Awake()
        {
            base.Awake();
            idHolder = GetComponent<ContextIdHolder>();
        }
        
        public override void OnButtonClick()
        {
            if (selectionContext == null || container == null || string.IsNullOrEmpty(idHolder.Id))
            {
                ServiceDebug.LogError("Инициализация не выполнена или не полностью выполнена");
                return;
            }

            selectionContext.Select(container, idHolder.Id);
        }
    }
}
