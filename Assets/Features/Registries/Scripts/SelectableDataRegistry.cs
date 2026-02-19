using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр данных с выбранным элементом
    /// </summary>
    /// <typeparam name="TData">Тип данных</typeparam>
    // TODO Возможно стоит заменить наследование на раздельный класс RegistrySelectionContext
    public abstract class SelectableDataRegistry<T> : BaseDataRegistry<T> where T : BaseData
    {
        /// <summary>
        /// Идентификатор выбранных данных
        /// </summary>
        public string SelectedDataId => selectedDataId;

        [SerializeField]
        protected string selectedDataId = string.Empty;
    }
}