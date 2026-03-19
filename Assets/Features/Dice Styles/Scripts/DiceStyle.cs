using System.Collections.Generic;
using System.Linq;
using Blackset.Data.Base;
using Extensions.Log;
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

        private Dictionary<string, DiceTypePrefabPair> _prefabMap;

        /// <summary>
        /// Возвращает пару меш/материал для указанного Id типа дайса.
        /// Словарь строится один раз при первом обращении.
        /// </summary>
        /// <returns>Пара или null если тип не найден</returns>
        public DiceTypePrefabPair GetPrefabPair(string diceTypeId)
        {
            _prefabMap ??= typePrefabs.ToDictionary(p => p.Type.Id);

            if (_prefabMap.TryGetValue(diceTypeId, out var pair)) 
                return pair;
            
            ServiceDebug.LogError($"[{nameof(DiceStyle)}] Не найден префаб для типа с Id '{diceTypeId}'");
            return null;
        }
    }
}