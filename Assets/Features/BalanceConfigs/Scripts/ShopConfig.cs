using System;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Конфигурация баланса цен в магазине
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ShopConfig),
        menuName = "Blackset/Shop/" + nameof(ShopConfig))]
    public class ShopConfig : ScriptableObject
    {
        #region Инспектор

        [Header("Цены гача-боксов"), Space]

        [SerializeField]
        public SecretBoxBuyPrices SecretBoxBuy = new()
        {
            diceBoxFixedPrice = 14,
            consumableBoxFixedPrice = 8
        };
        
        [SerializeField]
        public SellModifiers ItemsSell = new()
        {
            itemSellModifier = 0.5f,
        };

        #endregion

        #region Вспомогательные типы

        [Serializable]
        public struct SecretBoxBuyPrices
        {
            [Min(0)] public int diceBoxFixedPrice;
            [Min(0)] public int consumableBoxFixedPrice;
        }
        
        [Serializable]
        public struct SellModifiers
        {
            [Range(0f, 2f)] public float itemSellModifier;
        }

        #endregion
    }
}