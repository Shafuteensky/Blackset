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
        
        [Header("Идентификация"), Space]
        [SerializeField]
        protected string id;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(id))
                return;

            id = Guid.NewGuid().ToString(GUID_FORMAT);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}