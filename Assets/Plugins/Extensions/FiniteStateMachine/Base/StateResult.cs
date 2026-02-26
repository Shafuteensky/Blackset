namespace Extensions.FiniteStateMachine
{
    /// <summary>
    /// Результат исполнения тика состояния
    /// </summary>
    public readonly struct StateResult
    {
        /// <summary>
        /// Тип перехода в состояние
        /// </summary>
        public StateTransition Transition { get; }
        /// <summary>
        /// Идентификатор следующего состояния
        /// </summary>
        public string NextStateId { get; }

        /// <summary>
        /// Конструктор результата тика
        /// </summary>
        /// <param name="transition">Тип перехода в состояние</param>
        /// <param name="nextStateId">Идентификатор следующего состояния</param>
        private StateResult(StateTransition transition, string nextStateId = null)
        {
            Transition = transition;
            NextStateId = nextStateId;
        }

        /// <summary>
        /// Остаться в текущем состоянии
        /// </summary>
        /// <returns>Результат исполнения тика состояния</returns>
        public static StateResult StayInState() => new StateResult(StateTransition.Stay);
        /// <summary>
        /// Переключить состояние на другое
        /// </summary>
        /// <returns>Результат исполнения тика состояния</returns>
        public static StateResult Switch(string nextStateId) => new StateResult(StateTransition.Switch, nextStateId);
        /// <summary>
        /// Push состояния
        /// </summary>
        /// <returns>Результат исполнения тика состояния</returns>
        public static StateResult Push(string nextStateId) => new StateResult(StateTransition.Push, nextStateId);
        /// <summary>
        /// Pop состояния
        /// </summary>
        /// <returns>Результат исполнения тика состояния</returns>
        public static StateResult Pop() => new StateResult(StateTransition.Pop);
    }
}