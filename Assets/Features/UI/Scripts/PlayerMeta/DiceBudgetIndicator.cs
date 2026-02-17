using Blackset.Player;
using Extensions.Generics;
using TMPro;
using UnityEngine;

namespace Blackset.UI.PlayerMeta
{
    /// <summary>
    /// Вывод бюджета сборки дайсов
    /// </summary>
    public sealed class DiceBudgetIndicator : InitializableMonoBehaviour
    {
        [SerializeField]
        private PlayerDataFacade playerDataFacade;
        
        [SerializeField]
        private TMP_Text textBudget;
        
        private void OnEnable()
        {
            Initialize(playerDataFacade != null && textBudget != null);
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onLvlChanged += ShowBudget;
            
            PlayerMetaData meta = playerDataFacade.MetaData.Data;
            ShowBudget(meta.GetPlayerLvl(), 0);
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onLvlChanged -= ShowBudget;
        }

        private void ShowBudget(int curLvl, int prevLvl)
        {
            PlayerMetaData meta = playerDataFacade.MetaData.Data;
            textBudget.text = meta.GetActualMaxDiceBudget().ToString();
        }
    }
}