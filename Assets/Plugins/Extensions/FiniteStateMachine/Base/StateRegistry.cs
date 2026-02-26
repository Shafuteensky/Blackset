using System;
using System.Collections.Generic;

namespace Extensions.FiniteStateMachine
{
    /// <summary>
    /// Реестр всех доступных состояний машины
    /// </summary>
    public class StateRegistry<TContext> : IStateRegistry<TContext> // TODO заменить хранение по id-ключу на конкретный тип?
    {
        protected readonly Dictionary<string, IState<TContext>> states = new Dictionary<string, IState<TContext>>();

        /// <summary>
        /// Добавить состояние в реестр
        /// </summary>
        /// <param name="id">Идентификатор состояния</param>
        /// <param name="state">Конкретное состояние</param>
        /// <exception cref="ArgumentException">Ошибка идентификации состояния</exception>
        /// <exception cref="ArgumentNullException">Ошибка состояния</exception>
        public void Add(string id, IState<TContext> state)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("State id is null or empty.", nameof(id));
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (states.ContainsKey(id)) throw new ArgumentException($"State '{id}' is already registered.", nameof(id));

            states[id] = state;
        }

        /// <summary>
        /// Получить состояние по идентификатору
        /// </summary>
        /// <param name="stateId">Идентификатор состояния</param>
        /// <returns>Конкретное состояние</returns>
        /// <exception cref="ArgumentException">Ошибка идентификации состояния</exception>
        /// <exception cref="KeyNotFoundException">Ошибка поиска в реестре</exception>
        public IState<TContext> Get(string stateId)
        {
            if (string.IsNullOrEmpty(stateId)) throw new ArgumentException("State id is null or empty.", nameof(stateId));

            IState<TContext> state;
            if (!states.TryGetValue(stateId, out state))
            {
                throw new KeyNotFoundException($"State '{stateId}' not found in registry. States count = {states.Count}.");
            }

            return state;
        }
    }
}