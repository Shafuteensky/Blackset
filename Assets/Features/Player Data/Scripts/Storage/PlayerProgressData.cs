using System;

namespace Blackset.Player
{
    /// <summary>
    /// Структура данных прогресса игрока
    /// </summary>
    [Serializable]
    public class PlayerProgressData
    {
        /// <summary>
        /// Текущая стадия игры
        /// </summary>
        public int Stage { get; private set; }
        /// <summary>
        /// Количество сыгранных игр
        /// </summary>
        public int DuelsPlayed { get; private set; }

        /// <summary>
        /// Увеличить счетчик сыгранных дуэлей
        /// </summary>
        public void DuelPlayed() => DuelsPlayed++;
    }
}