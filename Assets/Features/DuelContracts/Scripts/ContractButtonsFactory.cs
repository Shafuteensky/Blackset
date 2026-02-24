using Extensions.Data.InMemoryData;
using Extensions.Data.InMemoryData.SelectionContext;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// UI фабрика кнопок доступных контрактов дуэлей
    /// </summary>
    public class ContractButtonsFactory : BaseInMemoryDataFactory<ContextIdHolder, DuelContract, ContractListContainer>
    {
        protected override void OnInstanceInitialization(ContextIdHolder instance, DuelContract item)
        {
            instance.Initialize(item.Id);
        }
    }
}
