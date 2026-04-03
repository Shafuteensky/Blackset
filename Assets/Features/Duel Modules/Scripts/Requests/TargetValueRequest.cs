using Blackset.Duel.Rules;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Запрос создания целевого значения
    /// </summary>
    public struct TargetValueRequest
    {
        /// <summary>
        /// Сид случайной генерации
        /// </summary>
        public string Seed { get; }
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration DuelRules { get; }

        /// <summary>
        /// Запрос создания целевого значения
        /// </summary>
        /// <param name="seed">Сид случайной генерации</param>
        /// <param name="duelRules">Правила дуэли</param>
        /// <param name="duelMode">Режим дуэли</param>
        public TargetValueRequest(string seed, DuelRulesConfiguration duelRules)
        {
            Seed = seed;
            DuelRules = duelRules;
        }
    }
}