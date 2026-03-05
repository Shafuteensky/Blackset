using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence
{
    /// <summary>
    /// Стейт-машина последовательности дуэли
    /// </summary>
    public class DuelStateMachine : StateMachine<DuelContext>
    {
        /// <summary>
        /// Новая стейт-машина
        /// </summary>
        /// <param name="registry">Реестр состояний</param>
        public DuelStateMachine(IStateRegistry<DuelContext> registry) : base(registry) { }
    }
}