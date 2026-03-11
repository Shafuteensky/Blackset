using System;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Log;
using Extensions.UIWindows;
using Features.Duel.Data.FightEnd;
using UnityEngine;

namespace Features.DuelUI
{
    /// <summary>
    /// Контроллер открытия окон по завершению дуэли
    /// </summary>
    public sealed class DuelEndWindowOpen : MonoBehaviour
    {
        [Header("Окна"), Space]
        [SerializeField] private UIWindowID windowOnWin;
        [SerializeField] private UIWindowID windowOnLose;
        [SerializeField] private UIWindowID windowOnDraw;
        
        private DuelController duelController;
        private UIWindowsController windowsController;
        
        private void Awake()
        {
            duelController = DuelController.Instance;
            windowsController = UIWindowsController.Instance;
        }
        
        private void OnEnable() => duelController.EventHub.Subscribe<DuelEndEvent>(OpenResultsWindowOnDuelEnd);

        private void OnDisable() => duelController?.EventHub.Unsubscribe<DuelEndEvent>(OpenResultsWindowOnDuelEnd);

        private void OpenResultsWindowOnDuelEnd(DuelEndEvent handler)
        {
            string windowToOpen;
            
            switch (handler.DuelEndResult.Winner.Value)
            {
                case FightWinner.Player:
                {
                    windowToOpen = windowOnWin.Id;
                    break;
                }
                case FightWinner.Opponent:
                {
                    windowToOpen = windowOnLose.Id;
                    break;
                }
                case FightWinner.None:
                {
                    windowToOpen = windowOnDraw.Id;
                    break;
                }
                default:
                {
                    windowToOpen = windowOnDraw.Id;
                    ServiceDebug.LogError("Необработанный случай завершения дуэли, засчет ничьей для вывода окна");
                    break;
                }
            }
            
            windowsController?.OpenWindowByID(windowToOpen);
        }
    }
}