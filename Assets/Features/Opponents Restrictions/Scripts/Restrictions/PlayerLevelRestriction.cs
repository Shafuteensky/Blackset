using Blackset.Data.Registries;
using Blackset.Opponents;
using Extensions.Types;
using UnityEngine;

namespace Blackset.OpponentsRestrictions
{
    /// <summary>
    /// Ограничение доступности соперника по уровню игрока
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Opponents/Restrictions/" + nameof(PlayerLevelRestriction),
        fileName = "OppRest_PlayerLevel")]
    public sealed class PlayerLevelRestriction : OpponentRestriction
    {
        [Header("Уровень игрока"), Space]
        [SerializeField] private int requiredLevel;
        [SerializeField] private ComparisonType comparisonType = ComparisonType.GreaterOrEqual;
        
        /// <summary>
        /// Блокирует доступность соперника, если сравнение уровня игрока не проходит условие
        /// </summary>
        public override OpponentAvailability GetBlockedAvailability(OpponentData item)
        {
            int playerLevel = GameData.Instance.PlayerDataFacade.MetaData.Data.GetPlayerLvl();

            if (!Comparator.IsConditionPassed(playerLevel, comparisonType, requiredLevel)) 
                return blockedAvailability;
            else
                return OpponentAvailability.None;
        }
    }
}