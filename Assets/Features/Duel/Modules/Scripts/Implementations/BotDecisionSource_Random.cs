using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Случайные решения бота
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(BotDecisionSource_Random),
        menuName = "Blackset/Duel/Modules/" + nameof(BotDecisionSource_Random))]
    public class BotDecisionSource_Random : BaseDuelModule, IBotDecisionSource
    {
        public TurnIntent BuildTurnIntent(DuelContext context)
        {
            TurnIntent randomIntents = new TurnIntent();
            
            //
            
            return randomIntents;
        }
    }
}