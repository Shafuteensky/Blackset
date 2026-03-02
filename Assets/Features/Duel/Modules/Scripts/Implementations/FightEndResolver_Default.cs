using Features.Duel.Data.FightEnd;
using Features.Duel.Requests;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер окончания битвы дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(FightEndResolver_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(FightEndResolver_Default))]
    public class FightEndResolver_Default : BaseDuelModule, IFightEndResolver
    {
        public FightEndResult Evaluate(FightEndRequest request)
        {
            FightEndResult result = new FightEndResult();
            
            //

            return result;
        }
    }
}