using Extensions.Data.InMemoryData.SelectionContext;
using UnityEngine;

namespace Blackset.DuelContracts.HoverInfo
{
    /// <summary>
    /// Элемент фабрики содержимого хранилища данных контрактов дуэлей (визуальное представление)
    /// </summary>
    public class ContractEntryVisualElement : ContextIdHolder<ContractListContainer, DuelContract>
    {
        [SerializeField]
        private Light lightObject;
        
        private void OnEnable()
        {
            if (lightObject == null) return;
            EnableLight(false);
            ContractHoverInfoEmitter.onShowRequested += LightON;
            ContractHoverInfoEmitter.onHideRequested += LightOFF;
        }
        
        private void OnDisable()
        {
            if (lightObject == null) return;
            LightOFF();
            ContractHoverInfoEmitter.onShowRequested -= LightON;
            ContractHoverInfoEmitter.onHideRequested -= LightOFF;
        }

        private void LightON(ContractListContainer _, string contractId, Vector2 __)
        {
            if (contractId == EntryId) EnableLight(true);
        }
        private void LightOFF() => EnableLight(false);
        
        private void EnableLight(bool state)
        {
            if (lightObject == null) return;
            lightObject.enabled = state;
        }
    }
}