using Extensions.Data.InMemoryData;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Контейнер мета-данных игрока
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerMetaDataContainer),
        menuName = "Blackset/Player/" + nameof(PlayerMetaDataContainer))]
    public class PlayerMetaDataContainer : InMemorySingleDataContainer<PlayerMetaData>
    {
        public void AddMoney(int amount)
        {
            if (Data == null || amount < 0)
            {
                ServiceDebug.LogError("Ошибка добавления валюты");
                return;
            }
            
            Data.AddMoney(amount);
            MarkDirty();
        }
    }
}