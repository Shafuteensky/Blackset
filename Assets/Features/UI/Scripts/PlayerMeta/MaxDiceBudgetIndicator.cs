using Blackset.Player;
using Extensions.Generics;
using TMPro;
using UnityEngine;

namespace Blackset.UI.PlayerMeta
{
    /// <summary>
    /// Вывод максимального бюджета сборки дайсов
    /// </summary>
    public sealed class MaxDiceBudgetIndicator : InitializableMonoBehaviour
    {
        [SerializeField]
        private TMP_Text textBudget;
        
        [Header("Данные игрока"), Space]
        [SerializeField]
        private PlayerDataFacade playerDataFacade;
        
        private void OnEnable()
        {
            Initialize(playerDataFacade != null && textBudget != null);
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onLvlChanged += ShowBudget;
            
            ShowBudget(0, 0);
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onLvlChanged -= ShowBudget;
        }
        
        private void ShowBudget(int _, int __)
        {
            PlayerMetaData meta = playerDataFacade.MetaData.Data;
            textBudget.text = meta.GetActualMaxDiceBudget().ToString();
        }
    }
}