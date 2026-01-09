using UnityEngine;

namespace Extensions.ID
{
    /// <summary>
    /// Универсальный идентификатор
    /// </summary>
    [CreateAssetMenu(menuName = "Extensions/" + nameof(ID))]
    public class ID : ScriptableObject
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id => id;
        
        [SerializeField]
        protected string id = string.Empty;
        
    }
}