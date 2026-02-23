using Blackset.Inventories;
using Blackset.Player;
using Extensions.Generics;
using TMPro;
using UnityEngine;

namespace Features.DiceBudget
{
    /// <summary>
    /// Вывод максимального значения пула дайсов определенного номинала
    /// </summary>
    public sealed class MaxPoolBudgetIndicator : InitializableMonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        
        [Header("Данные игрока"), Space]
        [SerializeField]
        private DicePoolInventory pool;
        [SerializeField]
        private PlayerDataFacade playerData;

        private void OnEnable()
        {
            Initialize(pool != null && text != null && playerData != null);
            if (!IsInitialized) return;

            pool.onDataUpdated += ShowBudget;
            ShowBudget();
        }

        private void OnDisable()
        {
            if (!IsInitialized) return;

            pool.onDataUpdated -= ShowBudget;
        }

        private void ShowBudget()
        {
            int usedPoolBudget = playerData.MetaData.Data.GetUsedDiceBudgetByPool(pool);
            text.text = usedPoolBudget.ToString();
        }
    }
}