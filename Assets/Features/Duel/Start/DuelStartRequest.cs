using Blackset.Duel.Rules;
using Blackset.DuelContracts;
using Blackset.Storms;

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
        /// Активен ли шторм
        /// </summary>
        public bool IsStormActive => Storm != null;
        /// <summary>
        /// Активный шторм
        /// </summary>
        public Storm Storm;

        /// <summary>
        /// Запрос начала дуэли по опрделенному контракту
        /// </summary>
        /// <param name="contract">Выбранный контракт дуэли</param>
        /// <param name="rulesConfiguration">Конфигурация правил дуэли</param>
        /// <param name="mode"></param>
        public DuelStartRequest(DuelContract contract, Storm storm = null)
        {
            Contract = contract;
            Storm = storm;
        }
    }
}