using Extensions.Data.InMemoryData;
using Extensions.Data.InMemoryData.SelectionContext;

namespace Blackset.Opponents
{
    /// <summary>
    /// UI фабрика кнопок доступных контрактов дуэлей
    /// </summary>
    public class ContractButtonsFactory : BaseInMemoryDataFactory<ContextIdHolder, OpponentContract, ContractListContainer>
    {
        protected override void OnInstanceInitialization(ContextIdHolder instance, OpponentContract item)
        {
            instance.Initialize(item.Id);
        }
    }
}
