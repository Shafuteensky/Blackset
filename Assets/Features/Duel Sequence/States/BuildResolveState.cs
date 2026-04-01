using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.Sets;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 3. Фиксация сборок участников
    /// </summary>
    /// <remarks>
    /// - Генерируются сборки участников согласно правилам
    /// </remarks>
    public class BuildResolveState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                IParticipantSetGenerator setGenerator = modules.Get<IParticipantSetGenerator>();
                DuelSetsContext setsContext = setGenerator.GenerateSets(context, participant.ParticipantId);
                participant.InitializeSets(setsContext);
            }
            
            eventHub.Publish(new SetReadyEvent());
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<TargetValueSetupState>();
        }
        
        public void Exit(DuelContext context) { }
    }
}