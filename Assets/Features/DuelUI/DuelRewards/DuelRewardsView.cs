using Blackset.Data.Registries;
using Blackset.Duel.Requests;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Blackset.Player;
using DG.Tweening;
using Extensions.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вьюшка полученных за дуэль наград
    /// </summary>
    public sealed class DuelRewardsView : MonoBehaviour
    {
        private DuelController duelController;
        private EventHub eventHub;
        
        [Header("Текущие данные игрока"), Space]
        [SerializeField] Slider playerExperienceSlider;
        
        [Header("Полученная награда"), Space]
        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text experienceText;
        [SerializeField] Slider earnedExperienceSlider;

        private void Awake()
        {
            duelController = DuelController.Instance;
            eventHub = duelController.EventHub;
        }

        private void OnEnable() => eventHub.SubscribeReplay<PlayerRewardedEvent>(ShowRewards);

        private void OnDisable() => eventHub.Unsubscribe<PlayerRewardedEvent>(ShowRewards);

        private void ShowRewards(PlayerRewardedEvent handler)
        {
            DuelRewards rewards = handler.DuelRewards;
            
            // Полученные деньги и опыт
            if (moneyText != null) moneyText.text = rewards.CurrencyDelta.ToString();
            if (experienceText != null) experienceText.text = rewards.ExpDelta.ToString();
            
            // Полоска опыта
            if (playerExperienceSlider != null) playerExperienceSlider.value = 
                GameData.Instance.PlayerDataFacade.MetaData.Data.GetProgressToNextLevelWithOffset(rewards.ExpDelta); 
            if (earnedExperienceSlider != null)
            {
                PlayerMetaData playerMetaData = GameData.Instance.PlayerDataFacade.MetaData.Data;
                float target = playerMetaData.GetProgressToNextLevel();

                earnedExperienceSlider.value = playerExperienceSlider.value;
                earnedExperienceSlider.DOValue(target, 1.5f).SetEase(Ease.OutCubic);
            }
        }
    }
}