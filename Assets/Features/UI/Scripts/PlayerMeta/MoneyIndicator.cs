using Blackset.Player;
using Extensions.Generics;
using TMPro;
using UnityEngine;

namespace Blackset.UI.PlayerMeta
{
    /// <summary>
    /// Вывод софт-валюты игрока
    /// </summary>
    public sealed class MoneyIndicator : InitializableMonoBehaviour
    {
        [SerializeField]
        private PlayerDataFacade playerDataFacade;
        
        [SerializeField]
        private TMP_Text textMoney;

        private void OnEnable()
        {
            Initialize(playerDataFacade != null && textMoney != null);
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onMoneyChanged += ShowMoney;
            
            ShowMoney(playerDataFacade.MetaData.Data.Money, 0, playerDataFacade.MetaData.Data.Money);
        }
        
        private void OnDisable()
        {
            if (!IsInitialized) return;
            
            playerDataFacade.MetaData.onMoneyChanged -= ShowMoney;
        }

        private void ShowMoney(int money, int amount, int oldMoney) => textMoney.text = money.ToString();
    }
}