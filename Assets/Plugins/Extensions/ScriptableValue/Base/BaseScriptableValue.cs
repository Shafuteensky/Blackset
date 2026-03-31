using Extensions.Identification;

namespace Extensions.ScriptableValues
{
    /// <summary>
    /// Базовая абстракция ScriptableValue без указания типа
    /// </summary>
    public abstract class BaseScriptableValue : IdentifiableObject
    {
        /// <summary>
        /// Сброс значения к дефолтному
        /// </summary>
        public abstract void ResetToDefault();
    }
}