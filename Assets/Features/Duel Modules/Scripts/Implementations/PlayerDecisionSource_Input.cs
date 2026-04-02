using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI.
    /// Реализует <see cref="IDuelInputHandler"/> — UI обращается напрямую к этому классу.
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource, IDuelInputHandler
    {
        private DuelContext duelContext;
        private string expectedParticipantId;
        private bool awaitingInput;
        private PlanningStageType currentStage;

        private UniTaskCompletionSource<SelectionState> inputCompletionSource;

        #region IPlayerDecisionSource

        /// <summary>
        /// Получить подтверждение выбора от игрока
        /// </summary>
        public UniTask<SelectionState> GetSelection(DuelContext context, CancellationToken cancellationToken)
        {
            ServiceGuard.NotNull(context, nameof(context));

            duelContext = context;
            expectedParticipantId = context.PlayerId;
            awaitingInput = true;
            currentStage = ResolveCurrentStage(context);

            inputCompletionSource = new UniTaskCompletionSource<SelectionState>();

            cancellationToken.Register(CancelInput);

            return inputCompletionSource.Task;
        }

        #endregion

        #region IDuelInputHandler

        /// <summary>
        /// Вызывается UI при выборе дайса участником
        /// </summary>
        public void OnDiceSelected(string participantId, SelectionState selection)
        {
            if (!CanAccept(participantId)) return;

            TurnParticipantState turnState = duelContext.Participants[participantId].FightState.TurnState;

            switch (currentStage)
            {
                case PlanningStageType.Declaration:
                {
                    turnState.ToggleDeclaredDice(selection.SelectedItemId);
                    break;
                }
                case PlanningStageType.Selection:
                {
                    turnState.ToggleSelectedDice(selection.SelectedItemId);
                    break;
                }
            }
        }

        /// <summary>
        /// Вызывается UI при выборе расходника участником
        /// </summary>
        public void OnConsumableSelected(string participantId, SelectionState selection)
        {
            if (!CanAccept(participantId) || currentStage != PlanningStageType.Selection) return;

            TurnParticipantState turnState = duelContext.Participants[participantId].FightState.TurnState;
            turnState.ToggleSelectedConsumable(selection.SelectedItemId);
        }
        
        /// <summary>
        /// Вызывается UI при выборе цели расходника участником
        /// </summary>
        /// <param name="participantId">Участник, применяющий расходник</param>
        /// <param name="targetParticipantId">Цель применения его расходника</param>
        public void OnConsumableTargetSelected(string participantId, string targetParticipantId)
        {
            if (!CanAccept(participantId) || currentStage != PlanningStageType.Selection) return;

            TurnParticipantState turnState = duelContext.Participants[participantId].FightState.TurnState;
            turnState.SetSelectedTargetParticipantId(targetParticipantId);
        }


        /// <summary>
        /// Вызывается UI при подтверждении текущего этапа
        /// </summary>
        public void OnInputCompletion()
        {
            if (!awaitingInput || !CanConfirmCurrentStage()) return;

            CompleteInput(BuildCurrentSelectionState());
        }

        /// <summary>
        /// Вызывается UI при запросе паса
        /// </summary>
        public void OnPassRequested()
        {
            if (!awaitingInput) return;

            duelContext.Participants[duelContext.PlayerId].FightState.TurnState.MarkPassed();
            CompleteInput(new SelectionState());
        }

        #endregion

        #region Завершение ввода

        private void CompleteInput(SelectionState result)
        {
            awaitingInput = false;
            expectedParticipantId = string.Empty;
            currentStage = PlanningStageType.None;
            inputCompletionSource.TrySetResult(result);
        }

        private void CancelInput()
        {
            awaitingInput = false;
            expectedParticipantId = string.Empty;
            currentStage = PlanningStageType.None;
            inputCompletionSource.TrySetCanceled();
        }

        #endregion

        #region Вспомогательная логика

        /// <summary>
        /// Проверяет, может ли текущий ввод быть принят от данного участника
        /// </summary>
        private bool CanAccept(string participantId) => awaitingInput && participantId == expectedParticipantId;

        /// <summary>
        /// Определить текущий этап ввода для игрока
        /// </summary>
        private PlanningStageType ResolveCurrentStage(DuelContext context)
        {
            TurnParticipantState turnState = context.Participants[context.PlayerId].FightState.TurnState;

            if (!turnState.IsDiceDeclared.Value)
                return PlanningStageType.Declaration;
            else
                return PlanningStageType.Selection;
        }

        /// <summary>
        /// Проверить, доступно ли подтверждение текущего этапа
        /// </summary>
        private bool CanConfirmCurrentStage()
        {
            TurnParticipantState turnState = duelContext.Participants[duelContext.PlayerId].FightState.TurnState;

            switch (currentStage)
            {
                case PlanningStageType.Declaration:
                    return turnState.IsDiceDeclared.Value;
                case PlanningStageType.Selection:
                    return turnState.IsDiceChosen.Value;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Собрать текущее состояние выбора для завершения этапа
        /// </summary>
        private SelectionState BuildCurrentSelectionState()
        {
            TurnParticipantState turnState = duelContext.Participants[duelContext.PlayerId].FightState.TurnState;

            switch (currentStage)
            {
                case PlanningStageType.Declaration:
                {
                    if (!turnState.IsDiceDeclared.Value)
                        return new SelectionState();
                    else
                        return new SelectionState(turnState.DeclaredDice.Value);
                }
                case PlanningStageType.Selection:
                {
                    if (!turnState.IsDiceChosen.Value)
                        return new SelectionState();
                    else
                        return new SelectionState(turnState.SelectedDice.Value);
                }
                default:
                    return new SelectionState();
            }
        }

        #endregion
    }
}