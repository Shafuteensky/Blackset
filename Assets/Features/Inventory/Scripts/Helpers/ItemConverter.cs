using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories.Cells;

namespace Blackset.Inventories.Helpers
{
    /// <summary>
    /// Конвертер данных инвентарь-контекст
    /// </summary>
    public static class ItemConverter
    {
        public static List<DiceItemContext> ToDiceItemContext(Inventory inventory)
        {
            GameData gameData = GameData.Instance;
            List<DiceItemContext> dices = new();

            foreach (InventoryCell cell in inventory.Data)
            {
                DiceItemContext context = new();
                if (gameData.Dices.GetById(cell.ItemId) is not DiceData dice) continue;
                context.Dice = dice.Id;
                if (gameData.DiceTypes.GetById(cell.ItemTypeId) is not DiceType type) continue;
                context.Type = type.Id;
                dices.Add(context);
            }
            
            return dices;
        }
        
        public static List<ConsumableItemContext> ToConsumableItemContext(Inventory inventory)
        {
            GameData gameData = GameData.Instance;
            List<ConsumableItemContext> consumables = new();

            foreach (InventoryCell cell in inventory.Data)
            {
                ConsumableItemContext context = new();
                if (gameData.Consumables.GetById(cell.ItemId) is not ConsumableData consumable) continue;
                context.Consumable = consumable;
                if (gameData.ConsumableTypes.GetById(cell.ItemTypeId) is not ConsumableType type) continue;
                context.Type = type;
                consumables.Add(context);
            }
            
            return consumables;
        }
    }
}