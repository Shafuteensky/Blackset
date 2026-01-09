using UnityEngine;

namespace Extentions
{
    /// <summary>
    /// Абстрактный триггер
    /// </summary>
    public abstract class GenericTrigger<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected T component = default;
        
        protected abstract void OnTriggerEnterAction(T other);
        protected abstract void OnTriggerStayAction(T other);
        protected abstract void OnTriggerExitAction(T other);
        
        protected void OnTriggerEnter (Collider other)
        {
            component = other.GetComponent<T>();
            if (component) OnTriggerEnterAction(component);
        }

        protected void OnTriggerStay (Collider other)
        {
            component = other.GetComponent<T>();
            if (component) OnTriggerStayAction(component);
        }
    
        protected void OnTriggerExit (Collider other)
        {
            component = other.GetComponent<T>();
            if (component) OnTriggerExitAction(component);
        }
    }
}