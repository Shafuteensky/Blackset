using UnityEngine;

namespace Blackset.Data.Items.Types
{
    /// <summary>
    /// Тип расходника
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Types/" + nameof(ConsumableType),
        fileName = nameof(ConsumableType))]
    public sealed class ConsumableType : BaseItemType // TODO Сделать runtime-генерируемым полем внутри ConsumableData (либо поле в ConsumableItem)?
                                                      // // Иначе дубликаты данных
    {
        /// <summary>
        /// Одноразовый ли расходник
        /// </summary>
        public bool SingleUse => singleUse;

        [Header("Тип использования")]
        [SerializeField]
        private bool singleUse;
        
        // TODO Шанс появления в магазине/награде — относительная фактическая редкость появления типа
    }
}