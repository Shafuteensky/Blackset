using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.Snapshots;
using Blackset.Effects;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Применение эффектов фазы PreRoll
    /// </summary>
    public class PreRollEffectState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            TurnSnapshot snapshot = context.Progress.CurrentTurnSnapshot;
            if (snapshot == null)
            {
                snapshot = new TurnSnapshot(context);
                context.Progress.CurrentTurnSnapshot = snapshot;
            }

            IEffectSourceCollector sourceCollector = modules.Get<IEffectSourceCollector>();

            foreach (string participantId in context.Participants.Keys)
            {
                if (!context.Participants.TryGetValue(participantId, out DuelParticipantState participant)) continue;
                TurnParticipantState turnState = participant.FightState.TurnState;
                if (turnState.HasPassed.Value) continue;

                List<EffectSourceRef> sources = sourceCollector.Collect(context, participantId, EffectPhase.PreRoll);

                foreach (EffectSourceRef source in sources)
                {
                    ItemUsageState usageState = ResolveUsageState(participant.FightState, source);
                    if (usageState == null) continue;

                    if (IsCurrentlySelectedSource(turnState, source) && !usageState.IsUsed)
                    {
                        usageState.MarkUsed(context.Progress.ThrowNumber.Value, participantId);
                    }

                    EffectApplyContext applyContext = new EffectApplyContext(
                        context,
                        snapshot,
                        EffectPhase.PreRoll,
                        context.Progress.ThrowNumber.Value,
                        participantId,
                        string.IsNullOrEmpty(usageState.TargetParticipantId) ? participantId : usageState.TargetParticipantId,
                        source.SourceInstanceId,
                        source.SourceKind,
                        turnState.SelectedDice.Value,
                        turnState.SelectedConsumable.Value,
                        usageState);

                    source.Effect.TryApplyEffect(applyContext);
                }
            }
        }

        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<RollResolveState>();
        }

        public void Exit(DuelContext context) { }

        #region Inner
        
        private ItemUsageState ResolveUsageState(FightParticipantState fightState, EffectSourceRef source)
        {
            switch (source.SourceKind)
            {
                case EffectSourceKind.Dice:
                    return fightState.GetOrCreateDiceUsageState(source.SourceInstanceId);

                case EffectSourceKind.Consumable:
                    return fightState.GetOrCreateConsumableUsageState(source.SourceInstanceId);

                default:
                    return null;
            }
        }

        private bool IsCurrentlySelectedSource(TurnParticipantState turnState, EffectSourceRef source)
        {
            switch (source.SourceKind)
            {
                case EffectSourceKind.Dice:
                    return turnState.IsDiceChosen.Value && turnState.SelectedDice.Value == source.SourceInstanceId;

                case EffectSourceKind.Consumable:
                    return turnState.IsConsumableChosen.Value && turnState.SelectedConsumable.Value == source.SourceInstanceId;

                default:
                    return false;
            }
        }
        
        #endregion
    }
}