using System.Collections.Generic;

namespace Features.Effects
{
    /// <summary>
    /// Очередь применения отложенных эффектов
    /// </summary>
    public class DeferredEffectsQueue
    {
        /// <summary>
        /// Очередь эффектов участников дуэли <идентификатор_участника, очередь>
        /// </summary>
        public Dictionary<string, Queue<DeferredEffect>> EffectsQueues { get; }

        /// <summary>
        /// Создать очереди эффектов для списка участников дуэли
        /// </summary>
        /// <param name="participantsIds">Список идентификаторов участников</param>
        public DeferredEffectsQueue(List<string> participantsIds)
        {
            EffectsQueues = new Dictionary<string, Queue<DeferredEffect>>();
            
            foreach (string participantId in participantsIds)
            {
                EffectsQueues.Add(participantId, new Queue<DeferredEffect>());
            }
        }
        
        /// <summary>
        /// Добавить эффект в очередь участника
        /// </summary>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="deferredEffect">Эффект</param>
        public void Enqueue(string participantId, DeferredEffect deferredEffect)
        {
            EffectsQueues[participantId].Enqueue(deferredEffect);
        }

        /// <summary>
        /// Взять следующий эффект из очереди участника
        /// </summary>
        /// <param name="participantId">Идентификатор участника</param>
        public DeferredEffect Dequeue(string participantId)
        {
            DeferredEffect effect = EffectsQueues[participantId].Dequeue();
            return effect;
        }

        /// <summary>
        /// Очистить очередь эффектов
        /// </summary>
        /// <param name="participantId">Идентификатор участника</param>
        public void Clear(string participantId)
        {
            
        }
    }
}