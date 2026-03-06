using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Requests;
using Blackset.Duel.Rules;
using Blackset.Duel.Snapshots;
using Blackset.DuelContracts;
using Blackset.Storms;
using Extensions.Log;
using Features.Duel.Sequence.States;
using UnityEngine;

namespace Blackset.Duel.Sequence
{
    /// <summary>
    /// Контроллер дуэли
    /// </summary>
    public sealed class DuelController : MonoBehaviour
    {
        [Header("Модули"), Space]
        [SerializeField]
        private DuelModuleRegistry modules;
        [SerializeField]
        private DuelInputPresenter inputPresenter;
        
        [Header("Входные данные"), Space]
        [SerializeField]
        private SelectedContract selectedContract;
        [SerializeField]
        private ActiveStorm activeStorm;
        
        private DuelRulesConfiguration rules;
        private DuelStateMachine stateMachine;
        private DuelContext context;
        private SnapshotCommiter commiter;

        private void Awake()
        {
            ServiceGuard.NotNull(modules, nameof(modules));
            ServiceGuard.NotNull(selectedContract, nameof(selectedContract));
            
            DuelStartRequest request = new DuelStartRequest(selectedContract.GetSelectedData());
            StartDuel(request);
        }

        private void Update()
        {
            stateMachine?.Tick(context);
        }

        // TODO Публикация событий состояний машины
        /// <summary>
        /// Публикация событий состояний машины
        /// </summary>
        /// <param name="evt"></param>
        /// <typeparam name="TEvent"></typeparam>
        //public void Publish<TEvent>(TEvent evt) where TEvent : Event;

        #region Управление состоянием дуэли
        
        /// <summary>
        /// Начать дуэль
        /// </summary>
        /// <param name="request">Запрос начала дуэли</param>
        public void StartDuel(DuelStartRequest request)
        {
            if (!TryInitializeContext(out context)) return;
            InitializeSequence(context);
            stateMachine.Start<DuelInitState>(context);
        }

        /// <summary>
        /// Завершить дуэль
        /// </summary>
        public void EndDuel()
        {
            stateMachine.Stop(context);
        }
        
        #endregion

        #region Инициализация

        private bool TryInitializeContext(out DuelContext duelContext)
        {
            ServiceGuard.NotNull(selectedContract, nameof(selectedContract));
            ServiceGuard.NotNull(activeStorm, nameof(activeStorm));
            duelContext = null;
            
            DuelContract activeContract = selectedContract.GetSelectedData();
            if (activeContract == null)
            {
                ServiceDebug.LogError("Активный контракт невалиден, дуэль не начата");
                return false;
            }

            duelContext = new(selectedContract.GetSelectedData(), activeStorm);
            return true;
        }
        
        private void InitializeSequence(DuelContext context)
        {
            DuelStateRegistry<DuelContext> duelStateRegistry = InitializeStateRegistry();
            stateMachine = new DuelStateMachine(duelStateRegistry);

            commiter = new SnapshotCommiter(context);
        }

        private DuelStateRegistry<DuelContext> InitializeStateRegistry()
        {
            DuelStateRegistry<DuelContext> duelStateRegistry = new();
            ServiceGuard.NotNull(modules, nameof(modules));
            duelStateRegistry.InitializeModules(modules);
            
            duelStateRegistry.Add(new DuelInitState());
            
            duelStateRegistry.Add(new BuildPreparationState());
            duelStateRegistry.Add(new BuildResolveState());
            
            duelStateRegistry.Add(new TargetValueSetupState());
            duelStateRegistry.Add(new BattleStartState());
            
            // ServiceGuard.NotNull(inputPresenter, nameof(inputPresenter));
            duelStateRegistry.Add(new RollPlanningState());
            duelStateRegistry.Add(new RollResolveState());
            
            duelStateRegistry.Add(new ScoreCommitState());
            duelStateRegistry.Add(new BattleCheckState());
            
            duelStateRegistry.Add(new DuelCheckState());
            duelStateRegistry.Add(new RewardResolveState());
            duelStateRegistry.Add(new DuelEndState());

            return duelStateRegistry;
        }
        
        #endregion
    }
}