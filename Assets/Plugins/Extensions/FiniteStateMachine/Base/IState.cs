namespace Extensions.FiniteStateMachine
{
    /// <summary>
    /// Состояние машины
    /// </summary>
    /// <typeparam name="TContext">Входные данные стейт-машины</typeparam>
    public interface IState<TContext>
    {
        /// <summary>
        /// Запуск состояния
        /// </summary>
        /// <param name="context">Входные данные стейт-машины</param>
        void Enter(TContext context);
        /// <summary>
        /// Тик состояния
        /// </summary>
        /// <returns>Результат исполнения тика состояния</returns>
        /// <param name="context">Входные данные стейт-машины</param>
        StateResult Tick(TContext context);
        /// <summary>
        /// Выход из состояния
        /// </summary>
        /// <param name="context">Входные данные стейт-машины</param>
        void Exit(TContext context);
    }
}