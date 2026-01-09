using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Загрузка InMemory БД на OnEnable
    /// <remarks>
    /// Используется для прогрева БД до использования
    /// </remarks>
    /// </summary>
    public class InMemoryDataBaseEnableLoader : MonoBehaviour
    {
        [SerializeField]
        protected List<InMemoryDataBaseObject> dataBases = new List<InMemoryDataBaseObject>();

        protected virtual void OnEnable()
        {
            foreach (InMemoryDataBaseObject dataBase in dataBases)
            {
                if (dataBase is InMemoryDataBase<InMemoryDataEntry> inMemoryDataBase)
                {
                    var _ = inMemoryDataBase.DataBase;
                }
            }
        }
    }

}