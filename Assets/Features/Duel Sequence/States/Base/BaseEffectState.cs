using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Blackset.Effects;
using UnityEngine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Базовый класс для стейтов применения эффектов.
    /// Содержит общий цикл обхода участников и применения эффектов.
    /// </summary>
    public abstract class BaseEffectState : BaseDuelState
    {
        protected abstract EffectPhase Phase { get; }

        protected void ApplyEffects(DuelContext context)
        {
            IEffectSourceCollector sourceCollector = modules.Get<IEffectSourceCollector>();

            foreach (string participantId in context.Participants.Keys)
            {
                if (!context.Participants.TryGetValue(participantId, out DuelParticipantState participant)) continue;

                TurnParticipantState turnState = participant.FightState.TurnState;
                if (turnState.HasPassed.Value) continue;

                List<EffectSourceRef> sources = sourceCollector.Collect(context, participantId, Phase);

                foreach (EffectSourceRef source in sources)
                {
                    ItemUsageState usageState = ResolveUsageState(participant.FightState, source);
                    if (usageState == null) continue;

                    if (IsCurrentlySelectedSource(turnState, source) && !usageState.IsUsed)
                        usageState.MarkUsed(context.Progress.ThrowNumber.Value, participantId);

                    string targetParticipantId = string.IsNullOrEmpty(usageState.TargetParticipantId)
                        ? participantId
                        : usageState.TargetParticipantId;
                        
                    EffectApplyContext applyContext = new EffectApplyContext(
                        context,
                        Phase,
                        context.Progress.ThrowNumber.Value,
                        participantId,
                        targetParticipantId,
                        source.SourceInstanceId,
                        source.SourceKind,
                        turnState.SelectedDice.Value,
                        turnState.SelectedConsumable.Value,
                        usageState);

                    bool applied = source.Effect.TryApplyEffect(applyContext);
                    if (applied)
                    {
                        eventHub.Publish(new EffectAppliedEvent(
                            participantId,
                            targetParticipantId,
                            source.SourceInstanceId,
                            Phase,
                            source.Effect.GetType().Name
                            ));
                    }
                }
            }
        }

        #region Inner

        private static ItemUsageState ResolveUsageState(FightParticipantState fightState, EffectSourceRef source)
        {
            switch (source.SourceKind)
            {
                case EffectSourceKind.Dice:       return fightState.GetOrCreateDiceUsageState(source.SourceInstanceId);
                case EffectSourceKind.Consumable:
                {
                    return fightState.GetOrCreateConsumableUsageState(source.SourceInstanceId);
                }
                default:                          return null;
            }
        }

        private static bool IsCurrentlySelectedSource(TurnParticipantState turnState, EffectSourceRef source)
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