using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data.Items.Types
{
    /// <summary>
    /// Тип расходника
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Item types/" + nameof(ConsumableType),
        fileName = nameof(ConsumableType))]
    public sealed class ConsumableType : BaseData
    {
        /// <summary>
        /// Одноразовый ли расходник
        /// </summary>
        public bool SingleUse => singleUse;

        [Header("Тип использования")]
        [SerializeField]
        private bool singleUse;
    }
}