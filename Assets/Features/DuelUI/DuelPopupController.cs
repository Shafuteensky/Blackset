using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Coroutines;
using Features.Duel.Data.FightEnd;
using TMPro;
using UnityEngine;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Контроллер попап-уведомлений дуэли
    /// </summary>
    public sealed class DuelPopupController : MonoBehaviour
    {
        [Header("параметры показа"), Space]
        [Range(0.5f, 4f)]
        [SerializeField] private float showDuration = 2f;
        
        [Header("Старт дуэли"), Space]
        [SerializeField] private GameObject targetValuePanel;
        [SerializeField] private TMP_Text targetValueText;
        
        [Header("Начало битвы"), Space]
        [SerializeField] private GameObject newFightPanel;
        [SerializeField] private TMP_Text fightNumberText;
        
        [Header("Завершение битвы"), Space]
        [SerializeField] private GameObject fightWonPanel;
        [SerializeField] private GameObject fightLostPanel;
        [SerializeField] private GameObject fightDrawPanel;
        
        private DuelController duelController;
        
        private void OnEnable()
        {
            duelController = DuelController.Instance;
            
            duelController.EventHub.Subscribe<TargetValueSetEvent>(ShowTargetValuePanel);
            duelController.EventHub.Subscribe<BattleStartEvent>(ShowNewFightPanel);
            duelController.EventHub.Subscribe<BattleEndEvent>(ShowFightResultPanel);
        }

        private void ShowTargetValuePanel(TargetValueSetEvent handler)
        {
            if (targetValuePanel == null || targetValueText == null) return;
            
            targetValueText.text = handler.TargetValue.ToString();
            targetValuePanel.SetActive(true);
            CoroutineDelay.Run(this, showDuration, () => targetValuePanel.SetActive(false));
        }
        
        private void ShowNewFightPanel(BattleStartEvent handler)
        {
            if (newFightPanel == null || fightNumberText == null) return;

            fightNumberText.text = handler.DuelContext.Progress.FightNumber.Value.ToString();
            newFightPanel.SetActive(true);
            CoroutineDelay.Run(this, showDuration, () => newFightPanel.SetActive(false));
        }
        
        private void ShowFightResultPanel(BattleEndEvent handler)
        {
            if (fightWonPanel == null || fightLostPanel == null || fightDrawPanel == null ||
                handler.FightEndResult.IsFightEnded == false) return;

            switch (handler.FightEndResult.Winner)
            {
                case FightWinner.Player:
                {
                    fightWonPanel.SetActive(true);
                    CoroutineDelay.Run(this, showDuration, () => fightWonPanel.SetActive(false));
                    break;
                }
                case FightWinner.Opponent:
                {
                    fightLostPanel.SetActive(true);
                    CoroutineDelay.Run(this, showDuration, () => fightLostPanel.SetActive(false));
                    break;
                }
                case FightWinner.None:
                {
                    fightDrawPanel.SetActive(true);
                    CoroutineDelay.Run(this, showDuration, () => fightDrawPanel.SetActive(false));
                    break;
                }
            }
        }
    }
}