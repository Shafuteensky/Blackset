using Extensions.Log;
using UnityEngine;

namespace Blackset.Player
{
    public class PlayerDataTest : MonoBehaviour
    {
        [SerializeField]
        protected PlayerDataContainer playerDataContainer;
        
        private void OnEnable()
        {
            ServiceDebug.Log($"Money: {playerDataContainer.Data.Money}");
            playerDataContainer.Data.AddMoney(1);
            playerDataContainer.NotifyUpdated();
            playerDataContainer.RequestSave();
            ServiceDebug.Log($"Money: {playerDataContainer.Data.Money}");
        }
    }
}