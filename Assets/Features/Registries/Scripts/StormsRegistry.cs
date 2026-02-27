using Blackset.Storms;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр данных штормов дуэлей
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(StormsRegistry),
        menuName = "Blackset/Registries/" + nameof(StormsRegistry))]
    public class StormsRegistry : BaseDataRegistry<Storm> { }
}