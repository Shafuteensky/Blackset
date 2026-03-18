using Blackset.Inventories;
using UnityEngine;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Базовый модуль представленя предмета
    /// </summary>
    public abstract class BaseInventoryItemModule : MonoBehaviour
    {
        protected InventoryItemElement itemElement;
        protected Inventory container;

        /// <summary>
        /// Инициализация модуля
        /// </summary>
        /// <param name="inventoryItemElement">Родительский элемент представленя предмета</param>
        public void Initialize(InventoryItemElement inventoryItemElement, Inventory newContainer)
        {
            itemElement = inventoryItemElement;
            container = newContainer;
            OnInitialized();
        }

        protected abstract void OnInitialized();
    }
}