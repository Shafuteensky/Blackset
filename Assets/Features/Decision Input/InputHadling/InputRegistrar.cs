using Blackset.Duel.Modules;
using Extensions.Log;
using Extensions.Singleton;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Регистратор хендлера ввода игрока
    /// </summary>
    public class InputRegistrar : MonoBehaviourSingleton<InputRegistrar>
    {
        public IDuelInputHandler InputHandler;
        
        [SerializeField] private DuelModuleRegistry duelModuleRegistry;
        protected override void Awake()
        {
            base.Awake();
    
            IPlayerDecisionSource playerDecisionSource = duelModuleRegistry.Get<IPlayerDecisionSource>();
            InputHandler = playerDecisionSource as IDuelInputHandler;
    
            if (InputHandler == null)
                ServiceDebug.LogError($"{nameof(IDuelInputHandler)} не найден, хендлер ввода не инициализирован");
        }
    }
}