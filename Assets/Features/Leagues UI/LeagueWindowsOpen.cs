using Extensions.UIWindows;
using UnityEngine;

namespace Blackset.LeaguesUI
{
    /// <summary>
    /// Контроллер открытия окон по завершению дуэли
    /// </summary>
    public sealed class LeagueWindowsOpen : BaseLeagueView
    {
        [Header("Окна"), Space]
        [Tooltip("Окно, открывающееся при победе в дуэли в составе лиги")]
        [SerializeField] private UIWindowID duelWinWindow;
        [Tooltip("Окно, открывающееся при победе в лиге")]
        [SerializeField] private UIWindowID leagueWinWindow;
        [Tooltip("Окно, открывающееся при проигрыше в дуэле/лиге")]
        [SerializeField] private UIWindowID leagueLoseWindow;
        
        private UIWindowsController windowsController;
        
        protected override void Awake()
        {
            base.Awake();
            windowsController = UIWindowsController.Instance;
        }
        
        private void OnEnable()
        {
            leagueController.LeagueDuelWonEvent += OpenDuelWinWindow;
            leagueController.LeagueLoseEvent += OpenLoseWindow;
            leagueController.LeagueWonEvent += OpenWinWindow;
        }

        private void OnDisable()
        {
            leagueController.LeagueDuelWonEvent -= OpenDuelWinWindow;
            leagueController.LeagueLoseEvent -= OpenLoseWindow;
            leagueController.LeagueWonEvent -= OpenWinWindow;
        }

        private void OpenWinWindow() =>  windowsController?.OpenWindowByID(leagueWinWindow.Id);
        private void OpenLoseWindow() =>  windowsController?.OpenWindowByID(leagueLoseWindow.Id);
        private void OpenDuelWinWindow() =>  windowsController?.OpenWindowByID(duelWinWindow.Id);
    }
}