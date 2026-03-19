using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Поставщик ввода игрока
    /// </summary>
    public abstract class InputHandlerProvider : MonoBehaviour
    {
        protected IDuelInputHandler inputHandler;
        
        private void Start()
        {
            inputHandler = InputRegistrar.Instance.InputHandler;
        }
    }
}