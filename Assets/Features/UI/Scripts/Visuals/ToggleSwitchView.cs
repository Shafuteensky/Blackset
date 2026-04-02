using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Extensions.Generics
{
    /// <summary>
    /// Визуальный контроллер мобильного свитча.
    /// Самостоятельно подписывается на <see cref="Toggle.onValueChanged"/>
    /// и не зависит от AbstractToggle или любой другой логики переключения.
    /// Для кастомизации анимации унаследуйся и переопредели
    /// <see cref="AnimateHandle"/> и <see cref="AnimateBackground"/>.
    /// </summary>
    public class ToggleSwitchView : MonoBehaviour
    {
        [Header("Управление"), Space]
        [SerializeField] protected Toggle toggle;
        
        [Header("Графика"), Space]
        [SerializeField] protected RectTransform handle;
        [SerializeField] protected Image background;

        [Header("Точки позиции (пустые RectTransform)"), Space]
        [SerializeField] protected RectTransform onPoint;
        [SerializeField] protected RectTransform offPoint;

        [Header("Цвета"), Space]
        [SerializeField] protected Color offColor = new Color(0.78f, 0.78f, 0.78f);
        [SerializeField] protected Color onColor  = new Color(0.20f, 0.78f, 0.35f);

        [Header("Анимация"), Space]
        [SerializeField] protected float duration = 0.3f;
        [SerializeField] protected Ease  easeType = Ease.OutBack;


        protected virtual void Awake()
        {
            if (toggle == null) return;
            
            toggle.targetGraphic = null;
            toggle.graphic = null;
            toggle.transition = Selectable.Transition.None;
        }

        protected virtual void OnEnable()
        {
            if (toggle == null) return;
            
            toggle.onValueChanged.AddListener(SetState);
            SetState(toggle.isOn, false);
        }

        protected virtual void OnDisable() => toggle?.onValueChanged.RemoveListener(SetState);

        /// <summary>
        /// Обновить визуальное состояние свитча.
        /// Вызывается автоматически при изменении Toggle,
        /// либо вручную для мгновенной установки без анимации.
        /// </summary>
        /// <param name="state">true — включён, false — выключен.</param>
        /// <param name="animate">Воспроизводить анимацию или применить мгновенно.</param>
        public void SetState(bool state, bool animate)
        {
            Vector2 targetPos   = state ? offPoint.anchoredPosition : onPoint.anchoredPosition;
            Color   targetColor = state ? onColor : offColor;

            if (animate)
            {
                AnimateHandle(targetPos);
                AnimateBackground(targetColor);
            }
            else
            {
                handle.anchoredPosition = targetPos;
                background.color = targetColor;
                handle.localScale = Vector3.one;
            }
        }
        
        private void SetState(bool state) => SetState(state, true);

        protected virtual void AnimateHandle(Vector2 targetPos)
        {
            handle.DOAnchorPos(targetPos, duration).SetEase(easeType);

            handle.DOScale(new Vector3(1.2f, 0.85f, 1f), duration * 0.3f)
                  .SetEase(Ease.OutQuad)
                  .OnComplete(() =>
                      handle.DOScale(Vector3.one, duration * 0.4f).SetEase(Ease.OutBack)
                  );
        }

        protected virtual void AnimateBackground(Color targetColor)
        {
            background.DOColor(targetColor, duration);
        }
    }
}