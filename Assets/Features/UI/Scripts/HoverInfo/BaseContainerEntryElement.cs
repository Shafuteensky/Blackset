using Extensions.Data.InMemoryData;
using Extensions.Generics;
using Extensions.Log;

namespace Blackset.UI.HoverInfo
{
    /// <summary>
    /// Абстракция элемента фабрики содержимого хранилища данных InMemoryDataContainer
    /// </summary>
    public class BaseContainerEntryElement<TContainer, TEntry> : InitializableMonoBehaviour
        where TContainer : InMemoryDataContainer<TEntry> 
        where TEntry : InMemoryDataEntry
    {
        /// <summary>
        /// Прилинкованный контейнер данных элемента
        /// </summary>
        public TContainer DataContainer => dataContainer;
        /// <summary>
        /// Идентификатор прилинкованной записи контейнера
        /// </summary>
        public string ItemCellId => itemCellId;
        
        protected TContainer dataContainer;
        protected string itemCellId;

        /// <summary>
        /// Инициализация элемента
        /// </summary>
        /// <param name="newItemCellId">Идентификатор хранимых данных</param>
        public virtual void InitializeElement(TContainer newContainer, string newItemCellId = null)
        {
            if (newContainer == null)
            {
                ServiceDebug.LogError("Ссылка на контейнер данных отсутствует, инициализация прервана");
                return;
            }
            
            dataContainer = newContainer;
            itemCellId = newItemCellId;
        }
    }
}