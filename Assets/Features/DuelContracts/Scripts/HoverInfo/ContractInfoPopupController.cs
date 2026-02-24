using Blackset.UI.HoverInfo;
using TMPro;
using UnityEngine;

namespace Blackset.DuelContracts.HoverInfo
{
    /// <summary>
    /// Контроллер вывода информации о контракте дуэли по запросу
    /// </summary>
    public class ContractInfoPopupController : BaseInfoPopupController<ContractListContainer, DuelContract, ContractEntryUIElement>
    {
        [Header("Текст"), Space]
        [SerializeField]
        protected TMP_Text nameText;
        [SerializeField]
        protected TMP_Text descriptionText;

        protected override void OnShow(ContractListContainer contracts, string contractId, Vector2 position)
        {
            if (nameText != null) nameText.text = contracts.GetOpponentData(contractId).DataName;
            if (descriptionText != null) descriptionText.text = contracts.GetOpponentData(contractId).DataDescription;
        }
    }
}