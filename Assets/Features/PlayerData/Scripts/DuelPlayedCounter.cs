using Extensions.Generics;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Увеличитель сыгранных дуэлей (при открытии окна результатов)
    /// </summary>
    public class DuelPlayedCounter : InitializableMonoBehaviour
    {
        [SerializeField]
        private PlayerDataFacade playerData;
        
        private void OnEnable()
        {
            Initialize(playerData != null && playerData.ProgressData != null);
            if (IsInitialized) playerData.ProgressData.DuelPlayed();
        }
    }
}