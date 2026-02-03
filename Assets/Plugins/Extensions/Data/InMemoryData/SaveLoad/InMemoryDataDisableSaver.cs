using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Срхранение InMemory БД на OnDisable
    /// <remarks>
    /// Используется для сохранения данных БД с отключенным автосейвом
    /// </remarks>
    /// </summary>
    public class InMemoryDataDisableSaver : MonoBehaviour
    {
        [SerializeField]
        protected List<InMemoryDataBaseObject> dataBases = new List<InMemoryDataBaseObject>();

        protected virtual void OnDisable()
        {
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