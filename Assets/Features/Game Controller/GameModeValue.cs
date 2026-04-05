using Extensions.ScriptableValues;
using UnityEngine;

namespace Blackset.Game
{
    /// <summary>
    /// Контейнер флага активного режима игры
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(GameModeValue),
        menuName = "Blackset/League/" + nameof(GameModeValue)
    )]
    public class GameModeValue : ScriptableValue<GameMode> { }
}