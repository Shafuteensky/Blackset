using UnityEngine;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;

namespace Blackset.Inventory.ItemPools
{
    /// <summary>
    /// Пул дайсов игрока для конкретного типа (номинала) дайса
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Inventory/" + nameof(DicesPoolRow),
        fileName = nameof(DicesPoolRow))]
    public sealed class DicesPoolRow : BasePlayerItemPool<DiceItemCell, DiceData, DiceType> { }
}