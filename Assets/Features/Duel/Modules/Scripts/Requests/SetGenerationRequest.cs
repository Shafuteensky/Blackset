using Blackset.Data.Items.Types;
using Blackset.Duel.Pools;
using Blackset.Duel.Rules;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Запрос на генерацию сборки учатника дуэли
    /// </summary>
    public struct SetGenerationRequest
    {
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration DuelRules { get; }
        /// <summary>
        /// Сид случайной выборки
        /// </summary>
        public int Seed { get; }
        
        /// <summary>
        /// Пулы предметов
        /// </summary>
        public DuelPoolsContext Pools { get; }
        
        /// <summary>
        /// Используется ли реролл
        /// </summary>
        public bool RerollUsed { get; }
        /// <summary>
        /// Какой дайс рероллится
        /// </summary>
        public DiceType RerollType { get; }

        /// <summary>
        /// Запрос на генерацию сборки участника дуэли
        /// </summary>
        /// <param name="duelRules">Правила дуэли</param>
        /// <param name="seed">Сид случайной выборки</param>
        /// <param name="pools">Пулы предметов</param>
        /// <param name="rerollUsed">Используется ли реролл</param>
        /// <param name="rerollType">Какой дайс рероллится</param>
        public SetGenerationRequest(DuelRulesConfiguration duelRules, int seed, DuelPoolsContext pools,
            bool rerollUsed = false, DiceType rerollType = default)
        {
            Pools = pools;
            Seed = seed;
            DuelRules = duelRules;
            RerollUsed = rerollUsed;
            RerollType = rerollType;
        }
    }
}