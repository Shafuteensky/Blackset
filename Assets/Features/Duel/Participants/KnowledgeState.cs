using System.Collections.Generic;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние знаний об участнике (знания об этом участнике со стороны других)
    /// </summary>
    /// <remarks>
    /// Актуален и существует на протяжении дуэли, поэтому нет необходимости в сбросе
    /// </remarks>
    public struct KnowledgeState
    {
        /// <summary>
        /// Список открытых за дуэль дайсов (по идентификатору из сборки)
        /// </summary>
        public List<string> RevealedDices { get; }

        /// <summary>
        /// Статус раскрытия дайса (хотя бы раз использован за дуэль с момента сосздания сборки участника)
        /// </summary>
        /// <param name="dice">Идентификатор дайса</param>
        /// <returns></returns>
        public bool IsDiceRevealed(string dice)
        {
            bool isRevealed = RevealedDices.Contains(dice);
            return isRevealed;
        }
    }
}