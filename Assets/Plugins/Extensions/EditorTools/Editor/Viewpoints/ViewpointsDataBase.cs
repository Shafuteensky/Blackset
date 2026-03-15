using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Extensions.EditorTools.Viewpoints
{
    /// <summary>
    /// Сохранение и загрузка вьюпортов ViewpointsWindow
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ViewpointsDataBaseBase), menuName = "Extensions/EditorTools/" + nameof(ViewpointsDataBaseBase))]
    public sealed class ViewpointsDataBaseBase : InMemoryDataContainer<ViewpointsData> { }
}