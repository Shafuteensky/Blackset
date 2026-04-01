using System;
using Blackset.BalanceConfigs;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Конфигурация баланса цен в магазине
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ShopConfig),
        menuName = "Blackset/Shop/" + nameof(ShopConfig))]
    public class ShopConfig : BaseBalanceConfig
    {
        private const float DEFAULT_SELL_MODIFIER = 1f;
        
        #region Инспектор

        [Header("Модификаторы покупки/продажи"), Space]
        
        [SerializeField]
        public BuySellModifiers ItemsBuySell = new()
        {
            ItemSellModifier = DEFAULT_SELL_MODIFIER,
            ItemBuyModifier = 1.5f
        };

        #endregion

        #region Вспомогательные типы
        
        [Serializable]
        public struct BuySellModifiers
        {
            [Range(0f, 2f)] public float ItemSellModifier;
            [Range(0f, 2f)] public float ItemBuyModifier;
        }

        #endregion
    }
}