using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Загрузка InMemory БД при остановке приложения на паузу
    /// <remarks>
    /// Используется для прогрева БД до использования
    /// </remarks>
    /// </summary>
    public class InMemoryDataPauseSaver : MonoBehaviour
    {
        [SerializeField]
        protected List<InMemoryDataBaseObject> dataBases = new List<InMemoryDataBaseObject>();
        
        protected virtual void OnApplicationPause(bool pause)
        {
            if (!pause) return;

            foreach (InMemoryDataBaseObject dataBase in dataBases)
            {
                if (dataBase is InMemoryDataContainer<InMemoryDataItem> inMemoryDataBase)
                {
                    inMemoryDataBase.RequestSave();
                }
            }
        }
    }

}