using Extensions.UIWindows;
using Features.Leagues;
using UnityEngine;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Контроллер открытия окон по завершению дуэли
    /// </summary>
    public sealed class LeagueWindowsOpen : MonoBehaviour
    {
        [Header("Окна"), Space]
        [Tooltip("Окно, открывающееся при победе в дуэли в составе лиги")]
        [SerializeField] private UIWindowID duelWinWindow;
        [Tooltip("Окно, открывающееся при победе в лиге")]
        [SerializeField] private UIWindowID leagueWinWindow;
        [Tooltip("Окно, открывающееся при проигрыше в дуэле/лиге")]
        [SerializeField] private UIWindowID leagueLoseWindow;
        
        private LeagueController leagueController;
        private UIWindowsController windowsController;
        
        private void Awake()
        {
            leagueController = LeagueController.Instance;
            windowsController = UIWindowsController.Instance;
        }
        
        private void OnEnable()
        {
            leagueController.LeagueDuelWonEvent += OpenWinWindow;
            leagueController.LeagueLoseEvent += OpenLoseWindow;
            leagueController.LeagueWonEvent += OpenDuelWinWindow;
        }

        private void OnDisable()
        {
            leagueController.LeagueDuelWonEvent -= OpenWinWindow;
            leagueController.LeagueLoseEvent -= OpenLoseWindow;
            leagueController.LeagueWonEvent -= OpenDuelWinWindow;
        }

        private void OpenWinWindow() =>  windowsController?.OpenWindowByID(leagueWinWindow.Id);
        private void OpenLoseWindow() =>  windowsController?.OpenWindowByID(leagueLoseWindow.Id);
        private void OpenDuelWinWindow() =>  windowsController?.OpenWindowByID(duelWinWindow.Id);
    }
}