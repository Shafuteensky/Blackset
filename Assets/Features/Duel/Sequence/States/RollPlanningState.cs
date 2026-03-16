using System;
using System.Linq;
using System.Threading;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Cysharp.Threading.Tasks;
using Extensions.FiniteStateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

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
                playerDecisionSource.Initialize(presenter);
                IBotDecisionSource botDecisionSource = modules.Get<IBotDecisionSource>();

                TurnParticipantState playerState = context.Participants[context.PlayerId].FightState.TurnState;
                TurnParticipantState opponentState = context.Participants[context.OpponentId].FightState.TurnState;
                
                // Объявление дайса ————————————————————————————————————————————————————————————————————————————————————
                
                eventHub.Publish(new DeclarationStartedEvent());
                
                string botDeclaredDiceId = botDecisionSource.BuildDeclaration(context);
                opponentState.DeclareDice(botDeclaredDiceId);
                eventHub.Publish(new DeclaredDiceEvent(botDeclaredDiceId, context.OpponentId));
                
                string declaredDiceId = await playerDecisionSource.GetDeclaration(context, cancellationToken);
                if (!playerState.HasPassed.Value)
                {
                    playerState.DeclareDice(declaredDiceId);
                    eventHub.Publish(new DeclaredDiceEvent(declaredDiceId, context.PlayerId));
                }
                
                // Намерения ———————————————————————————————————————————————————————————————————————————————————————————
                
                eventHub.Publish(new PlanningStartedEvent());

                TurnParticipantState botIntent = botDecisionSource.BuildIntentState(context);
                opponentState.ApplyState(botIntent);
                
                TurnParticipantState playerIntent;
                if (!playerState.HasPassed.Value)
                {
                    playerIntent = await playerDecisionSource.GetIntentState(context, cancellationToken);
                }
                else playerIntent = playerState;
                playerState.ApplyState(playerIntent);

                // —————————————————————————————————————————————————————————————————————————————————————————————————————

                // Зачет очков дуэли за честность
                IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();
                duelScoreResolver.ResolveHonesty(context.Participants[context.PlayerId], 
                    declaredDiceId, playerIntent.ChosenDice.Value);
                duelScoreResolver.ResolveHonesty(context.Participants[context.OpponentId], 
                    botDeclaredDiceId, botIntent.ChosenDice.Value);
                
                planningCompleted = true;
                eventHub.Publish(new PlanningCompletedEvent());
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