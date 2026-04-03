using System;
using Blackset.BalanceConfigs;
using UnityEngine;

namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Конфигурация правил лиги
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(LeagueConfig),
        menuName = "Blackset/League/" + nameof(LeagueConfig))]
    public class LeagueConfig : BaseBalanceConfig
    {
        #region Инспектор

        [Header("Участники в лиге (помимо игрока)"), Space]
        [SerializeField]
        public OpponentsSettings Opponents = new()
        {
            NumberOfOpponents = 3,
        };

        #endregion

        #region Вспомогательные типы

        [Serializable]
        public struct OpponentsSettings
        {
            public int NumberOfOpponents;
        }

        #endregion
    }
}