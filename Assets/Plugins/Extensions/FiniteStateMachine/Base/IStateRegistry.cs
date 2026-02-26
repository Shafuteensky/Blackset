namespace Extensions.FiniteStateMachine
{
    /// <summary>
    /// Реестр всех доступных состояний машины
    /// </summary>
    /// <typeparam name="TContext">Входные данные стейт-машины</typeparam>
    public interface IStateRegistry<TContext>
    {
        /// <summary>
        /// Получить состояние по известному идентиифкатору
        /// </summary>
        /// <param name="stateId">Идентификатор состояния</param>
        /// <returns>Найденное состояние</returns>
        IState<TContext> Get(string stateId);
    }
}