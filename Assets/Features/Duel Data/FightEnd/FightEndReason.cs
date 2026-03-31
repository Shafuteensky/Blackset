namespace Features.Duel.Data.FightEnd
{
    /// <summary>
    /// Причина окончания боя (триггер)
    /// </summary>
    public enum FightEndReason
    {
        None, // Причины нет — битва не завершена
        PlayerWon, // Игроком выполнены условия для победы 
        OpponentWon, // ботом
        PlayerLost, // Игроком выполнены условия для проигрыша 
        OpponentLost, // ботом
        Draw, // Ничья
        Other // Иные причины
    }
}