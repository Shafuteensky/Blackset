using Blackset.Data;

namespace Blackset.Duel.TurnIntents
{
    /// <summary>
    /// Намерения участника на ход
    /// </summary>
    public struct TurnIntent
    {
        /// <summary>
        /// Спасовал ли
        /// </summary>
        public bool IsPass;
        
        /// <summary>
        /// Объявленный дайс
        /// </summary>
        public DiceItemContext DeclaredDice;
        /// <summary>
        /// Выбранный дайс (фактический на бросок)
        /// </summary>
        public DiceItemContext ChosenDice;
        
        /// <summary>
        /// Использован ли расходник
        /// </summary>
        public bool ConsumableUsed;
        /// <summary>
        /// Выбранный расходник (фактический на бросок)
        /// </summary>
        public ConsumableItemContext ChosenConsumable;
        /// <summary>
        /// Цель расходника
        /// </summary>
        public ConsumableTarget ConsumableTarget;
    }
}