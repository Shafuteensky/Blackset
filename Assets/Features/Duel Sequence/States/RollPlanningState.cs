using System;
using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
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

        private DuelContext context;
        private TurnParticipantState botState;
        private TurnParticipantState playerState;
        private IBotDecisionSource botDecisionSource;
        private IPlayerDecisionSource playerDecisionSource;

        private SelectionState playerLastDeclaration; 

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

            return StateResult.Switch<PreRollEffectState>();
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
                eventHub.Publish(new DeclarationStartedEvent());

                await PlayerDeclareDice(cancellationToken);
                BotDeclareDice();

                eventHub.Publish(new SelectionStartedEvent());

                await PlayerSelectChoices(cancellationToken);
                BotSelectDice();
                BotSelectConsumable();

                ResolveDuelScore();

                planningCompleted = true;
                eventHub.Publish(new PlanningCompletedEvent());
            }
            catch (OperationCanceledException) { }
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
            if (botState.HasPassed.Value) return;
            
            SelectionState botSelection = botDecisionSource.BuildDeclaration(context);
            botState.DeclareDice(botSelection);
            
            if (!botSelection.IsItemSelected) 
                botState.MarkPassed();
            else
            {
                context.Knowledge[context.OpponentId].RevealDice(botSelection.SelectedItemId);
                eventHub.Publish(new DiceDeclarationCompletedEvent(botSelection.SelectedItemId, context.OpponentId));
            }
        }

        /// <summary>
        /// Игрок объявляет дайс
        /// </summary>
        private async UniTask PlayerDeclareDice(CancellationToken cancellationToken)
        {
            if (playerState.HasPassed.Value)
            {
                playerLastDeclaration = new SelectionState();
                return;
            }

            SelectionState playerSelection = await playerDecisionSource.GetSelection(context, cancellationToken);
            playerLastDeclaration = playerSelection;

            if (!playerSelection.IsItemSelected)
                playerState.MarkPassed();
            else
            {
                context.Knowledge[context.PlayerId].RevealDice(playerState.DeclaredDice.Value);
                eventHub.Publish(new DiceDeclarationCompletedEvent(playerState.DeclaredDice.Value, context.PlayerId));
            }
        }

        #endregion

        #region Выборы

        /// <summary>
        /// Бот выбирает дайс
        /// </summary>
        private void BotSelectDice()
        {
            if (botState.HasPassed.Value) return;
            
            SelectionState botSelection = botDecisionSource.BuildDiceSelection(context);
            botState.SelectDice(botSelection);
            
            if (!botSelection.IsItemSelected) 
                botState.MarkPassed();
            else
            {
                context.Knowledge[context.OpponentId].RevealDice(botSelection.SelectedItemId);
                eventHub.Publish(new DiceSelectionCompletedEvent(botSelection.SelectedItemId, context.OpponentId));
            }
        }

        /// <summary>
        /// Бот выбирает расходник
        /// </summary>
        private void BotSelectConsumable()
        {
            if (botState.HasPassed.Value) return;
            
            SelectionState botSelection = botDecisionSource.BuildConsumableSelection(context);
            botState.SelectConsumable(botSelection);
            
            if (botSelection.IsItemSelected)
            {
                context.Knowledge[context.OpponentId].RevealConsumable(botSelection.SelectedItemId);
                eventHub.Publish(
                    new ConsumableSelectionCompletedEvent(botSelection.SelectedItemId, context.OpponentId));
            }
        }
        
        /// <summary>
        /// Игрок совершает выбор дайса и расходника
        /// </summary>
        private async UniTask PlayerSelectChoices(CancellationToken cancellationToken)
        {
            if (playerState.HasPassed.Value) return;
            playerState.SelectDice(playerLastDeclaration);

            SelectionState playerSelection = await playerDecisionSource.GetSelection(context, cancellationToken);

            if (!playerSelection.IsItemSelected)
                playerState.MarkPassed();
            else
            {
                context.Knowledge[context.PlayerId].RevealDice(playerState.SelectedDice.Value);
                eventHub.Publish(new DiceSelectionCompletedEvent(playerState.SelectedDice.Value, context.PlayerId));
            }

            if (playerState.IsConsumableChosen.Value)
            {
                context.Knowledge[context.PlayerId].RevealConsumable(playerState.SelectedConsumable.Value);
                eventHub.Publish(new ConsumableSelectionCompletedEvent(playerState.SelectedConsumable.Value, context.PlayerId));
            }
        }

        #endregion

        #region Зачёт очков

        /// <summary>
        /// Зачёт очков дуэли 
        /// </summary>
        private void ResolveDuelScore()
        {
            IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();

            // За честное объявление
            duelScoreResolver.ResolveHonesty(context, context.PlayerId, 
                playerState.DeclaredDice.Value, playerState.SelectedDice.Value);
            duelScoreResolver.ResolveHonesty(context, context.OpponentId,
                botState.DeclaredDice.Value, botState.SelectedDice.Value);
        }

        #endregion
    }
}