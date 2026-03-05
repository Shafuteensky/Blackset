using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Requests;
using Blackset.Duel.Rules;
using Blackset.Duel.Snapshots;
using Blackset.DuelContracts;
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
            DuelStartRequest request = new DuelStartRequest(selectedContract.GetSelectedData());
            StartDuel(request);
        }

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
            InitializeSequence();
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
        
        private void InitializeSequence()
        {
            DuelStateRegistry<DuelContext> duelStateRegistry = InitializeStateRegistry();
            stateMachine = new DuelStateMachine(duelStateRegistry);

            ServiceGuard.NotNull(inputPresenter, nameof(inputPresenter));
            ServiceGuard.NotNull(selectedContract, nameof(selectedContract));
            context = new DuelContext(selectedContract.GetSelectedData(), inputPresenter);
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