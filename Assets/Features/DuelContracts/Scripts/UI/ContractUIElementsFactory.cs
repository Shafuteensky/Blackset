using Blackset.DuelContracts.HoverInfo;
using Extensions.Data.InMemoryData;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// UI фабрика кнопок доступных контрактов дуэлей
    /// </summary>
    public class ContractUIElementsFactory : BaseInMemoryDataFactory<ContractEntryUIElement, DuelContract, ContractListContainer>
    {
        protected override void OnInstanceInitialization(ContractEntryUIElement instance, DuelContract item)
        {
            instance.Initialize(dataContainer, item.Id);
        }
    }
}
