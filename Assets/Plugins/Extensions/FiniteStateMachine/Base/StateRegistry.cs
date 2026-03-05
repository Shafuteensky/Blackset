using System;
using System.Collections.Generic;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence
{
    /// <summary>
    /// Реестр всех доступных состояний машины
    /// </summary>
    public sealed class StateRegistry<TContext> : IStateRegistry<TContext>
    {
        private readonly Dictionary<Type, IState<TContext>> states = new();

        public void Add<TState>(TState state) where TState : class, IState<TContext>
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            states[typeof(TState)] = state;
        }

        public IState<TContext> Get(Type stateType)
        {
            if (states.TryGetValue(stateType, out IState<TContext> state))
                return state;

            throw new InvalidOperationException(
                $"Состояние '{stateType.Name}' не найдено в реестре. Вызовите Add() при инициализации.");
        }
    }
}