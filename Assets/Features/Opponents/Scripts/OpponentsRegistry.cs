using System.Collections.Generic;
using Blackset.Data.Registries;
using Blackset.OpponentsRestrictions;
using UnityEngine;

namespace Blackset.Opponents
{
    /// <summary>
    /// Реестр данных о соперниках
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(OpponentsRegistry),
        menuName = "Blackset/Opponents/" + nameof(OpponentsRegistry))]
    public class OpponentsRegistry : BaseDataRegistry<OpponentData>
    {
        /// <summary>
        /// Получить список соперников по доступности
        /// </summary>
        /// <param name="availability">Требуемая доступность</param>
        public List<OpponentData> GetUnrestricted(OpponentAvailability availability = OpponentAvailability.None)
        {
            List<OpponentData> availableOpponents = new();

            foreach (OpponentData opponent in Data)
            {
                if (GetAvailability(opponent, availability)) 
                    availableOpponents.Add(opponent);
            }
            
            return availableOpponents;
        }

        private bool GetAvailability(OpponentData opponent, OpponentAvailability availability)
        {
            return (opponent.GetAvailability() & availability) != 0;
        }
    }
}