using Extensions.Reactive;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Модификаторы текущего броска участника
    /// </summary>
    public class CurrentRollModifiersState
    {
        /// <summary>
        /// Имеет преимущество на текущий бросок
        /// </summary>
        public ReactiveProperty<bool> HasAdvantage { get; private set; } = new(false);
        /// <summary>
        /// Имеет помеху на текущий бросок
        /// </summary>
        public ReactiveProperty<bool> HasDisadvantage { get; private set; } = new(false);

        /// <summary>
        /// Сбросить модификаторы текущего броска
        /// </summary>
        public void Reset()
        {
            HasAdvantage.Value = false;
            HasDisadvantage.Value = false;
        }

        /// <summary>
        /// Дать преимущество на текущий бросок
        /// </summary>
        public void SetAdvantage()
        {
            HasAdvantage.Value = true;
            HasDisadvantage.Value = false;
        }

        /// <summary>
        /// Дать помеху на текущий бросок
        /// </summary>
        public void SetDisadvantage()
        {
            HasDisadvantage.Value = true;
            HasAdvantage.Value = false;
        }

        public CurrentRollModifiersState Clone()
        {
            CurrentRollModifiersState clone = new();
            
            clone.HasAdvantage.Value = HasAdvantage.Value;
            clone.HasDisadvantage.Value = HasDisadvantage.Value;
            
            return clone;
        }
    }
}