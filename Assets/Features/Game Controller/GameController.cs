using Blackset.Duel.Sequence;
using Blackset.DuelUI;
using Blackset.LeaguesUI;
using Extensions.Log;
using Extensions.SceneFlow;
using Extensions.Singleton;
using Features.Leagues;
using UnityEngine;

namespace Blackset.Game
{
    /// <summary>
    /// Контроллер режимов игры
    /// </summary>
    // TODO Перевести контроллеры режимов в не-синглтоны, вызывать через этот контроллер
    public class GameController : MonoBehaviourSingleton<GameController>
    {
        [Header("Режимы игры"), Space]
        [SerializeField] private GameModeValue gameMode;

        [Header("Контроллеры окон результатов"), Space] 
        [SerializeField] private LeagueWindowsOpen leagueWindowsController;
        [SerializeField] private DuelEndWindowOpen DuelEndWindowController;
            
        [Header("Фолбэк сцена"), Space]
        [SerializeField] private SceneID fallbackScene;
        
        private DuelController duelController;
        private LeagueController leagueController;

        private void Start()
        {
            duelController = DuelController.Instance;
            leagueController = LeagueController.Instance;

            if (gameMode == null || duelController == null || leagueController == null)
            {
                LoadFallbackScene();
                return;
            }
            
            if (leagueWindowsController != null) 
                leagueWindowsController.enabled = gameMode.Value == GameMode.League;
            if (DuelEndWindowController != null) 
                DuelEndWindowController.enabled = gameMode.Value == GameMode.Duel;
            
            StartGame();
        }

        private void LoadFallbackScene()
        {
            if (fallbackScene != null) 
                SceneController.Instance.LoadSceneByID(fallbackScene.Id);
        }

        /// <summary>
        /// Начать игру по текущему режиму
        /// </summary>
        public void StartGame()
        {
            switch (gameMode.Value)
            {
                case GameMode.Duel:
                    duelController.StartDuel();
                    break;
                
                case GameMode.League:
                    leagueController.StartLeague();
                    break;
                
                default:
                    ServiceDebug.LogError("Необработанный режим игры, игра не начата");
                    LoadFallbackScene();
                    break;
            }
        }

        /// <summary>
        /// Завершить игру по текущему режиму
        /// </summary>
        public void EndGame()
        {
            switch (gameMode.Value)
            {
                case GameMode.Duel:
                    duelController.EndDuel();
                    break;
                
                case GameMode.League:
                    leagueController.EndLeague();
                    break;
                
                default:
                    ServiceDebug.LogError("Необработанный режим игры, игра не завершена");
                    break;
            }
        }
    }
}