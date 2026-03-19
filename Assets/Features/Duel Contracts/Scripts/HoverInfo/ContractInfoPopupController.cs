using Blackset.Data;
using Blackset.Inventories.Cells;
using Blackset.UI.HoverInfo;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("dicePanel")] 
        [SerializeField] protected GameObject itemPanel;
        [FormerlySerializedAs("diceImage")] 
        [SerializeField] protected Image itemImage;
        [FormerlySerializedAs("diceText")]  
        [SerializeField] protected TMP_Text itemText;
        
        protected override void OnShow(ContractListContainer contracts, string contractId, Vector2 position)
        {
            Reset();
            if (nameText != null) nameText.text = contracts.GetOpponentData(contractId).DataName;
            if (descriptionText != null) descriptionText.text = contracts.GetOpponentData(contractId).DataDescription;
            if (moneyText != null) moneyText.text = contracts.GetById(contractId).MoneyReward.ToString();
            if (itemPanel != null) itemPanel.SetActive(true);
            if (itemImage != null) itemImage.sprite = contracts.GetById(contractId).ItemReward.GetTypeData().Icon;
            if (itemText != null) 
            {
                ItemContext item = contracts.GetById(contractId).ItemReward;
                itemText.text = $"{item.GetItemData().DataName}";
            }
        }

        private void Reset()
        {
            nameText.text = "";
            descriptionText.text = "";
            moneyText.text = "";
            itemPanel.SetActive(false);
            itemImage.sprite = null;
            itemText.text = "";
        }
    }
}