using System;
using System.Collections.Generic;
using Blackset.Duel.Requests;
using Blackset.Duel.Sequence;
using Blackset.DuelContracts;
using Blackset.DuelEvents.EventTypes;
using Extensions.Log;
using Extensions.Singleton;
using Features.Duel.Data.FightEnd;
using Unity.VisualScripting;
using UnityEngine;

namespace Features.Leagues
{
    /// <summary>
    /// Контроллер лиги
    /// </summary>
    public sealed class LeagueController : MonoBehaviourSingleton<LeagueController>
    {
        /// <summary>
        /// Событие победы игроком в лиге (победы в последней дуэле лиге)
        /// </summary>
        public event Action LeagueWonEvent;
        /// <summary>
        /// Событие поражения игрока в лиге
        /// </summary>
        public event Action LeagueLoseEvent;
        /// <summary>
        /// Событие победы игроком в дуэле лиги (победы в не последней дуэле лиге)
        /// </summary>
        public event Action LeagueDuelWonEvent;

        [SerializeField] private LeagueDataContainer leagueDataContainer;
        [SerializeField] private SelectedContract selectedContract;
        [SerializeField] private ContractListContainer contractListContainer;

        private DuelController duelController;

        private int currentDuelIndex;
        private bool isLeagueActive;

        protected override void Awake()
        {
            base.Awake();
            duelController = DuelController.Instance;

            ServiceGuard.NotNull(duelController, nameof(duelController));
            ServiceGuard.NotNull(leagueDataContainer, nameof(leagueDataContainer));

            duelController.EventHub.Subscribe<DuelEndEvent>(OnDuelEnded);
        }

        private void OnDestroy()
        {
            if (duelController == null) return;
            duelController.EventHub.Unsubscribe<DuelEndEvent>(OnDuelEnded);
        }

        /// <summary>
        /// Начать лигу
        /// </summary>
        public void StartLeague()
        {
            if (duelController == null || leagueDataContainer == null) return;

            if (leagueDataContainer.Data == null) return;
            if (leagueDataContainer.Data.DuelContracts == null) return;
            if (leagueDataContainer.Data.DuelContracts.Count == 0) return;

            currentDuelIndex = 0;
            isLeagueActive = true;

            contractListContainer.Clear();
            Debug.Log(leagueDataContainer.Data.DuelContracts.Count);
            leagueDataContainer.GenerateNewLeague();
            Debug.Log(leagueDataContainer.Data.DuelContracts.Count);
            foreach (DuelContract contract in leagueDataContainer.Data.DuelContracts)
            {
                contractListContainer.Add(contract);
                Debug.Log(contract);
            }
            
            StartCurrentDuel();
        }

        private void StartCurrentDuel()
        {
            if (!isLeagueActive) return;
            if (leagueDataContainer.Data == null) return;
            if (leagueDataContainer.Data.DuelContracts == null) return;

            if (currentDuelIndex >= leagueDataContainer.Data.DuelContracts.Count)
            {
                isLeagueActive = false;
                return;
            }

            DuelContract activeContract = leagueDataContainer.Data.DuelContracts[currentDuelIndex];
            selectedContract.Select(activeContract.Id);
            
            DuelStartRequest request = new DuelStartRequest(activeContract);
            duelController.StartDuel(request);
        }

        private void OnDuelEnded(DuelEndEvent handler)
        {
            if (!isLeagueActive)
            {
                LeagueWonEvent?.Invoke();
                return;
            }

            switch (handler.DuelEndResult.Winner.Value)
            {
                case FightWinner.Player:
                    LeagueDuelWonEvent?.Invoke();
                    break;
                
                case FightWinner.Opponent:
                    LeagueLoseEvent?.Invoke();
                    return;
            }
            
            currentDuelIndex++;
            StartCurrentDuel();
        }
    }
}