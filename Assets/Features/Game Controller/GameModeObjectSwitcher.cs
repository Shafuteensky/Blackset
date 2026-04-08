using System.Collections.Generic;
using UnityEngine;

namespace Blackset.Game
{
    /// <summary>
    /// Переключатель объектов в зависимости от режима игры
    /// </summary>
    public sealed class GameModeObjectSwitcher : MonoBehaviour
    {
        [Header("Режимы игры"), Space]
        [SerializeField] private GameModeValue gameMode;
        
        [Header("Переключаемые объекты"), Space]
        [SerializeField] private List<GameObject> duelObjects;
        [SerializeField] private List<GameObject> leagueObjects;

        private void OnEnable()
        {
            bool isDuel = gameMode.Value == GameMode.Duel;

            foreach (var duelObject in duelObjects)
                duelObject.SetActive(isDuel);
                    
            foreach (var leagueObject in leagueObjects)
                leagueObject.SetActive(!isDuel);
        }
    }
}