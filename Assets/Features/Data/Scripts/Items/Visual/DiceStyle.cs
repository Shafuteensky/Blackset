using System.Collections.Generic;
using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Стиль дайсов (внешний вид)
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Visual/" + nameof(DiceStyle),
        fileName = nameof(DiceStyle))]
    public sealed class DiceStyle : BaseData // TODO Может сделать структурой в составе DiceData?
    {
        /// <summary>
        /// Набор материалов по типам дайсов
        /// </summary>
        public IReadOnlyList<DiceTypePrefabPair> TypePrefabs => typePrefabs;

        [Header("Префабы внешнего вида дайса"), Space]
        [SerializeField]
        [Tooltip("Уникальны для каждого игрового типа дайса")]
        private List<DiceTypePrefabPair> typePrefabs = new List<DiceTypePrefabPair>();
    }
}