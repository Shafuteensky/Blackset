using Blackset.UI.HoverInfo;
using UnityEngine;

namespace Blackset.DuelContracts.HoverInfo
{
    /// <summary>
    /// Эмиттер запроса вывода информации контаркта дуэли по наведению курсора на объект
    /// </summary>
    [RequireComponent(typeof(ContractEntryUIElement))]
    public class ContractHoverInfoEmitter : BaseHoverInfoEmitter<ContractListContainer, DuelContract, ContractEntryUIElement> { }
}