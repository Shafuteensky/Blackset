using Extensions.Log;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Визульная репрезентация расходника
    /// </summary>
    public class ConsumableViewRepresentation : MonoBehaviour
    {
        // TODO Создать родителя для показа прочих предметов
        // [SerializeField] private ItemClass itemClass;
        [SerializeField] private Transform consumableModelPosition;

        private Material material;
        private Mesh body;
        
        /// <summary>
        /// Иницализация
        /// </summary>
        /// <param name="consumableItemContext">Данные расходника</param>
        public void Initialize(ConsumableItemContext consumableItemContext)
        {
            ConsumableData consumableData = consumableItemContext.GetConsumable();

            if (consumableData == null)
            {
                ServiceDebug.LogError("Ошибка получения данных расходника");
                return;
            }
            
            GameObject consumablePrefab = consumableData.ConsumablePrefab;
            GameObject consumableView = Instantiate(consumablePrefab, consumableModelPosition);
            consumableView.transform.localPosition = Vector3.zero;
        }
    }
}