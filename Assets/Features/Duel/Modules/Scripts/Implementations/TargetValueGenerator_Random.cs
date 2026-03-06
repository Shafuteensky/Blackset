using System.Collections.Generic;
using System.Linq;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Duel.Requests;
using Blackset.Duel.Rules;
using Blackset.Duel.TargetValue;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Случайный генератор целевого значения
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(TargetValueGenerator_Random),
        menuName = "Blackset/Duel/Modules/" + nameof(TargetValueGenerator_Random))]
    public class TargetValueGenerator_Random : BaseDuelModule, ITargetValueGenerator
    {
        private const int DEFAULT_ROLLS_NUMBER = 6;
        
        public TargetValueContext Generate(TargetValueRequest request)
        {
            TargetValuePolicy activeTargetValuePolicy = request.DuelRules.TargetValuePolicy;
            TargetValueContext targetValue = new TargetValueContext();
            
            switch (activeTargetValuePolicy)
            {
                // Бросок стандартного набора дайсов
                case TargetValuePolicy.RandomSet:
                {
                    Dictionary<DiceType, int> setRolls = ThrowSet(request.Seed);
                    int newValue = GetValueFromSet(setRolls);
                    targetValue.SetTargetValue(newValue, false, setRolls);
                    break;
                }
                
                // Бросок стандартного набора дайсов (в количестве дайсов в сборке по правилам)
                case TargetValuePolicy.RandomSetByDicesInSet:
                {
                    int dicesToRoll = request.DuelRules.MaxThrowsPerFight;
                    Dictionary<DiceType, int> setRolls = ThrowSet(request.Seed, dicesToRoll);
                    int newValue = GetValueFromSet(setRolls);
                    targetValue.SetTargetValue(newValue, false, setRolls);
                    break;
                }
                
                // Фиксированное: 21
                case TargetValuePolicy.Blackjack:
                {
                    targetValue.SetTargetValue(21, true);
                    break;
                }

                // Необработанные случаи
                default:
                {
                    ServiceDebug.LogError(
                        $"Необработанный случай политики генерации целевого значения ({activeTargetValuePolicy})");
                    break;
                }
            }
            
            return targetValue;
        }

        #region Генерация значения по отдельным правилам

        private int GetValueFromSet(Dictionary<DiceType, int> rollsSet)
        {
            int result = 0;
            foreach (int rollResult in rollsSet.Values)
            {
                result += rollResult;
            }
            return result;
        }
        
        private Dictionary<DiceType, int> ThrowSet(int seed, int throwCount = DEFAULT_ROLLS_NUMBER)
        {
            System.Random random = new(seed);
            Dictionary<DiceType, int> rolls = new();
            List<DiceType> rollTypes = GameData.Instance.DiceTypes.Data.OfType<DiceType>()
                .Where(d => !d.IsRestricted).ToList();

            if (rollTypes.Count == 0)
                return rolls;

            for (int i = 0; i < throwCount; i++)
            {
                DiceType diceType = rollTypes[i % rollTypes.Count];
                rolls[diceType] = random.Next(1, diceType.SidesNumber + 1);
            }

            return rolls;
        }

        #endregion
    }
}