using DG.Tweening;
using UnityEngine;

namespace Blackset.CameraEffects
{
    /// <summary>
    /// Постоянное покачивание объекта через DOTween
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class SimpleSwayTween : MonoBehaviour
    {
        [SerializeField] private Vector3 positionOffset = new Vector3(0.03f, 0.02f, 0f);
        [SerializeField] private Vector3 rotationOffset = new Vector3(0.1f, 0.3f, 0f);
        [SerializeField] private float duration = 3f;
        [SerializeField] private Ease ease = Ease.InOutSine;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private bool useUnscaledTime;
        
        private Transform target;

        private Vector3 baseLocalPosition;
        private Vector3 baseLocalEulerAngles;
        private Sequence swaySequence;

        private void Awake()
        {
            target = transform;
            CacheBaseState();
        }

        private void OnEnable()
        {
            if (playOnEnable) Play();
        }

        private void OnDisable() => Stop();

        /// <summary>
        /// Запустить покачивание
        /// </summary>
        public void Play()
        {
            if (target == null) target = transform;

            CacheBaseState();

            if (swaySequence != null && swaySequence.IsActive()) swaySequence.Kill();

            swaySequence = DOTween.Sequence();
            swaySequence.SetUpdate(useUnscaledTime);

            swaySequence.Append(target.DOLocalMove(baseLocalPosition + positionOffset, duration).SetEase(ease));
            swaySequence.Join(target.DOLocalRotate(baseLocalEulerAngles + rotationOffset, duration).SetEase(ease));

            swaySequence.Append(target.DOLocalMove(baseLocalPosition - positionOffset, duration).SetEase(ease));
            swaySequence.Join(target.DOLocalRotate(baseLocalEulerAngles - rotationOffset, duration).SetEase(ease));

            swaySequence.SetLoops(-1, LoopType.Yoyo);
        }

        /// <summary>
        /// Остановить покачивание и вернуть исходное состояние
        /// </summary>
        public void Stop()
        {
            if (swaySequence != null && swaySequence.IsActive()) swaySequence.Kill();

            swaySequence = null;

            if (target == null) return;

            target.localPosition = baseLocalPosition;
            target.localRotation = Quaternion.Euler(baseLocalEulerAngles);
        }

        private void CacheBaseState()
        {
            baseLocalPosition = target.localPosition;
            baseLocalEulerAngles = target.localEulerAngles;
        }
    }
}