namespace Blackset.Duel.History
{
    /// <summary>
    /// Список исторических событий 
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    public interface IHistoryEvents<TEvent>
    {
        public void AddEntry(TEvent entry);
        public bool TryGetLastEntry(out TEvent entry);
    }
}