using System;
using UnityEngine;

namespace Extensions.Identification
{
    /// <summary>
    /// Базовый идентифицруемый скриптовый объект
    /// </summary>
    public abstract class IdentifiableObject : ScriptableObject
    {
        protected const string GUID_FORMAT = "N";
        
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id => id;
        
        [SerializeField]
        protected string id = string.Empty;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString(GUID_FORMAT);
            }
        }
        protected void Awake() => id = name;
    }
}