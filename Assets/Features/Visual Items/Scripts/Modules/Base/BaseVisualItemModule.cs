using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Базовый класс модуля визуального предмета
    /// </summary>
    public abstract class BaseVisualItemModule : MonoBehaviour
    {
        public abstract void Initialize(VisualItemContext context);
    }
}