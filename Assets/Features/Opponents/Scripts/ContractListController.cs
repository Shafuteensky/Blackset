using Blackset.Player;
using UnityEngine;

namespace Blackset.Opponents
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
        
        private OpponentGenerator opponentGenerator;
        
        private void OnEnable()
        {
            Initialize(contractList != null && opponentsRegistry != null);
            if (!IsInitialized) return;
            
            opponentGenerator ??= new OpponentGenerator(opponentsRegistry);
            if (IsUpdateNeeded()) UpdateContractList();
        }

        private void UpdateContractList()
        {
            contractList.Clear();
            for (int i = 0; i < contractsToGenerate; i++)
            {
                OpponentContract newContract = opponentGenerator.GetRandomOpponent();
                contractList.Add(newContract);
            }
        }
    }
}