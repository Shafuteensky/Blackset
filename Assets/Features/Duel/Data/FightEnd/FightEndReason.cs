namespace Features.Duel.Data.FightEnd
{
    /// <summary>
    /// Причина окончания боя (триггер)
    /// </summary>
    public enum FightEndReason
    {
        PlayerWon, // Игроком выполнены условия для победы 
        OpponentWon, // ботом
        PlayerLost, // Игроком выполнены условия для проигрыша 
        OpponentLost // ботом
    }
}