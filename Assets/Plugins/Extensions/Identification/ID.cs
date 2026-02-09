using System;
using UnityEngine;

namespace Extensions.Identification
{
    /// <summary>
    /// Универсальный идентификатор
    /// </summary>
    [CreateAssetMenu(menuName = "Extensions/" + nameof(ID))]
    public class ID : ScriptableObject
    {
        protected const string GUID_FORMAT = "N";
        
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id => id;
        
        [SerializeField]
        protected string id = string.Empty;
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString(GUID_FORMAT);
            }
        }
    }
}