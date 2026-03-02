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
        /// Выбранный дайс (фактический на бросок)
        /// </summary>
        public string ChosenDice;
        
        /// <summary>
        /// Выбран ли расходник для использования
        /// </summary>
        public bool ConsumableChosen;
        /// <summary>
        /// Выбранный расходник (фактический на бросок)
        /// </summary>
        public string ChosenConsumable;
        /// <summary>
        /// Цель расходника
        /// </summary>
        public ConsumableTarget ConsumableTarget;
    }
}