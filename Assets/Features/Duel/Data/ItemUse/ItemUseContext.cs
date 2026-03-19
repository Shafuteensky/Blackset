using System;

namespace Blackset.Duel
{
    /// <summary>
    /// Структура данных об использовании дайса/расходника
    /// </summary>
    public struct ItemUseContext : IEquatable<ItemUseContext>
    {
        /// <summary>
        /// Идентификатор использованного предмета (дайса/расходника)
        /// </summary>
        public string ItemId { get; private set; }
        /// <summary>
        /// Идентификатор участника-цели (на которого применен предмет)
        /// </summary>
        public string TargetParticipantId { get; private set; }
        /// <summary>
        /// Идентификатор дайса-цели (на который применен предмет)
        /// </summary>
        public string TargetDiceId { get; private set; }

        /// <summary>
        /// Новая структура данных об использовании дайса/расходника
        /// </summary>
        public ItemUseContext(string itemId, string targetParticipantId, string targetDiceId)
        {
            ItemId = itemId;
            TargetParticipantId = targetParticipantId;
            TargetDiceId = targetDiceId;
        }
        
        /// <summary>
        /// Пустые данные
        /// </summary>
        /// <returns></returns>
        public static ItemUseContext Empty => new ItemUseContext
        {
            ItemId = string.Empty,
            TargetParticipantId = string.Empty,
            TargetDiceId = string.Empty
        };
        
        #region Equality
            
        public bool Equals(ItemUseContext other)
        {
            bool isEqual = ItemId == other.ItemId && 
                           TargetParticipantId == other.TargetParticipantId && 
                           TargetDiceId == other.TargetDiceId;
            return isEqual;
        }

        public override bool Equals(object obj) => obj is ItemUseContext other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(ItemId, TargetParticipantId, TargetDiceId);
        
        #endregion    
    }
}