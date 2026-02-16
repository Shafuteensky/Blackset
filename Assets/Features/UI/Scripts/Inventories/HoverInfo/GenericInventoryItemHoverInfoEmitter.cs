using System;
using System.Collections;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Extensions.Coroutines;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Абстракция контроллера запросов вывода информации о ячейке инвентаря
    /// </summary>
    public abstract class GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType> : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : Effects.EffectingItemData
        where TItemType : Data.Items.Types.BaseItemType
    {
        #region События

        /// <summary>
        /// Запрос вывода информации
        /// </summary>
        /// <typeparam name="TInventory">Ивентарь, запрашиваемый для вывода информации</typeparam>
        /// <typeparam name="string">Идентификатор ячейки инвентаря</typeparam>
        /// <typeparam name="Vector2">Позиция UI-элемента</typeparam>
        public static event Action<TInventory, string, Vector2> onShowRequested;
        /// <summary>
        /// Запрос прекращения вывода информации
        /// </summary>
        public static event Action onHideRequested;

        #endregion
        
        [SerializeField]
        [Tooltip("UI-элемент")]
        protected GenericInventoryItemElement<TInventory, TItemCell, TData, TItemType> element;
        [SerializeField]
        [Range(0f, 2f)]
        [Tooltip("Время удержания курсора над элементом до отправки запроса на вывод информации")]
        protected float holdSeconds = 0.75f;

        protected bool isPointerInside;
        protected CoroutineTask holdTask;

        protected virtual void Awake() => holdTask = new CoroutineTask(this);
        protected virtual void OnDisable() => HideInfo();

        public void OnPointerEnter(PointerEventData eventData) => ScheduleShow();
        public void OnPointerExit(PointerEventData eventData) => HideInfo();

        protected void HideInfo()
        {
            isPointerInside = false;
            holdTask.Stop();
            if (element != null) onHideRequested?.Invoke();
        }

        protected void ScheduleShow()
        {
            isPointerInside = true;
            if (element == null) return;
            holdTask.Start(HoldRoutine());
        }

        protected IEnumerator HoldRoutine()
        {
            float time = 0f;

            while (time < holdSeconds)
            {
                if ( !isPointerInside || 
                     !isActiveAndEnabled ) 
                    yield break;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            if ( !isPointerInside || 
                 !isActiveAndEnabled || 
                 element == null ) 
                yield break;

            onShowRequested?.Invoke(element.Inventory, element.ItemCellId, element.transform.position);
        }
    }
}
