using Blackset.Data;
using Blackset.UI.HoverInfo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.DuelContracts.HoverInfo
{
    /// <summary>
    /// Контроллер вывода информации о контракте дуэли по запросу
    /// </summary>
    public class ContractInfoPopupController : BaseInfoPopupController<ContractListContainer, DuelContract, ContractEntryUIElement>
    {
        [Header("Данные о сопернике"), Space]
        [SerializeField]
        protected TMP_Text nameText;
        [SerializeField]
        protected TMP_Text descriptionText;
        
        [Header("Награда"), Space]
        [SerializeField]
        protected TMP_Text moneyText;
        [SerializeField]
        protected GameObject dicePanel;
        [SerializeField]
        protected Image diceImage;
        [SerializeField]
        protected TMP_Text diceText;
        
        protected override void OnShow(ContractListContainer contracts, string contractId, Vector2 position)
        {
            Reset();
            if (nameText != null) nameText.text = contracts.GetOpponentData(contractId).DataName;
            if (descriptionText != null) descriptionText.text = contracts.GetOpponentData(contractId).DataDescription;
            if (moneyText != null) moneyText.text = contracts.GetById(contractId).MoneyReward.ToString();
            if (dicePanel != null) dicePanel.SetActive(true);
            if (diceImage != null) diceImage.sprite = contracts.GetById(contractId).DiceReward.Type.Icon;
            if (diceText != null) 
            {
                DiceItemContext dice = contracts.GetById(contractId).DiceReward;
                diceText.text = $"{dice.Dice.DataName}";
            }
        }

        private void Reset()
        {
            nameText.text = "";
            descriptionText.text = "";
            moneyText.text = "";
            dicePanel.SetActive(false);
            diceImage.sprite = null;
            diceText.text = "";
        }
    }
}