using Blackset.Data.Items.Visual;
using Blackset.DuelContracts;

namespace Blackset.ObjectDistribution
{
    /// <summary>
    /// Распределяет представления расходников инвентаря по точкам
    /// </summary>
    public class InventoryViewFactoryConsumablesDistributor : BaseFactoryObjectsDistributor<VisualConsumable,
        ConsumablesItemViewFactory> { }
}