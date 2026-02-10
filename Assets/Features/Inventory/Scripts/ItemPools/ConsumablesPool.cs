using UnityEngine;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;

namespace Blackset.Inventory.ItemPools
{
    /// <summary>
    /// Пул расходников игрока
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Inventory/" + nameof(ConsumablesPool),
        fileName = nameof(ConsumablesPool))]
    public sealed class ConsumablesPool : BasePlayerItemPool<ConsumableItemCell, ConsumableData, ConsumableType> { }
}