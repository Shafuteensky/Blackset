using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Opponents
{
    /// <summary>
    /// Данные соперника в дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(OpponentData),
        menuName = "Blackset/Opponents/" + nameof(OpponentData))]
    public class OpponentData : BaseData
    {   
        [Header("Параметры сложности"), Space]
        [SerializeField]
        [Tooltip("Ценность сборки: определяет распределение редкостей на сборку (0 — все редкости минимальные, 1 — максимальные)")]
        [Range(0f, 1f)]
        protected float buildValue;
        [SerializeField]
        [Tooltip("Уровень мастерства: определяет уровень стратегического мышления (0 — случайный выбор, 1 — осмысленный выбор)")]
        [Range(0f, 1f)]
        protected float masteryLevel;
        [SerializeField]
        [Tooltip("Уровень хитрости: определяет как часто соперник будет обманывать на объявлении (0 — полностью честен, 1 — всегда лжет)")] 
        [Range(0f, 1f)]
        protected float cunningLevel; // TODO Нужен ли параметр? Механика обмана еще в плане?
        
        
        [Header("Пулы предметов"), Space]
        [SerializeField]
        [Tooltip("Пул дайсов (для дуэли берутся случайные)")]
        protected List<OpponentDicesPool> dices = new();
        [SerializeField]
        [Tooltip("Пул расходников (для дуэли берутся случайные)")]
        protected List<ConsumableData> consumables = new();
    }
}