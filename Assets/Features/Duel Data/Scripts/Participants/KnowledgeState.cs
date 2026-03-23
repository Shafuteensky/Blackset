using System.Collections.Generic;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние знаний об участнике (знания об этом участнике со стороны других)
    /// </summary>
    /// <remarks>
    /// Актуален и существует на протяжении дуэли, поэтому нет необходимости в сбросе
    /// </remarks>
    public sealed class KnowledgeState
    {
        /// <summary>
        /// Список открытых за дуэль дайсов (по идентификатору из сборки)
        /// </summary>
        public IReadOnlyCollection<string> RevealedDices => revealedDices;
        /// <summary>
        /// Список открытых за дуэль расходников (по идентификатору из сборки)
        /// </summary>
        public IReadOnlyCollection<string> RevealedConsumables => revealedConsumables;

        private readonly HashSet<string> revealedDices = new();
        private readonly HashSet<string> revealedConsumables = new();

        /// <summary>
        /// Раскрыть дайс
        /// </summary>
        /// <param name="dice">Идентификатор дайса</param>
        /// <returns>True, если дайс был раскрыт впервые</returns>
        public bool RevealDice(string dice)
        {
            if (string.IsNullOrEmpty(dice) || revealedDices.Contains(dice)) 
                return false;
            
            return revealedDices.Add(dice);
        }
        
        /// <summary>
        /// Раскрыть расходник
        /// </summary>
        /// <param name="dice">Идентификатор расходника</param>
        /// <returns>True, если расходник был раскрыт впервые</returns>
        public bool RevealConsumable(string consumable)
        {
            if (string.IsNullOrEmpty(consumable) || revealedDices.Contains(consumable)) 
                return false;
            
            return revealedConsumables.Add(consumable);
        }
        
        /// <summary>
        /// Создание копии
        /// </summary>
        /// <returns></returns>
        public KnowledgeState Clone()
        {
            KnowledgeState clone = new KnowledgeState();
            
            foreach (string dice in revealedDices)
                clone.revealedDices.Add(dice);
            foreach (string consumable in revealedConsumables)
                clone.revealedConsumables.Add(consumable);
            
            return clone;
        }
    }
}