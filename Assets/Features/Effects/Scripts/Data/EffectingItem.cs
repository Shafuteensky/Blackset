using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventories.Items;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Базовые данные игровой единицы, содержащей эффект
    /// </summary>
    public abstract class EffectingItem : InventoryItem
    {
        /// <summary>
        /// Конфигурация эффекта
        /// </summary>
        public EffectConfig Effect => effect;
        /// <summary>
        /// Цвет предмета // TODO Заменить на рендер изображения предмета
        /// </summary>
        public Color Color => color;
        /// <summary>
        /// Набор, к которому относится предмет
        /// </summary>
        public BaseSet Set => set;
        
        /// <summary>
        /// Доступные типы предметов для этих данных
        /// </summary>
        public List<InventoryItemType> AvailableTypes => availableTypes;
        
        [SerializeField] private BaseSet set;
        [SerializeField] private List<InventoryItemType> availableTypes = new List<InventoryItemType>();
        
        [Header("Особенности"), Space]
        [SerializeField] protected EffectConfig effect;
        [SerializeField] protected Color color = Color.white;
    }
}