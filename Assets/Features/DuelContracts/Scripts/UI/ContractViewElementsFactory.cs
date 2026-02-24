using Blackset.DuelContracts.HoverInfo;
using Extensions.Data.InMemoryData;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// UI фабрика визуальных представлений доступных контрактов дуэлей
    /// </summary>
    public class ContractViewElementsFactory : BaseInMemoryDataFactory<ContractEntryVisualElement, DuelContract, ContractListContainer>
    {
        protected override void OnInstanceInitialization(ContractEntryVisualElement instance, DuelContract item)
        {
            instance.Initialize(dataContainer, item.Id);
        }
    }
}
