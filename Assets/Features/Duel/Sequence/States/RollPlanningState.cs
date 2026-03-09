using System;
using System.Threading;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.TurnIntents;
using Cysharp.Threading.Tasks;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 6. Фаза решений хода (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Ожидание ввода от игрока (объявление дайса, выбор фактических действий)
    /// - Генерация выборов бота-соперника
    /// - Создание записей намерений участников
    /// </remarks>
    public class RollPlanningState : BaseDuelState, IState<DuelContext>
    {
        private CancellationTokenSource planningCancellationTokenSource;
        private bool planningCompleted;
        private Exception planningException;

        public void Enter(DuelContext context)
        {
            context.Progress.OnNewThrow();
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                participant.FightState.TurnState.ResetForNewTurn();
            }

            planningCompleted = false;
            planningException = null;

            planningCancellationTokenSource = new CancellationTokenSource();

            RunPlanningAsync(context, planningCancellationTokenSource.Token).Forget();
        }

        public StateResult Tick(DuelContext context)
        {
            if (planningException != null)
            {
                throw planningException;
            }

            if (!planningCompleted)
            {
                return StateResult.Stay();
            }

            return StateResult.Switch<RollResolveState>();
        }

        public void Exit(DuelContext context)
        {
            if (planningCancellationTokenSource != null)
            {
                planningCancellationTokenSource.Cancel();
                planningCancellationTokenSource.Dispose();
                planningCancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Последовательно выполняет все шаги планирования хода
        /// </summary>
        private async UniTaskVoid RunPlanningAsync(DuelContext context, CancellationToken cancellationToken)
        {
            try
            {
                IPlayerDecisionSource playerDecisionSource = modules.Get<IPlayerDecisionSource>();
                IBotDecisionSource botDecisionSource = modules.Get<IBotDecisionSource>();

                // Объявление дайса
                
                string declaredDiceId = await playerDecisionSource.GetDeclaration(context, cancellationToken);
                context.Participants[context.PlayerId].FightState.TurnState.DeclareDice(declaredDiceId);

                string botDeclaredDiceId = botDecisionSource.BuildDeclaration(context);
                context.Participants[context.OpponentId].FightState.TurnState.DeclareDice(botDeclaredDiceId);

                // Намерения 
                
                TurnIntent playerIntent = await playerDecisionSource.GetTurnIntent(context, cancellationToken);
                context.Participants[context.PlayerId].FightState.TurnState.ChoseDice(playerIntent.ChosenDice);
                if (string.IsNullOrEmpty(playerIntent.ChosenConsumable))
                    context.Participants[context.PlayerId].FightState.TurnState.ChoseConsumable(playerIntent.ChosenConsumable);

                TurnIntent botIntent = botDecisionSource.BuildTurnIntent(context);
                context.Participants[context.OpponentId].FightState.TurnState.ChoseDice(botIntent.ChosenDice);
                if (string.IsNullOrEmpty(botIntent.ChosenConsumable))
                    context.Participants[context.OpponentId].FightState.TurnState.ChoseConsumable(botIntent.ChosenConsumable);

                planningCompleted = true;
            }
            catch (OperationCanceledException)
            {
                // Нормальная ситуация: стейт покинули раньше завершения ввода
            }
            catch (Exception exception)
            {
                planningException = exception;
            }
        }
    }
}