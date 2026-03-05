using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Requests;
using Blackset.Duel.Rules;
using Blackset.Duel.Snapshots;
using Blackset.DuelContracts;
using Extensions.Generics;
using UnityEngine;

namespace Blackset.Duel.Sequence
{
    /// <summary>
    /// Контроллер дуэли
    /// </summary>
    public sealed class DuelController : InitializableMonoBehaviour
    {
        [Header("Входные данные"), Space]
        [SerializeField]
        private DuelModuleRegistry modules;
        [SerializeField]
        private DuelInputPresenter inputPresenter;
        [SerializeField]
        private SelectedContract selectedContract;
        
        private DuelRulesConfiguration rules;
        private DuelStateMachine stateMachine;
        private DuelContext context;
        private SnapshotCommiter commiter;

        private void Awake()
        {
            Initialize(modules != null && inputPresenter != null && selectedContract != null);
            if (!IsInitialized) return;

            InitializeSequence();
            
            DuelStartRequest request = new DuelStartRequest(selectedContract.GetSelectedData());
            StartDuel(request);
        }

        /// <summary>
        /// Начать дуэль
        /// </summary>
        /// <param name="request">Запрос начала дуэли</param>
        public void StartDuel(DuelStartRequest request)
        {
            
        }

        /// <summary>
        /// Завершить дуэль
        /// </summary>
        public void EndDuel()
        {
            
        }

        /// <summary>
        /// Публикация событий состояний машины
        /// </summary>
        /// <param name="evt"></param>
        /// <typeparam name="TEvent"></typeparam>
        //public void Publish<TEvent>(TEvent evt) where TEvent : Event;

        private void InitializeSequence()
        {
            StateRegistry<DuelContext> duelStateRegistry = new();
            stateMachine = new DuelStateMachine(duelStateRegistry);
            context = new DuelContext(selectedContract.GetSelectedData(), inputPresenter);
            commiter = new SnapshotCommiter(context);
        }
    }
}