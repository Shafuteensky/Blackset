using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Effects;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Сборщик источников эффектов участника для текущей фазы
    /// </summary>
    public interface IEffectSourceCollector : IDuelModuleInterface
    {
        /// <summary>
        /// Собрать источники эффектов участника для указанной фазы
        /// </summary>
        /// <param name="context">Контекст дуэли</param>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="phase">Текущая фаза применения эффектов</param>
        /// <returns>Список источников эффектов</returns>
        List<EffectSourceRef> Collect(DuelContext context, string participantId, EffectPhase phase);
    }
}