using Blackset.Inventories;
using Blackset.Player;
using Extensions.Generics;
using TMPro;
using UnityEngine;

namespace Blackset.UI.PlayerMeta
{
    /// <summary>
    /// Вывод использованного бюджета сборки дайсов
    /// </summary>
    public sealed class UsedDiceBudgetIndicator : InitializableMonoBehaviour
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

            foreach (Inventory dicePool in playerDataFacade.DicesPoolRows)
            {
                dicePool.onDataUpdated += ShowBudget;
            }
            
            ShowBudget();
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;

            foreach (Inventory dicePool in playerDataFacade.DicesPoolRows)
            {
                dicePool.onDataUpdated -= ShowBudget;
            }
        }

        private void ShowBudget()
        {
            PlayerMetaData meta = playerDataFacade.MetaData.Data;
            textBudget.text = meta.GetTotalUsedDiceBudget(playerDataFacade.DicesPoolRows).ToString();
        }
    }
}