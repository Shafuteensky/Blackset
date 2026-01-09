using System.Collections.Generic;
using UnityEngine;
using VHierarchy.Libs;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Срхранение InMemory БД на OnDisable
    /// <remarks>
    /// Используется для сохранения данных БД с отключенным автосейвом
    /// </remarks>
    /// </summary>
    public class InMemoryDataBaseDisableSaver : MonoBehaviour
    {
        [SerializeField]
        protected List<InMemoryDataBaseObject> dataBases = new List<InMemoryDataBaseObject>();

        protected virtual void OnDisable()
        {
            foreach (InMemoryDataBaseObject dataBase in dataBases)
            {
                if (dataBase is InMemoryDataBase<InMemoryDataEntry> inMemoryDataBase)
                {
                    dataBase.Save();
                }
            }
        }
    }

}