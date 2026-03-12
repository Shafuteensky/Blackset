using System;
using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Пара: тип дайса и материал для его визуала
    /// </summary>
    [Serializable]
    public sealed class DiceTypePrefabPair
    {
        /// <summary>
        /// Тип дайса
        /// </summary>
        public DiceType Type => type;
        /// <summary>
        /// Материал для визуализации
        /// </summary>
        public Material Material => material;
        /// <summary>
        /// Визуальный объект (меш)
        /// </summary>
        public Mesh Body => body;

        [SerializeField]
        private DiceType type;
        [SerializeField]
        private Material material;
        [SerializeField]
        private Mesh body;
    }
}