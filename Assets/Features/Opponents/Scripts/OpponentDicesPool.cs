using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Opponents
{
    /// <summary>
    /// Пул дайсов определенного номинала соперника
    /// </summary>
    [Serializable]
    public struct OpponentDicesPool
    {
        public InventoryItemType diceNominal;
        public List<DiceData> dices;
    }
}