using Blackset.Inventories.Items;
using UnityEngine;

namespace Blackset.ItemsRestrictions
{
    /// <summary>
    /// Абстракция ограничения доступности предмета игроку
    /// </summary>
    public abstract class ItemRestriction : ScriptableObject
    {
        [Header("Блокировка доступа"), Space]
        [Tooltip("Какой доступ будет отключен при неисполнении условий")]
        [SerializeField] protected ItemAvailability blockedAvailability = ItemAvailability.All;

        /// <summary>
        /// Получить флаги доступности, которые нужно заблокировать
        /// </summary>
        public abstract ItemAvailability GetBlockedAvailability(InventoryItem item);
    }
}