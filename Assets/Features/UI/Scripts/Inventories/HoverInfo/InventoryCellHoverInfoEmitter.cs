using System;
using System.Collections;
using Blackset.Inventories;
using Extensions.Coroutines;
using Extensions.ScriptableValues;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Абстракция контроллера запросов вывода информации о ячейке инвентаря
    /// </summary>
    public sealed class InventoryCellHoverInfoEmitter : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        #region События

        /// <summary>
        /// Запрос вывода информации
        /// </summary>
        /// <typeparam name="TInventory">Ивентарь, запрашиваемый для вывода информации</typeparam>
        /// <typeparam name="string">Идентификатор ячейки инвентаря</typeparam>
        /// <typeparam name="Vector2">Позиция UI-элемента</typeparam>
        public static event Action<Inventory, string, Vector2> onShowRequested;
        /// <summary>
        /// Запрос прекращения вывода информации
        /// </summary>
        public static event Action onHideRequested;

        #endregion
        
        [SerializeField]
        [Tooltip("UI-элемент")]
        private InventoryItemElement element;
        [SerializeField]
        [Tooltip("Время удержания курсора над элементом до отправки запроса на вывод информации")]
        private FloatValue holdSeconds;

        private bool isPointerInside;
        private CoroutineTask holdTask;

        private void Awake() => holdTask = new CoroutineTask(this);
        private void OnDisable() => HideInfo();

        public void OnPointerEnter(PointerEventData eventData) => ScheduleShow();
        public void OnPointerExit(PointerEventData eventData) => HideInfo();

        private void HideInfo()
        {
            isPointerInside = false;
            holdTask.Stop();
            if (element != null) onHideRequested?.Invoke();
        }

        private void ScheduleShow()
        {
            isPointerInside = true;
            if (element == null) return;
            holdTask.Start(HoldRoutine());
        }

        private IEnumerator HoldRoutine()
        {
            float time = 0f;

            while (time < holdSeconds.Value)
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
