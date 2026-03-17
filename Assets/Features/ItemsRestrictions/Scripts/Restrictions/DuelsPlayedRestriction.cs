using Blackset.Data.Registries;
using Blackset.Inventories.Items;
using UnityEngine;

namespace Blackset.ItemsRestrictions
{
    [CreateAssetMenu(
        menuName = "Blackset/Items/Restrictions/Duels Played Restriction",
        fileName = "ItemRestriction_DuelsPlayed")]
    public sealed class DuelsPlayedRestriction : ItemRestriction
    {
        [Header("Условие"), Space]
        [SerializeField] private int requiredDuels;
        
        /// <summary>
        /// Блокирует доступность предмета, если сыграно меньше требуемого количества дуэлей
        /// </summary>
        public override ItemAvailability GetBLockedAvailability(InventoryItem item)
        {
            int duelsPlayed = GameData.Instance.PlayerDataFacade.ProgressData.Data.DuelsPlayed;

            if (duelsPlayed < requiredDuels) return blockedAvailability;

            return ItemAvailability.None;
        }
    }
}