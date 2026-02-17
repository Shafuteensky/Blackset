using Blackset.Player;
using Extensions.Generics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.UI.PlayerMeta
{
    /// <summary>
    /// Вывод опыта игрока
    /// </summary>
    public sealed class ExperienceIndicator : InitializableMonoBehaviour
    {
        [SerializeField]
        private PlayerDataFacade playerDataFacade;
        
        [Header("Опыт"), Space]
        [SerializeField]
        private TMP_Text textCurExp;
        [SerializeField]
        private TMP_Text textSumExp;
        [SerializeField]
        private Slider lvlSlider;

        [Header("Уровень"), Space]
        [SerializeField]
        private TMP_Text textLvl;
        
        private void OnEnable()
        {
            Initialize(playerDataFacade != null);
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onExpChanged += ShowExperience;
            playerDataFacade.MetaData.onLvlChanged += ShowLevel;
            
            PlayerMetaData meta = playerDataFacade.MetaData.Data;
            ShowExperience(meta.GetThisLevelExp(), 0, meta.GetThisLevelRequiredExp(), meta.GetProgressToNextLevel());
            ShowLevel(meta.GetPlayerLvl(), 0);
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onExpChanged -= ShowExperience;
            playerDataFacade.MetaData.onLvlChanged -= ShowLevel;
        }

        private void ShowExperience(int curExp, int addedExp, int reqExp, float progress)
        {
            if (textCurExp != null) 
                textCurExp.text = curExp.ToString();
            if (textSumExp != null) 
                textSumExp.text = reqExp.ToString();
            if (lvlSlider != null) 
                lvlSlider.value = progress;
        }

        private void ShowLevel(int curLvl, int prevLvl)
        {
            if (textLvl != null) 
                textLvl.text = curLvl.ToString();
        }
    }
}