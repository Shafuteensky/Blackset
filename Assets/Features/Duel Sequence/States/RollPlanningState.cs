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
                // Начало стадии объявления (блеф)
                eventHub.Publish(new DeclarationStartedEvent());
                // Объявление дайса
                BotDeclareDice();
                await PlayerDeclareDice(cancellationToken);

                // Начало стадии выборов действий
                eventHub.Publish(new SelectionStartedEvent());
                // Выбор дайса
                BotSelectDice();
                await PlayerSelectDice(cancellationToken);
                // Выбор цели дайса
                BotSelectDiceTarget();
                await PlayerSelectDiceTarget(cancellationToken);
                // Выбор расходника
                BotSelectConsumable();
                await PlayerSelectConsumable(cancellationToken);
                // Выбор цели дайса
                BotSelectConsumableTarget();
                await PlayerSelectConsumableTarget(cancellationToken);

                // Зачёт очков за честность
                ResolveDuelScore();

                // Окончание планирования
                planningCompleted = true;
                eventHub.Publish(new PlanningCompletedEvent());
            }
            catch (OperationCanceledException) { } // Нормальная ситуация: стейт покинули раньше завершения ввода
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

        #region Выбор цели дайса

        /// <summary>
        /// Бот выбирает цель дайса
        /// </summary>
        private void BotSelectDiceTarget()
        {
            // TODO
            SelectionState botSelection = botDecisionSource.BuildDiceSelection(context);
            botState.SelectDice(botSelection);
        }

        /// <summary>
        /// Игрок выбирает цель дайса
        /// </summary>
        private async UniTask PlayerSelectDiceTarget(CancellationToken cancellationToken)
        {
            // TODO
            SelectionState playerSelection = playerState.HasPassed.Value
                ? new SelectionState()
                : await playerDecisionSource.GetSelection(context, cancellationToken);
            playerState.SelectDice(playerSelection);
        }

        #endregion

        #region Выбор расходника

        /// <summary>
        /// Бот выбирает расходник
        /// </summary>
        private void BotSelectConsumable()
        {
            // TODO
            SelectionState botSelection = botDecisionSource.BuildDiceSelection(context);
            botState.SelectDice(botSelection);
        }

        /// <summary>
        /// Игрок выбирает расходник
        /// </summary>
        private async UniTask PlayerSelectConsumable(CancellationToken cancellationToken)
        {
            // TODO
            SelectionState playerSelection = playerState.HasPassed.Value
                ? new SelectionState()
                : await playerDecisionSource.GetSelection(context, cancellationToken);
            playerState.SelectDice(playerSelection);
        }

        #endregion

        #region Выбор цели дайса

        /// <summary>
        /// Бот выбирает цель расходника
        /// </summary>
        private void BotSelectConsumableTarget()
        {
            // TODO
            SelectionState botSelection = botDecisionSource.BuildDiceSelection(context);
            botState.SelectDice(botSelection);
        }

        /// <summary>
        /// Игрок выбирает  цель расходника
        /// </summary>
        private async UniTask PlayerSelectConsumableTarget(CancellationToken cancellationToken)
        {
            // TODO
            SelectionState playerSelection = playerState.HasPassed.Value
                ? new SelectionState()
                : await playerDecisionSource.GetSelection(context, cancellationToken);
            playerState.SelectDice(playerSelection);
        }

        #endregion

        #region Зачёт очков

        /// <summary>
        /// Зачёт очков дуэли
        /// </summary>
        private void ResolveDuelScore()
        {
            IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();

            // Очки за честность объявления
            duelScoreResolver.ResolveHonesty(context.Participants[context.PlayerId],
                playerDeclaration, playerState.SelectedDice.Value.ItemId);
            duelScoreResolver.ResolveHonesty(context.Participants[context.OpponentId],
                botDeclaration, botState.SelectedDice.Value.ItemId);
        }

        #endregion
    }
}