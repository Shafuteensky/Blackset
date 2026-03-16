using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Extensions.Log;
using Features.Inventory.Scripts.Items;
using VHierarchy;

namespace Blackset.Duel.Sets
{
    /// <summary>
    /// Сбори участника дуэли
    /// </summary>
    public class DuelSetsContext
    {
        /// <summary>
        /// Сборка дайсов <идентификатор_на_дуэль, дайс> (id задается при создании сборки)
        /// </summary>
        public Inventory DiceSetInventory { get; private set; }
        /// <summary>
        /// Сборка расходников <идентификатор_на_дуэль, расходник> (id задается при создании сборки)
        /// </summary>
        public Inventory ConsumableSetInventory { get; private set; }

        /// <summary>
        /// Заполнить данные сборок участника
        /// </summary>
        /// <param name="dices">Список дайсов</param>
        /// <param name="consumables">Список расходников</param>
        public DuelSetsContext(
            Inventory diceSetInventory,
            Inventory consumableSetInventory,
            List<DiceItemContext> dices,
            List<ConsumableItemContext> consumables)
        {
            if (diceSetInventory == null || consumableSetInventory == null)
            {
                ServiceDebug.LogError("Инвентари не инициализированы, сборки не созданы");
                return;
            }
            
            DiceSetInventory = diceSetInventory;
            ConsumableSetInventory = consumableSetInventory;
            
            // Сброс данных в сохраняемом инвентаре
            DiceSetInventory.Clear();
            ConsumableSetInventory.Clear();
            
            foreach (DiceItemContext dice in dices)
            {
                ItemContext newDice = new ItemContext(dice.Dice, dice.Type, ItemClass.Dice);
                DiceSetInventory.AddItem(newDice);
            }

            foreach (ConsumableItemContext consumable in consumables)
            {
                // TODO Обновить ConsumableItemContext (как DiceItemContext), потом AddItem
                //ConsumableSetInventory.AddItem(consumable.Consumable, consumable.Type)
            }
        }

        #region Getters

        /// <summary>
        /// Получить дайс по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор дайса в сборке (ячейки инвентаря)</param>
        /// <param name="dice">Выходные данные о найденном дайсе</param>
        /// <returns>true если дайс найден, иначе false</returns>
        public bool TryGetDice(string id, out DiceItemContext dice)
        {
            ServiceGuard.NotNullOrEmpty(id, nameof(id));
            
            dice = new DiceItemContext();
            InventoryCell cell = DiceSetInventory.GetById(id);
            
            if (cell != null)
            {
                dice.Dice = cell.Item.ItemId;
                dice.Type = cell.Item.ItemTypeId;
            }
            
            return cell != null;
        }

        /// <summary>
        /// Получить расходник по идентификатору
        /// </summary>
        public bool TryGetConsumable(string id, out ConsumableItemContext consumable)
        {
            ServiceGuard.NotNullOrEmpty(id, nameof(id));
            
            consumable = new ConsumableItemContext();
            InventoryCell cell = ConsumableSetInventory.GetById(id);
            
            if (cell != null)
            {
                // TODO Обновить после ConsumableItemContext 
                
                // consumable.Consumable = cell.ItemId;
                // consumable.Type = cell.ItemTypeId;
            }
            
            return cell != null;
        }

        #endregion
    }
}