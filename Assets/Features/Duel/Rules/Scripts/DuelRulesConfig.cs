using UnityEngine;

namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Конфигурация правил дуэли
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Duel/" + nameof(DuelRulesConfig),
        fileName = nameof(DuelRulesConfig))]
    public sealed class DuelRulesConfig : ScriptableObject
    {
        [Header("Сборки"), Space]
        
        [SerializeField, Tooltip("Максимум рероллов на класс предметов (дайсы, расходники)")]
        [Range(0, 6)]
        private int maxRerolls = 1;
        
        [Header("Дуэль"), Space]
        
        [SerializeField, Tooltip("Политика победы в дуэли")]
        private DuelWinPolicy duelWinPolicy = DuelWinPolicy.WinMostFights;
        
        [Header("Битвы"), Space]
        
        [SerializeField, Tooltip("Максимальное количество битв за дуэль")]
        [Range(1, 5)]
        private int maxFightsPerDuel = 3;
        
        [SerializeField, Tooltip("Политика победы в битве в составе дуэли")]
        private FightWinPolicy fightWinPolicy = FightWinPolicy.ExactOrClosest;
        
        [SerializeField, Tooltip("Политика проигрыша в битве в составе дуэли")]
        private FightLossPolicy fightLossPolicy = FightLossPolicy.LessOrBust;
        
        [Header("Броски"), Space]
        
        [SerializeField, Tooltip("Максимальное количество ходов (бросков дайсов) за битву")]
        [Range(1, 12)]
        private int maxThrowsPerFight = 6;
        
        [SerializeField, Tooltip("Политика броска дайса в течении одной битвы")]
        private DiceThrowPolicy diceThrowPolicy = DiceThrowPolicy.Once;
        
        [SerializeField, Tooltip("Политика применения эффектов")]
        private EffectsPolicy effectsPolicy = EffectsPolicy.Both;
        
    }
}