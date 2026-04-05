using Extensions.Generics;
using UnityEngine;

namespace Blackset.Game
{
    /// <summary>
    /// Кнопка установки режима игры
    /// </summary>
    public class SetGameModeButton : AbstractButton
    {
        [SerializeField] private GameModeValue gameModeValue;
        [SerializeField] private GameMode gameMode;
        
        public override void OnButtonClick()
        {
            if (gameModeValue != null) gameModeValue.Value = gameMode;
        }
    }
}