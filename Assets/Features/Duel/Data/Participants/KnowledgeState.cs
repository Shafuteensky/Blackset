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

        private readonly HashSet<string> revealedDices = new();

        /// <summary>
        /// Раскрыть дайс
        /// </summary>
        /// <param name="dice">Идентификатор дайса</param>
        /// <returns>True, если дайс был раскрыт впервые</returns>
        public bool RevealDice(string dice)
        {
            if (string.IsNullOrEmpty(dice))
                return false;

            return revealedDices.Add(dice);
        }

        /// <summary>
        /// Статус раскрытия дайса (хотя бы раз использован за дуэль с момента создания сборки участника)
        /// </summary>
        /// <param name="dice">Идентификатор дайса</param>
        /// <returns>True, если дайс раскрыт</returns>
        public bool IsDiceRevealed(string dice) => revealedDices.Contains(dice);
    }
}