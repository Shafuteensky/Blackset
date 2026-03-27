using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Effects;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Стандартный сборщик источников эффектов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(EffectSourceCollector_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(EffectSourceCollector_Default))]
    public class EffectSourceCollector_Default : BaseDuelModule, IEffectSourceCollector
    {
        /// <summary>
        /// Собрать источники эффектов участника для указанной фазы
        /// </summary>
        /// <param name="context">Контекст дуэли</param>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="phase">Текущая фаза применения эффектов</param>
        /// <returns>Список источников эффектов</returns>
        public List<EffectSourceRef> Collect(DuelContext context, string participantId, EffectPhase phase)
        {
            return new List<EffectSourceRef>();
        }
    }
}