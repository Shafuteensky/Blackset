using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.OpponentsRestrictions;
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
        /// <summary>
        /// Ценность сборки: определяет распределение редкостей на сборку
        /// </summary>
        public float BuildValue => buildValue;
        /// <summary>
        /// Уровень мастерства: определяет уровень стратегического мышления
        /// </summary>
        public float MasteryLevel => masteryLevel;
        /// <summary>
        /// Уровень хитрости: определяет как часто соперник будет обманывать на объявлении
        /// </summary>
        public float CunningLevel => cunningLevel;
        
        [Header("Доступ"), Space]
        [SerializeField] private OpponentAvailability baseAvailability = OpponentAvailability.All;
        [SerializeField] private List<OpponentRestriction> restrictions = new();
        
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

        #region Доступность предмета
        
        /// <summary>
        /// Пул дайсов
        /// </summary>
        public List<DiceItemContext> GetDicesPool()
        {
            List<DiceItemContext> dicesInPool = new List<DiceItemContext>();
            
            foreach (OpponentDicesPool pool in dices)
            {
                DiceType diceType = pool.diceNominal;
                foreach (DiceData diceInPool in pool.dices)
                {
                    DiceItemContext diceItem = new DiceItemContext
                    {
                        Dice = diceInPool.Id,
                        Type = diceType.Id
                    };
                    dicesInPool.Add(diceItem);
                }
            }
            
            return dicesInPool;
        }

        /// <summary>
        /// Пул расходников
        /// </summary>
        public List<ConsumableItemContext> GetConsumablesPool()
        {
            List<ConsumableItemContext> consumablesInPool = new List<ConsumableItemContext>();
            
            foreach (ConsumableData consumableInPool in consumables)
            {
                ConsumableItemContext consumableItem = new()
                {
                    Consumable = consumableInPool.Id,
                    Type = GameData.Instance.ConsumableTypes.Data[0].Id
                };
                consumablesInPool.Add(consumableItem);
            }

            return consumablesInPool;
        }

        /// <summary>
        /// Общий уровень сложности соперника
        /// </summary>
        /// <returns>Значение от 0 до 1</returns>
        public float DifficultyLevel()
        {
            float raw = 0f;
            raw += buildValue;
            raw += masteryLevel;
            raw += cunningLevel;

            float average = raw / 3f;
            return average;
        }
        
        #endregion
        
        #region Доступность предмета
        
        /// <summary>
        /// Уровень доступности предмета
        /// </summary>
        public OpponentAvailability GetAvailability()
        {
            OpponentAvailability availability = baseAvailability;

            foreach (OpponentRestriction restriction in restrictions)
            {
                if (restriction == null) continue;

                OpponentAvailability blockedAvailability = restriction.GetBlockedAvailability(this);
                availability &= ~blockedAvailability;

                if (availability == OpponentAvailability.None) break;
            }

            return availability;
        }

        /// <summary>
        /// Доступен ли соперник к дуэли
        /// </summary>
        /// <returns>true если доступен, иначе false</returns>
        public bool IsAvailableForDuel() => (GetAvailability() & OpponentAvailability.Duel) != 0;

        /// <summary>
        /// Доступен ли предмет к лиге
        /// </summary>
        /// <returns>true если доступен, иначе false</returns>
        public bool IsAvailableForLeague() => (GetAvailability() & OpponentAvailability.League) != 0;
        
        #endregion
    }
}