using Blackset.Inventories;
using DG.Tweening;
using Extensions.Generics;
using UnityEngine;

namespace Features.DiceBudget
{
    /// <summary>
    /// Воспроизведение анимации при недостатке бюджета пула дайсов
    /// </summary>
    public class OnBudgetLackAnimation : InitializableMonoBehaviour
    {
        [Header("Анимация"), Space]
        [SerializeField]
        protected DOTweenAnimation tween;

        private void OnEnable()
        {
            Initialize(tween != null);
            if (!IsInitialized) return;
            DicePoolInventory.onBudgetCheckFailed += PlayAnimation;
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;
            DicePoolInventory.onBudgetCheckFailed -= PlayAnimation;
        }
        
        private void PlayAnimation(int _)
        {
            tween.DORestart();
        }
    }
}