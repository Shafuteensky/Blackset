using System;
using Unity.VisualScripting;
using UnityEditor;
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

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString(GUID_FORMAT);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
    }
}