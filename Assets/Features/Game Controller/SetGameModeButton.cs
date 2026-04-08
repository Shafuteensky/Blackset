using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.Game
{
    /// <summary>
    /// Кнопка установки режима игры
    /// </summary>
    public class SetGameModeButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameModeValue gameModeValue;
        [SerializeField] private GameMode gameMode;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (gameModeValue != null) gameModeValue.Value = gameMode;
        }
    }
}