using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.Inventories.Scripts.Items;
using Blackset.ItemsRestrictions;
using Extensions.Log;
using UnityEngine;

namespace Blackset.ItemGenerators
{
    /// <summary>
    /// Базовый генератор предметов
    /// </summary>
    public class ItemsGenerator
    {
        private readonly GameData gameData = GameData.Instance;

        /// <summary>
        /// Конструктор генератора предметов
        /// </summary>
        public ItemsGenerator() { }

        /// <summary>
        /// Получить новый случайный предмет указанного класса и доступности
        /// </summary>
        /// <param name="itemClass">Класс предмета</param>
        /// <param name="availability">Требуемая доступность предмета</param>
        /// <param name="set">Набор, к которому должен принадлежать предмет (по-умолчанию любой набор)</param>
        /// <returns>Контекст случайного предмета</returns>
        public ItemContext GetRandomItem(ItemClass itemClass, ItemAvailability availability, BaseSet set = null)
        {
            List<InventoryItem> availableItems = GetAvailableItems(itemClass, availability, set);
            if (availableItems == null || availableItems.Count == 0)
            {
                ServiceDebug.Log($"Не найдено предметов класса {itemClass} с доступностью {availability}");
                return new();
            }

            InventoryItem itemData = availableItems[Random.Range(0, availableItems.Count)];
            string itemTypeId = GetRandomItemTypeId(itemData);

            if (string.IsNullOrEmpty(itemTypeId))
            {
                ServiceDebug.Log($"Не найден доступный тип для предмета {itemData.Id}");
                return new();
            }

            return new ItemContext(itemData.Id, itemTypeId, itemClass);
        }

        #region Internal

        protected List<InventoryItem> GetAvailableItems(ItemClass itemClass, ItemAvailability availability, BaseSet set = null)
        {
            List<InventoryItem> sourceItems = GetItemsByClass(itemClass);
            if (sourceItems == null || sourceItems.Count == 0) return null;

            List<InventoryItem> result = new();

            foreach (InventoryItem item in sourceItems)
            {
                if (item == null) continue;

                if (set != null && IsItemInSet(item, set) == false) continue;

                if (HasRequiredAvailability(item, availability) == false) continue;

                result.Add(item);
            }

            return result;
        }

        protected List<InventoryItem> GetItemsByClass(ItemClass itemClass)
        {
            if (gameData == null) return null;

            switch (itemClass)
            {
                case ItemClass.Dice:
                {
                    if (gameData.Dices == null || gameData.Dices.Data == null) return null;

                    List<InventoryItem> result = new();
                    foreach (var item in gameData.Dices.Data)
                    {
                        result.Add(item);
                    }

                    return result;
                }
                case ItemClass.Consumable:
                {
                    if (gameData.Consumables == null || gameData.Consumables.Data == null) return null;

                    List<InventoryItem> result = new();
                    foreach (var item in gameData.Consumables.Data)
                    { 
                        result.Add(item);
                    }

                    return result;
                }
                default:
                {
                    ServiceDebug.LogError($"Необработанный класс предмета: {itemClass}");
                    return null;
                }
            }
        }

        protected bool HasRequiredAvailability(InventoryItem item, ItemAvailability availability)
        {
            if (item == null) return false;

            if (availability == ItemAvailability.None) return true;

            ItemAvailability itemAvailability = item.GetAvailability();
            return (itemAvailability & availability) == availability;
        }

        protected bool IsItemInSet(InventoryItem item, BaseSet set)
        {
            if (item == null || set == null) return false;

            switch (item.ItemClass)
            {
                case ItemClass.Dice:
                {
                    if (item is DiceData dice == false) return false;

                    return dice.Set == set;
                }
                case ItemClass.Consumable:
                {
                    if (item is ConsumableData consumable == false) return false;

                    return consumable.Set == set;
                }
                default:
                {
                    ServiceDebug.LogError($"Необработанный класс предмета: {item.ItemClass}");
                    return false;
                }
            }
        }

        protected string GetRandomItemTypeId(InventoryItem item)
        {
            if (item == null) return string.Empty;

            switch (item.ItemClass)
            {
                case ItemClass.Dice:
                {
                    if (item is DiceData dice == false || 
                        dice.AvailableTypes == null || 
                        dice.AvailableTypes.Count == 0)
                    {
                        return string.Empty;
                    }

                    return dice.AvailableTypes[Random.Range(0, dice.AvailableTypes.Count)].Id;
                }
                case ItemClass.Consumable:
                {
                    if (item is ConsumableData consumable == false || 
                        consumable.AvailableTypes == null || 
                        consumable.AvailableTypes.Count == 0)
                    {
                        return string.Empty;
                    }

                    return consumable.AvailableTypes[Random.Range(0, consumable.AvailableTypes.Count)].Id;
                }
                default:
                {
                    ServiceDebug.LogError($"Необработанный класс предмета: {item.ItemClass}");
                    return string.Empty;
                }
            }
        }

        #endregion
    }
}