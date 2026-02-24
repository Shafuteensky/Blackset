using Blackset.DuelContracts.HoverInfo;
using Blackset.Opponents;
using TMPro;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Текст заголовка (имени) кнопки выбора контракта
    /// </summary>
    [RequireComponent(typeof(ContractEntryUIElement))]
    public sealed class ContractContextHeader : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text headerText;
        [SerializeField]
        private ContractListContainer contractListContainer;

        private ContractEntryUIElement contextHolder;

        private void Awake()
        {
            contextHolder = GetComponent<ContractEntryUIElement>();
        }

        private void OnEnable()
        {
            if (!contextHolder.IsInitialized) contextHolder.onInitialized += SetHeader;
            else SetHeader();
        }

        private void OnDisable()
        {
            contextHolder.onInitialized -= SetHeader;
        }

        private void SetHeader()
        {
            OpponentData opponentData = contractListContainer.GetOpponentData(contextHolder.EntryId);
            headerText.text = opponentData.DataName;
        }
    }
}