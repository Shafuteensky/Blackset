using System;
using Blackset.Data.Registries;
using Blackset.Opponents;
using TMPro;
using UnityEngine;
using Extensions.Data.InMemoryData.SelectionContext;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Текст заголовка (имени) кнопки выбора контракта
    /// </summary>
    [RequireComponent(typeof(ContextIdHolder))]
    public sealed class ContractContextHeader : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text headerText;
        [SerializeField]
        private ContractListContainer contractListContainer;

        private ContextIdHolder contextIdHolder;

        private void Awake()
        {
            contextIdHolder = GetComponent<ContextIdHolder>();
        }

        private void OnEnable()
        {
            if (!contextIdHolder.IsInitialized) contextIdHolder.onInitialized += SetHeader;
            else SetHeader();
        }

        private void OnDisable()
        {
            contextIdHolder.onInitialized -= SetHeader;
        }

        private void SetHeader()
        {
            OpponentData opponentData = contractListContainer.GetOpponentData(contextIdHolder.Id);
            headerText.text = opponentData.DataName;
        }
    }
}