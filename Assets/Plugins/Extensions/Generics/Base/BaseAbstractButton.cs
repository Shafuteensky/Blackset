using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Generics
{
    /// <summary>
    /// Базовая абстракция кнопки
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class BaseAbstractButton : MonoBehaviour
    {
        protected Button button;

        protected virtual void Awake() => button = GetComponent<Button>();
        
        /// <summary>
        /// Нажатие на кнопку (зажатие выполнено)
        /// </summary>
        public abstract void OnButtonClick();
    }
}