using System.Collections.Generic;
using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Стиль дайсов (внешний вид)
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Dice/" + nameof(DiceStyle),
        fileName = nameof(DiceStyle))]
    public sealed class DiceStyle : BaseData
    {
        /// <summary>
        /// Набор материалов по типам дайсов
        /// </summary>
        public IReadOnlyList<DiceTypePrefabPair> TypePrefabs => typePrefabs;

        [SerializeField]
        private List<DiceTypePrefabPair> typePrefabs = new List<DiceTypePrefabPair>();
    }
}