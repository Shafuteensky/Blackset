using Blackset.Data.Items.Visual;
using Blackset.DuelContracts;

namespace Blackset.ObjectDistribution
{
    /// <summary>
    /// Распределяет представления дайсов инвентаря по точкам
    /// </summary>
    public class InventoryViewFactoryDicesDistributor : BaseFactoryObjectsDistributor<VisualDice,
        DiceItemViewFactory> { }
}