using System;
using System.Collections.Generic;
using Blackset.Data.Registries;
using Blackset.DuelContracts;
using Extensions.Data.InMemoryData;

namespace Features.Leagues
{
    /// <summary>
    /// Контракт на лигу
    /// </summary>
    public class LeagueContract : InMemoryDataEntry
    {
        public List<DuelContract> DuelContracts { get; private set; } = new();

        private ContractGenerator contractGenerator = new();
        
        /// <summary>
        /// Новый контракт
        /// </summary>
        /// <param name="opponent">Данные соперника</param>
        public LeagueContract()
        {
            int numberOfOpponents = GameData.Instance.LeagueConfig.Opponents.NumberOfOpponents;
            DuelContracts = contractGenerator.GetRandomContractsList(numberOfOpponents);
        }
    }
}