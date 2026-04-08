using System;
using System.Collections.Generic;
using Blackset.Duel.Requests;
using Blackset.Duel.Sequence;
using Blackset.DuelContracts;
using Blackset.DuelEvents.EventTypes;
using Extensions.Log;
using Extensions.Singleton;
using Features.Duel.Data.FightEnd;
using UnityEngine;

namespace Features.Leagues
{
    /// <summary>
    /// Контроллер лиги
    /// </summary>
    public sealed class LeagueController : MonoBehaviourSingleton<LeagueController>
    {
        /// <summary>
        /// Событие начала лиги
        /// </summary>
        public event Action LeagueStart;
        /// <summary>
        /// Событие начала новой дуэли лиги
        /// </summary>
        /// <typeparam name="int">Индекс новой дуэли</typeparam>
        public event Action<int> LeagueDuelStart;
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

        /// <summary>
        /// Индекс текущей дуэли
        /// </summary>
        public int CurrentDuelIndex => currentDuelIndex;
        /// <summary>
        /// Количество дуэдей в лиге
        /// </summary>
        public int TotalDuelsNumber => leagueDataContainer.Data.DuelContracts.Count;
        
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
            if (TotalDuelsNumber == 0) return;

            currentDuelIndex = 0;
            isLeagueActive = true;

            contractListContainer.Clear();
            leagueDataContainer.GenerateNewLeague();
            foreach (DuelContract contract in leagueDataContainer.Data.DuelContracts)
            {
                contractListContainer.Add(contract);
            }
            
            StartCurrentDuel();
            LeagueStart?.Invoke();
        }

        /// <summary>
        /// Завершить лигу принужденно
        /// </summary>
        public void EndLeague()
        {
            LeagueLoseEvent?.Invoke();
        }

        private void StartCurrentDuel()
        {
            if (leagueDataContainer.Data == null) return;
            if (leagueDataContainer.Data.DuelContracts == null) return;

            DuelContract activeContract = leagueDataContainer.Data.DuelContracts[currentDuelIndex];
            selectedContract.Select(activeContract.Id);
            
            DuelStartRequest request = new DuelStartRequest(activeContract);
            duelController.StartDuel(request);
            
            LeagueDuelStart?.Invoke(currentDuelIndex);
        }

        private void OnDuelEnded(DuelEndEvent handler)
        {
            if (!isLeagueActive) return;

            switch (handler.DuelEndResult.Winner.Value)
            {
                case FightWinner.Player:
                    currentDuelIndex++;
                    if (currentDuelIndex >= TotalDuelsNumber)
                    {
                        isLeagueActive = false;
                        LeagueWonEvent?.Invoke();
                    }
                    else
                    {
                        LeagueDuelWonEvent?.Invoke();
                        StartCurrentDuel();
                    }
                    return;
        
                case FightWinner.Opponent:
                    isLeagueActive = false;
                    LeagueLoseEvent?.Invoke();
                    return;
            }
        }
    }
}