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

        /// <summary>
        /// Шаговый параметр N для правил с бонусом редкости
        /// (Пиковый, Застрахованный, Стабильный, Утяжелённый, Увеличенный).
        /// Базовое значение: 1.
        /// </summary>
        [Min(1)]
        [SerializeField] protected int n = 1;

        public SideNumberRule(SideNumbersRules rule, int n = 1)
        {
            this.rule = rule;
            this.n    = Mathf.Max(1, n);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Публичный метод
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Получить изменённые по правилам грани
        /// </summary>
        /// <param name="sides">Грани дайса</param>
        /// <returns>Изменённые грани дайса</returns>
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
                // ── Базовые ──────────────────────────────────────────────────

                // Все грани максимального значения
                case SideNumbersRules.AllMax:
                {
                    for (int i = 0; i < sides.Length; i++)
                        numbers[i] = sides.Length;

                    break;
                }

                // Все грани минимального значения
                case SideNumbersRules.AllMin:
                {
                    for (int i = 0; i < sides.Length; i++)
                        numbers[i] = MIN_DICE_SIDE_VALUE;

                    break;
                }

                // ── Математика ────────────────────────────────────────────────

                // Чётный — нечётные значения сдвигаются вверх на +1
                // Пример d6: [1,2,3,4,5,6] → [2,2,4,4,6,6]
                case SideNumbersRules.Even:
                {
                    for (int i = 0; i < sides.Length; i++)
                    {
                        int val = i + 1;
                        numbers[i] = (val % 2 != 0) ? val + 1 : val;
                    }

                    break;
                }

                // Нечётный — чётные значения сдвигаются вниз на -1
                // Пример d6: [1,2,3,4,5,6] → [1,1,3,3,5,5]
                case SideNumbersRules.Odd:
                {
                    for (int i = 0; i < sides.Length; i++)
                    {
                        int val = i + 1;
                        numbers[i] = (val % 2 == 0) ? val - 1 : val;
                    }

                    break;
                }

                // Пиковый — N ближайших к каждому из крайних значений граней
                // заменяются на соответствующий экстремум.
                // Пример d6, N=1: [1,2,3,4,5,6] → [1,1,3,4,6,6]
                // Пример d6, N=2: [1,2,3,4,5,6] → [1,1,1,6,6,6]
                case SideNumbersRules.Peaked:
                {
                    FillDefault(numbers);

                    // Не даём N съесть больше половины граней с каждой стороны
                    int effectiveN = Mathf.Clamp(n, 0, (sides.Length - 1) / 2);

                    // Заменяем N граней со стороны минимума
                    for (int i = 1; i <= effectiveN; i++)
                        numbers[i] = MIN_DICE_SIDE_VALUE;

                    // Заменяем N граней со стороны максимума
                    for (int i = sides.Length - 2; i >= sides.Length - 1 - effectiveN; i--)
                        numbers[i] = sides.Length;

                    break;
                }

                // Застрахованный — крайние значения (мин и макс) заменяются
                // своими ближайшими соседями в сторону МО.
                // Пример d6: [1,2,3,4,5,6] → [2,2,3,4,5,5]
                case SideNumbersRules.Insured:
                {
                    for (int i = 0; i < sides.Length; i++)
                    {
                        int val = i + 1;

                        if (val == MIN_DICE_SIDE_VALUE && sides.Length > 1)
                            numbers[i] = MIN_DICE_SIDE_VALUE + 1;
                        else if (val == sides.Length && sides.Length > 1)
                            numbers[i] = sides.Length - 1;
                        else
                            numbers[i] = val;
                    }

                    break;
                }

                // Стабильный — N граней, ближайших к МО, заменяются значением МО.
                // МО для [1..max] = (max+1)/2, округляется по правилу AwayFromZero.
                // Пример d6 (МО≈4), N=2: [1,2,3,4,5,6] → [1,2,4,4,5,6]
                // Пример d6 (МО≈4), N=4: [1,2,3,4,5,6] → [1,4,4,4,4,6]
                case SideNumbersRules.Stable:
                {
                    FillDefault(numbers);

                    float  mean      = (sides.Length + 1f) / 2f;
                    int    meanValue = (int)Math.Round(mean, MidpointRounding.AwayFromZero);
                    int    effectiveN = Mathf.Clamp(n, 0, sides.Length);

                    // Отсортируем индексы по расстоянию до МО
                    int[] indices = new int[sides.Length];
                    for (int i = 0; i < sides.Length; i++) indices[i] = i;

                    Array.Sort(indices, (a, b) =>
                    {
                        float da = Math.Abs((a + 1) - mean);
                        float db = Math.Abs((b + 1) - mean);
                        return da.CompareTo(db);
                    });

                    for (int i = 0; i < effectiveN; i++)
                        numbers[indices[i]] = meanValue;

                    break;
                }

                // ── Экстремальные ─────────────────────────────────────────────

                // Утяжелённый — максимальная грань +N, минимальная −N.
                // Остальные грани остаются стандартными.
                // Пример d6, N=1: [1,2,3,4,5,6] → [0,2,3,4,5,7]
                case SideNumbersRules.Weighted:
                {
                    FillDefault(numbers);
                    numbers[0]                 -= n;   // минимальная грань
                    numbers[sides.Length - 1]  += n;   // максимальная грань

                    break;
                }

                // Увеличенный — все грани сдвигаются вверх на N.
                // Пример d6, N=1: [1,2,3,4,5,6] → [2,3,4,5,6,7]
                case SideNumbersRules.Increased:
                {
                    for (int i = 0; i < sides.Length; i++)
                        numbers[i] = (i + 1) + n;

                    break;
                }

                // Верхний — грани равномерно заполняют верхнюю половину диапазона
                // (от ceil(МО) до максимума), дублируясь при необходимости.
                // Пример d6 (верхняя половина [4,5,6]): → [4,4,5,5,6,6]
                // Пример d8 (верхняя половина [5,6,7,8]): → [5,5,6,6,7,7,8,8]
                case SideNumbersRules.Upper:
                {
                    // ceil((n+1)/2) — первое значение верхней половины
                    int halfStart  = (int)Math.Ceiling((sides.Length + 1.0) / 2.0);
                    int upperCount = sides.Length - halfStart + 1;

                    for (int i = 0; i < sides.Length; i++)
                        numbers[i] = halfStart + (i * upperCount / sides.Length);

                    break;
                }

                // Нижний — грани равномерно заполняют нижнюю половину диапазона
                // (от минимума до floor(МО)), дублируясь при необходимости.
                // Пример d6 (нижняя половина [1,2,3]): → [1,1,2,2,3,3]
                // Пример d8 (нижняя половина [1,2,3,4]): → [1,1,2,2,3,3,4,4]
                case SideNumbersRules.Lower:
                {
                    // floor((n+1)/2) — последнее значение нижней половины
                    int halfEnd    = (int)Math.Floor((sides.Length + 1.0) / 2.0);
                    int lowerCount = halfEnd;

                    for (int i = 0; i < sides.Length; i++)
                        numbers[i] = 1 + (i * lowerCount / sides.Length);

                    break;
                }

                // Базовые: от 1 до максимума граней дайса
                default:
                {
                    FillDefault(numbers);
                    break;
                }
            }

            return numbers;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Вспомогательное
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Заполняет массив последовательностью 1..n (стандартный дайс)</summary>
        private static void FillDefault(int[] numbers)
        {
            for (int i = 0; i < numbers.Length; i++)
                numbers[i] = i + 1;
        }
    }
}