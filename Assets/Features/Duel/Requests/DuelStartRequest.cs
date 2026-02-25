using Blackset.Duel.Rules;
using Blackset.DuelContracts;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Структура запроса начала дуэли
    /// </summary>
    public struct DuelStartRequest
    {
        /// <summary>
        /// Данные выбранного контракта
        /// </summary>
        public DuelContract Contract;

        /// <summary>
        /// Конфигурация правил дуэли
        /// </summary>
        public DuelRulesConfig RulesConfig;
        
        /// <summary>
        /// Режим дуэли
        /// </summary>
        public DuelMode DuelMode;

        public DuelStartRequest(DuelContract contract, DuelRulesConfig rulesConfig, DuelMode mode)
        {
            Contract = contract;
            RulesConfig = rulesConfig;
            DuelMode = mode;
        }
    }
}