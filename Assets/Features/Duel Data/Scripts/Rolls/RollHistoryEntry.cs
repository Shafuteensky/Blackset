namespace Blackset.Duel.Rolls
{
    /// <summary>
    /// Запись истории броска дайса в рамках боя
    /// </summary>
    public class RollHistoryEntry
    {
        /// <summary>
        /// Порядковый номер броска в бою
        /// </summary>
        public int ThrowIndex { get; }
        /// <summary>
        /// Идентификатор дайса из сборки
        /// </summary>
        public string DiceInstanceId { get; }
        /// <summary>
        /// Сырой результат броска
        /// </summary>
        /// <remarks>
        /// Задается единожды при ролле результата (броске дайса)
        /// </remarks>
        public int RawResult { get; }

        /// <summary>
        /// Финальный результат броска после эффектов
        /// </summary>
        /// <remarks>
        /// Обновляется при применении эффектов
        /// </remarks>
        public int FinalResult { get; set; }

        /// <summary>
        /// Создать запись истории броска
        /// </summary>
        /// <param name="throwIndex">Порядковый номер броска в бою</param>
        /// <param name="diceInstanceId">Идентификатор дайса из сборки</param>
        /// <param name="rawResult">Сырой результат броска</param>
        public RollHistoryEntry(int throwIndex, string diceInstanceId, int rawResult)
        {
            ThrowIndex = throwIndex;
            DiceInstanceId = diceInstanceId;
            RawResult = rawResult;
            FinalResult = rawResult;
        }
    }
}