using System;
using System.Collections;
using Extensions.Coroutines;
using Extensions.ScriptableValues;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Generics
{
    /// <summary>
    /// Абстракция зажимаемой кнопки
    /// </summary>
    public abstract class AbstractHoldButton : BaseAbstractButton, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        protected const float DEFAULT_HOLD_DURATION = 1f;
        
        #region События
        
        /// <summary>
        /// Кнопка зажата до конца, выполнение действия кнопки
        /// </summary>
        public event Action onButtonClicked;
        /// <summary>
        /// Зажатие кнопки начато
        /// </summary>
        public event Action onHoldStarted;
        /// <summary>
        /// Измнение прогресса зажатия
        /// </summary>
        /// <typeparam name="float">Прогресс зажатия от 0 до 1</typeparam>
        public event Action<float> onHoldProgressChanged;
        /// <summary>
        /// Кнопка отжата до завершения (выполнения onButtonClicked)
        /// </summary>
        public event Action onHoldCanceled;
        /// <summary>
        /// Кнопка зажата до конца
        /// </summary>
        public event Action onHoldCompleted;

        #endregion
        
        [SerializeField]
        protected FloatValue holdDurationValue;
        [SerializeField]
        protected bool useUnscaledTime = true;

        protected CoroutineTask holdTask;

        protected bool isHolding;
        protected float holdDuration;
        protected float holdTime;

        protected virtual void Awake()
        {
            base.Awake();
            holdTask = new CoroutineTask(this);
        }

        protected virtual void OnEnable()
        {
            holdDuration = holdDurationValue == null ? DEFAULT_HOLD_DURATION : holdDurationValue.Value;
            if (holdDuration < 0f) holdDuration = 0f;
        }

        protected virtual void OnDisable()
        {
            if (isHolding) CancelHoldInternal(true);
            else holdTask?.Stop();
        }

        #region Pointer Events

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (button == null || !button.interactable) return;
            // if (eventData.button != PointerEventData.InputButton.Left) return; // TODO заменить на настраиваемую конфигурацию?

            StartHold();
        }

        public virtual void OnPointerUp(PointerEventData eventData) => TryCancelHold();

        public virtual void OnPointerExit(PointerEventData eventData) => TryCancelHold();
        
        #endregion

        #region Internal

        protected virtual void StartHold()
        {
            if (holdTask == null || holdTask.IsRunning) return;

            isHolding = true;
            holdTime = 0f;

            onHoldStarted?.Invoke();
            onHoldProgressChanged?.Invoke(0f);

            if (GetProgress() >= 1f)
            {
                CompleteHoldInternal();
                return;
            }
            
            holdTask.Start(HoldRoutine());
        }

        protected virtual IEnumerator HoldRoutine()
        {
            while (isHolding)
            {
                if (button == null || !button.interactable)
                {
                    CancelHoldInternal(true);
                    yield break;
                }

                float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                holdTime += dt;

                float progress = GetProgress();
                onHoldProgressChanged?.Invoke(progress);

                if (progress >= 1f)
                {
                    CompleteHoldInternal();
                    yield break;
                }

                yield return null;
            }
        }

        private void TryCancelHold()
        {
            if (!isHolding) return;
            CancelHoldInternal(false);
        }

        protected virtual void CancelHoldInternal(bool silent)
        {
            isHolding = false;
            holdTime = 0f;

            holdTask?.Stop();
            onHoldProgressChanged?.Invoke(0f);

            if (!silent) onHoldCanceled?.Invoke();
        }

        protected virtual void CompleteHoldInternal()
        {
            isHolding = false;
            holdTime = 0f;

            holdTask?.Stop();
            onHoldProgressChanged?.Invoke(1f);

            onHoldCompleted?.Invoke();

            OnButtonClick();
            onButtonClicked?.Invoke();
        }

        protected float GetProgress()
        {
            if (holdDuration <= 0f) return 1f;
            return Mathf.Clamp01(holdTime / holdDuration);
        }
        
        #endregion
    }
}
