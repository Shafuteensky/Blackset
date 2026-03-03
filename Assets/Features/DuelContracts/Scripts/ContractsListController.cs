using System.Collections.Generic;
using Blackset.Data.Registries;
using Blackset.Player;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Контроллер списка контрактов дуэлей
    /// </summary>
    public sealed class ContractsListController : PlayerProgressUpdater
    {
        [Header("Данные соперников"), Space]
        [SerializeField]
        private ContractListContainer contractList;
        
        [Header("Параметры контрактов"), Space]
        [SerializeField]
        [Range(1, 6)]
        private int contractsToGenerate = 3;
        
        private ContractGenerator contractGenerator;
        
        private void OnEnable()
        {
            Initialize(contractList != null);
            if (!IsInitialized) return;
            
            contractGenerator ??= new ContractGenerator();
            if (IsUpdateNeeded() || contractList.Data.Count <= 0) UpdateContractList();
        }

        private void UpdateContractList()
        {
            contractList.Clear();
            List<DuelContract> duelContracts = contractGenerator.GetRandomContractsList(contractsToGenerate);
            foreach (DuelContract contract in duelContracts)
            {
                contractList.Add(contract);
            }
        }
    }
}