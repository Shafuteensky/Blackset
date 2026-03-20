using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Scripts.Items;

namespace Blackset.Data
{
    /// <summary>
    /// Структура данных определенного расходника
    /// </summary>
    public struct ConsumableItemContext
    {
        /// <summary>
        /// Игровые данные расходника
        /// </summary>
        public string Consumable;
        /// <summary>
        /// Тип расходника
        /// </summary>
        public string Type;
        // TODO редкость: public BaseItemRarity Rarity;
            
        public ConsumableData GetConsumable() => GameData.Instance.GetConsumable(Consumable);
            
        public ConsumableType GetConsumableType() => GameData.Instance.GetConsumableType(Type);

        public ItemContext ToItemContext() => new ItemContext(Consumable, Type, ItemClass.Consumable);
    }
}