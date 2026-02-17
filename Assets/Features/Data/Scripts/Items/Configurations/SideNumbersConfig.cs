using System;
using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Data.Configurations
{
    [CreateAssetMenu(
        menuName = "Blackset/Items/Configurations/" + nameof(SideNumbersConfig),
        fileName = nameof(SideNumbersConfig))]
    public sealed class SideNumbersConfig : BaseData
    {
        private const int DEFAULT_DICE_SIDE_VALUE = 1;
        
        [Header("Правила заполнения граней"), Space]
        [SerializeField]
        private List<SideNumberRule> sideNumbersRules = new();

        protected override void OnValidate()
        {
            base.OnValidate();

            // В первую очередь заполнение по базовым правилам — от 1 до максимума граней дайса
            if (sideNumbersRules.Count <= 0)
            {
                sideNumbersRules.Add(new SideNumberRule(SideNumbersRules.Default));
            }
        }

        /// <summary>
        /// Получить финальные значения граней
        /// </summary>
        /// <param name="type">Тип дайса</param>
        /// <returns></returns>
        public int[] GetSideNumbers(DiceType type)
        {
            if (type == null)
            {
                ServiceDebug.LogError("Тип дайса не задан");
                return Array.Empty<int>();
            }
            
            int[] numbers = new int[type.SidesNumber];
            Array.Fill(numbers, DEFAULT_DICE_SIDE_VALUE);
            
            int[] newNumbers = numbers;
            foreach (SideNumberRule rule in sideNumbersRules)
            {
                newNumbers = rule.GetMutatedSides(numbers);
            }
            if (newNumbers != Array.Empty<int>()) numbers = newNumbers;
            
            return numbers;
        }
    }
}