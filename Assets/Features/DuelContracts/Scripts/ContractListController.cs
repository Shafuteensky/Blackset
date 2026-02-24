using Blackset.Opponents;
using Blackset.Player;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Контроллер списка контрактов дуэлей
    /// </summary>
    public sealed class ContractListController : PlayerProgressUpdater
    {
        [Header("Данные соперников"), Space]
        [SerializeField]
        private ContractListContainer contractList;
        [SerializeField]
        private OpponentsRegistry opponentsRegistry;
        
        [Header("Параметры контрактов"), Space]
        [SerializeField]
        [Range(1, 6)]
        private int contractsToGenerate = 3;
        
        private ContractGenerator _contractGenerator;
        
        private void OnEnable()
        {
            Initialize(contractList != null && opponentsRegistry != null);
            if (!IsInitialized) return;
            
            _contractGenerator ??= new ContractGenerator(opponentsRegistry);
            if (IsUpdateNeeded()) UpdateContractList();
        }

        private void UpdateContractList()
        {
            contractList.Clear();
            for (int i = 0; i < contractsToGenerate; i++)
            {
                DuelContract newContract = _contractGenerator.GetRandomOpponent();
                contractList.Add(newContract);
            }
        }
    }
}