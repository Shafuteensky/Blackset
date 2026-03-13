using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Визульная репрезентация дайса
    /// </summary>
    public class DiceViewRepresentation : MonoBehaviour
    {
        // TODO Создать родителя для показа прочих предметов
        // [SerializeField] private ItemClass itemClass;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        private Material material;
        private Mesh body;
        
        /// <summary>
        /// Иницализация
        /// </summary>
        /// <param name="diceItemContext">Данные дайса</param>
        public void Initialize(DiceItemContext diceItemContext)
        {
            DiceTypePrefabPair diceStyleData = diceItemContext.GetDice().Style.GetPrefabPair(diceItemContext.Type);
            material = diceStyleData.Material;
            body = diceStyleData.Body;

            meshFilter.sharedMesh = body;
            meshRenderer.material = material;
        }
    }
}