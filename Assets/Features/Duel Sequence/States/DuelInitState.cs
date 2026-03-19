using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;
using Extensions.Helpers;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 1. Состояние инициализации дуэли
    /// </summary>
    /// <remarks>
    /// - Применяются правила шторма
    /// - Регистрируются участники и инициализируются данные о знаниях (KnowledgeContext)
    /// </remarks>
    public class DuelInitState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            context.Seed = IdGenerator.NewGuid();
            context.ApplyActiveStorm();

            // Регистрация участников и иницализация знаний
            string playerId = context.RegisterPlayer();
            context.Knowledge.Add(playerId, new KnowledgeState());
            string botId = context.RegisterBot(context.Contract);
            context.Knowledge.Add(botId, new KnowledgeState());
            
            eventHub.Publish(new DuelInitedEvent());
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BuildPreparationState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}