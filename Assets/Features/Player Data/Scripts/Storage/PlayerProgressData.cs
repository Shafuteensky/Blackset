using System;
using System.Collections.Generic;
using System.Linq;
using Blackset.Opponents;

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
        /// Поверженные соперники [идентификаторы_соперника, количество_побед_над_ним]
        /// </summary>
        public List<PlayedOpponentData> DefeatedOpponents { get; private set; }

        /// <summary>
        /// Увеличить счетчик сыгранных дуэлей
        /// </summary>
        public void DuelPlayed() => DuelsPlayed++;
        
        /// <summary>
        /// Зачесть соперника как поверженного
        /// </summary>
        public void OpponentDefeat(string opponentId)
        {
            for (int i = 0; i < DefeatedOpponents.Count; i++)
            {
                if (DefeatedOpponents[i].OpponentId == opponentId)
                {
                    PlayedOpponentData data = DefeatedOpponents[i];
                    data.VictoriesNumber++;
                    DefeatedOpponents[i] = data;
                    return;
                }
            }

            DefeatedOpponents.Add(new PlayedOpponentData
            {
                OpponentId = opponentId,
                VictoriesNumber = 1,
                LossesNumber = 0
            });
        }
        
        /// <summary>
        /// Зачесть соперника как победившего
        /// </summary>
        public void OpponentWon(string opponentId)
        {
            for (int i = 0; i < DefeatedOpponents.Count; i++)
            {
                if (DefeatedOpponents[i].OpponentId == opponentId)
                {
                    PlayedOpponentData data = DefeatedOpponents[i];
                    data.LossesNumber++;
                    DefeatedOpponents[i] = data;
                    return;
                }
            }

            DefeatedOpponents.Add(new PlayedOpponentData
            {
                OpponentId = opponentId,
                VictoriesNumber = 0,
                LossesNumber = 1
            });
        }
    }
}