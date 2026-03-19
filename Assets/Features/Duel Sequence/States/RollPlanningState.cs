using System;
using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Cysharp.Threading.Tasks;
using Extensions.FiniteStateMachine;
using UnityEngine;

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

        private DuelContext context;
        private TurnParticipantState botState;
        private TurnParticipantState playerState;
        private IBotDecisionSource botDecisionSource;
        private IPlayerDecisionSource playerDecisionSource;

        private string playerDeclaration;
        private string botDeclaration;

        #region IState

        public void Enter(DuelContext context)
        {
            context.Progress.OnNewThrow();
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                participant.FightState.TurnState.ResetForNewTurn();
            }

            this.context = context;
            botState = context.Participants[context.OpponentId].FightState.TurnState;
            playerState = context.Participants[context.PlayerId].FightState.TurnState;
            botDecisionSource = modules.Get<IBotDecisionSource>();
            playerDecisionSource = modules.Get<IPlayerDecisionSource>();

            planningCompleted = false;
            planningException = null;

            planningCancellationTokenSource = new CancellationTokenSource();

            RunPlanningAsync(planningCancellationTokenSource.Token).Forget();
        }

        public StateResult Tick(DuelContext context)
        {
            // Ошибка планирования (метод RunPlanningAsync)
            if (planningException != null) throw planningException;
            // Ожидание ввода, если не завершен
            if (!planningCompleted) return StateResult.Stay();

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

        #endregion

        #region Планирование хода

        /// <summary>
        /// Последовательно выполняет все шаги планирования хода
        /// </summary>
        private async UniTaskVoid RunPlanningAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Объявление дайса
                
                eventHub.Publish(new DeclarationStartedEvent());
                
                BotDeclareDice();
                await PlayerDeclareDice(cancellationToken);

                // Выбор дайса
                
                eventHub.Publish(new PlanningStartedEvent());
                
                BotSelectDice();
                await PlayerSelectDice(cancellationToken);

                // Зачёт очков за честность
                
                ResolveHonesty();

                // Окончание планирования
                
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

        #endregion

        #region Объявление дайса

        /// <summary>
        /// Бот объявляет дайс
        /// </summary>
        private void BotDeclareDice()
        {
            botDeclaration = botDecisionSource.BuildDeclaration(context);
            SelectionState botSelection = new SelectionState();
            botSelection.SelectItem(botDeclaration);
            botState.DeclareDice(botSelection);
            eventHub.Publish(new DeclaredDiceEvent(botDeclaration, context.OpponentId));
        }

        /// <summary>
        /// Игрок объявляет дайс
        /// </summary>
        private async UniTask PlayerDeclareDice(CancellationToken cancellationToken)
        {
            SelectionState playerSelection = await playerDecisionSource.GetSelection(context, cancellationToken);
            playerDeclaration = string.Empty;
            if (!playerState.HasPassed.Value) // Если не спасовал
            {
                playerDeclaration = playerSelection.SelectedItemId;
                eventHub.Publish(new DeclaredDiceEvent(playerDeclaration, context.PlayerId));
            }
            playerState.DeclareDice(playerSelection);
        }

        #endregion

        #region Выбор дайса

        /// <summary>
        /// Бот выбирает дайс
        /// </summary>
        private void BotSelectDice()
        {
            SelectionState botSelection = botDecisionSource.BuildDiceSelection(context);
            botState.SelectDice(botSelection);
        }

        /// <summary>
        /// Игрок выбирает дайс
        /// </summary>
        private async UniTask PlayerSelectDice(CancellationToken cancellationToken)
        {
            SelectionState playerSelection = playerState.HasPassed.Value
                ? new SelectionState()
                : await playerDecisionSource.GetSelection(context, cancellationToken);
            playerState.SelectDice(playerSelection);
        }

        #endregion

        #region Зачёт очков

        /// <summary>
        /// Зачёт очков дуэли за честность объявления
        /// </summary>
        private void ResolveHonesty()
        {
            IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();

            duelScoreResolver.ResolveHonesty(context.Participants[context.PlayerId],
                playerDeclaration, playerState.SelectedDice.Value.ItemId);

            duelScoreResolver.ResolveHonesty(context.Participants[context.OpponentId],
                botDeclaration, botState.SelectedDice.Value.ItemId);
        }

        #endregion
    }
}