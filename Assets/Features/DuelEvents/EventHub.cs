using System;
using System.Collections.Generic;

namespace Blackset.DuelEvents
{
    /// <summary>
    /// Локальный хаб событий
    /// </summary>
    public sealed class EventHub
    {
        private readonly Dictionary<Type, Delegate> handlers = new();

        /// <summary>
        /// Подписаться на событие
        /// </summary>
        /// <param name="handler">Исполняемое действие</param>
        /// <typeparam name="TEvent">Тип события</typeparam>
        /// <exception cref="ArgumentNullException">Действие не назначено</exception>
        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            Type eventType = typeof(TEvent);

            if (handlers.TryGetValue(eventType, out Delegate existing))
            {
                // Проверить что такого обработчика ещё нет
                foreach (Delegate d in existing.GetInvocationList())
                    if (d.Equals(handler)) return;

                handlers[eventType] = Delegate.Combine(existing, handler);
                return;
            }

            handlers[eventType] = handler;
        }

        /// <summary>
        /// Отписаться от события
        /// </summary>
        /// <param name="handler">Исполняемое действие</param>
        /// <typeparam name="TEvent">Тип события</typeparam>
        /// <exception cref="ArgumentNullException">Действие не указано</exception>
        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            Type eventType = typeof(TEvent);

            if (!handlers.TryGetValue(eventType, out Delegate existing)) return;

            Delegate updated = Delegate.Remove(existing, handler);

            if (updated == null)
            {
                handlers.Remove(eventType);
                return;
            }

            handlers[eventType] = updated;
        }
        
        /// <summary>
        /// Опубликовать событие
        /// </summary>
        /// <param name="evt">Событие</param>
        /// <typeparam name="TEvent">Тип события</typeparam>
        /// <exception cref="InvalidOperationException">Колбэк невалиден</exception>
        public void Publish<TEvent>(TEvent evt)
        {
            Type eventType = typeof(TEvent);
            if (!handlers.TryGetValue(eventType, out Delegate existing)) return;

            Action<TEvent> callback = existing as Action<TEvent>;
            if (callback == null)
                throw new InvalidOperationException($"Невалидный коллбек (несовпадение типов) для события '{eventType.Name}'");

            // Снимаем копию на случай если обработчик изнутри вызовет Unsubscribe
            Action<TEvent> snapshot = (Action<TEvent>)callback.Clone();
            snapshot.Invoke(evt);
        }

        /// <summary>
        /// Очистить все подписки
        /// </summary>
        public void Clear() => handlers.Clear();
        
        /// <summary>
        /// Есть ли подписчики
        /// </summary>
        /// <typeparam name="TEvent">Тип события</typeparam>
        /// <returns>true если есть, иначе false</returns>
        public bool HasSubscribers<TEvent>() => handlers.ContainsKey(typeof(TEvent));
    }
}