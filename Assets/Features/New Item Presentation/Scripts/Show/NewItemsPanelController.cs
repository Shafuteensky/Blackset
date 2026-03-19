using Extensions.Log;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер панели презентации новых полученных предметов
    /// </summary>
    /// <remarks>
    /// Активирует панель показа новых предметов, если презентационный инвентарь не пуст
    /// </remarks>
    public class NewItemsPanelController : AbstractNewItemsShowController
    {
        [Header("Панель показа новых предметов"), Space]
        [SerializeField] private GameObject newItemsPanel;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            newItemsPanel.SetActive(showOnEnable && !presenterInventory.IsEmpty);
        }

        protected override void OnNewItemsShowRequested() => OpenPresentationPanel();
        
        private void OpenPresentationPanel()
        {
            if (newItemsPanel == null)
            {
                ServiceDebug.LogError("Панель не определена, новые предметы не показаны");
                return;
            }
            newItemsPanel.SetActive(true);
        }
    }
}