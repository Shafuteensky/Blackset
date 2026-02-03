using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Extensions.EditorTools.Bookmarks
{
    /// <summary>
    /// Сохранение и загрузка закладок ObjectBookmarksWindow
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BookmarksDataBaseBase), menuName = "Extensions/EditorTools/" + nameof(BookmarksDataBaseBase))]
    public class BookmarksDataBaseBase : InMemoryDataContainer<BookmarkData> { }
}