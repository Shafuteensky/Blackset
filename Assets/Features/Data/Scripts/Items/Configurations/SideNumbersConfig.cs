using System;
using System.Collections.Generic;
using System.Text;
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
        private const string MODIFIED_COLOR = "#C75C00";

        [Header("Правила заполнения граней"), Space]
        [SerializeField]
        private List<SideNumberRule> sideNumbersRules = new();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            // В первую очередь заполнение по базовым правилам — от 1 до максимума граней дайса
            if (sideNumbersRules.Count <= 0)
            {
                sideNumbersRules.Add(new SideNumberRule(SideNumbersRules.Default));
            }
        }
#endif

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

        /// <summary>
        /// Получить строковое представление граней дайса в формате [1,2,3,4,5,6].
        /// Значения, отличающиеся от стандартного диапазона [1..K], выделяются оранжевым цветом (TMP rich text).
        /// </summary>
        /// <param name="type">Тип дайса</param>
        /// <returns>Строка вида [1,<color=#FF8C00>8</color>,3,4,5,6]</returns>
        public string GetSideNumbersString(DiceType type)
        {
            int[] sides = GetSideNumbers(type);

            if (sides == null || sides.Length == 0)
                return "[]";

            int           sidesCount = sides.Length;
            StringBuilder sb         = new StringBuilder("[");

            for (int i = 0; i < sidesCount; i++)
            {
                int  value     = sides[i];
                bool isDefault = value == i + 1; // стандартное значение для позиции i — это i+1

                if (isDefault)
                {
                    sb.Append(value);
                }
                else
                {
                    sb.Append($"<color={MODIFIED_COLOR}>{value}</color>");
                }

                if (i < sidesCount - 1)
                    sb.Append(", ");
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}