using System;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Data.Configurations
{
    /// <summary>
    /// Правила заполнения значений граней дайса
    /// </summary>
    [Serializable]
    public class SideNumberRule
    {
        protected const int MIN_DICE_SIDE_VALUE = 1;
        
        [SerializeField]
        protected SideNumbersRules rule = SideNumbersRules.Default;
        
        public SideNumberRule(SideNumbersRules rule)
        {
           this.rule = rule;
        }
        
        /// <summary>
        /// Получить измененные по правилам грани
        /// </summary>
        /// <param name="sides">Грани дайса</param>
        /// <returns>Измененные грани дайса</returns>
        public int[] GetMutatedSides(int[] sides)
        { 
            if (sides == null || sides.Length <= 0)
            {
                ServiceDebug.LogError("Получены невалидные данные о значениях сторон дайсов");
                return Array.Empty<int>();
            }
            
            int[] numbers = new int[sides.Length];

            switch (rule)
            {
                // Все грани максимального значения
                case SideNumbersRules.AllMax:
                {
                    for (int i = 0; i < sides.Length; i++)
                    {
                        numbers[i] = sides.Length;
                    }
                    
                    break;
                }
                // Все грани минимального значения
                case SideNumbersRules.AllMin:
                {
                    for (int i = 0; i < sides.Length; i++)
                    {
                        numbers[i] = MIN_DICE_SIDE_VALUE;
                    }
                    
                    break;
                }
                // Базовые: от 1 до максимума граней дайса
                default:
                {
                    for (int i = 1; i <= sides.Length; i++)
                    {
                        numbers[i] = i;
                    }
                    
                    break;
                }
            }
            
            
            return numbers;
        }
    }
}