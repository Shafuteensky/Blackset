namespace Extensions.Coroutines
{
    using System;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Stateless-утилита для отложенного выполнения действия через корутину
    /// </summary>
    public static class CoroutineDelay
    {
        /// <summary>
        /// Запуск действия с задержкой перед исполнением
        /// </summary>
        /// <param name="owner">Хозяин корутины</param>
        /// <param name="delay">Задержка в секундах</param>
        /// <param name="action">Действие</param>
        public static void Run(MonoBehaviour owner, float delay, Action action)
        {
            if (owner == null || !owner.isActiveAndEnabled)
                return;

            owner.StartCoroutine(Routine(owner, delay, action));
        }

        private static IEnumerator Routine(MonoBehaviour owner, float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            if (owner == null || !owner.isActiveAndEnabled)
                yield break;

            action?.Invoke();
        }
    }
}