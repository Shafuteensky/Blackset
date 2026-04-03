namespace Blackset.Player
{
    /// <summary>
    /// Данные об игре против соперника
    /// </summary>
    public struct PlayedOpponentData
    {
        /// <summary>
        /// Идентификатор соперника
        /// </summary>
        public string OpponentId;
        /// <summary>
        /// Количество побед соперника над игроком
        /// </summary>
        public int VictoriesNumber;
        /// <summary>
        /// Количество поражений соперника от игрока
        /// </summary>
        public int LossesNumber;
    }
}