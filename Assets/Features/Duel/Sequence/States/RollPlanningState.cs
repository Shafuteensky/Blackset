using System;
using System.Linq;
using System.Threading;
using Blackset.DecisionInput;
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
                SelectionState playerSelection = new SelectionState();
                SelectionState botSelection = new SelectionState();
                
                IBotDecisionSource botDecisionSource = modules.Get<IBotDecisionSource>();
                IPlayerDecisionSource playerDecisionSource = modules.Get<IPlayerDecisionSource>();
                playerDecisionSource.Initialize(presenter);

                TurnParticipantState botState = context.Participants[context.OpponentId].FightState.TurnState;
                TurnParticipantState playerState = context.Participants[context.PlayerId].FightState.TurnState;
                
                string playerDeclaration = string.Empty;
                string botDeclaration = string.Empty;
                
                // Объявление дайса ————————————————————————————————————————————————————————————————————————————————————
                
                eventHub.Publish(new DeclarationStartedEvent());
                
                // Бот объявляет дайс
                botDeclaration = botDecisionSource.BuildDeclaration(context);
                botSelection.SelectItem(botDeclaration);
                botState.DeclareDice(botSelection);
                eventHub.Publish(new DeclaredDiceEvent(botDeclaration, context.OpponentId));
                
                // Игрок объявляет дайс
                playerSelection = await playerDecisionSource.GetSelection(context, cancellationToken);
                if (!playerState.HasPassed.Value) // Если не спасовал
                {
                    playerDeclaration = playerSelection.SelectedItemId;
                    playerState.DeclareDice(playerSelection);
                    eventHub.Publish(new DeclaredDiceEvent(playerSelection.SelectedItemId, context.PlayerId));
                }
                
                // Выбор дайса —————————————————————————————————————————————————————————————————————————————————————————
                
                eventHub.Publish(new PlanningStartedEvent());

                // Бот выбирает дайс
                botSelection = botDecisionSource.BuildDiceSelection(context);
                botState.SelectDice(botSelection);
                
                // Игрок выбирает дайс
                if (!playerState.HasPassed.Value)
                {
                    playerSelection = await playerDecisionSource.GetSelection(context, cancellationToken);
                }
                else playerSelection = new SelectionState();
                playerState.SelectDice(playerSelection);

                // —————————————————————————————————————————————————————————————————————————————————————————————————————

                // Зачет очков дуэли за честность
                IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();
                duelScoreResolver.ResolveHonesty(context.Participants[context.PlayerId], 
                    playerDeclaration, playerState.SelectedDice.Value.ItemId);
                duelScoreResolver.ResolveHonesty(context.Participants[context.OpponentId], 
                    botDeclaration, botState.SelectedDice.Value.ItemId);
                
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