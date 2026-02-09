using System.Collections.Generic;
using UnityEngine;
using Blackset.Data.Base;

namespace Blackset.Effects
{
    /// <summary>
    /// Конфигурация эффекта
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Effects/Effect",
        fileName = nameof(EffectConfig))]
    public sealed class EffectConfig : BaseData
    {
        public EffectType EffectType => effectType;

        public int Power => power;

        public EffectPhase[] Phases => phases;

        public EffectTarget TargetType => targetType;

        public bool RequiresChoice => requiresChoice;

        [SerializeField]
        private EffectType effectType;
        [SerializeField]
        private EffectPhase[] phases;
        [SerializeField]
        private EffectTarget targetType;
        
        [SerializeField]
        private int power;
        [SerializeField]
        private bool requiresChoice;

        /// <summary>
        /// Получить последний бросок участника
        /// </summary>
        /// <param name="participantId">Идентификатор участника</param>
        // public DuelHistoryEntry GetLastThrow(string participantId)
        // {
        //     return null;
        // }

        /// <summary>
        /// Получить эффекты, выпавшие на бросках
        /// </summary>
        public List<EffectConfig> GetThrownEffects()
        {
            return new List<EffectConfig>();
        }
    }
}