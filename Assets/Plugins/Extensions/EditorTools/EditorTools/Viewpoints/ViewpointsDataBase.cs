using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Extensions.EditorTools.Viewpoints
{
    /// <summary>
    /// Сохранение и загрузка закладок ViewpointsWindow
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ViewpointsDataBase), menuName = "Extensions/EditorTools/" + nameof(ViewpointsDataBase))]
    public sealed class ViewpointsDataBase : InMemoryDataBase<ViewpointsData> { }
}