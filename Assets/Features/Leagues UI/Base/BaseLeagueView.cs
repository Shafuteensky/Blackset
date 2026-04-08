using Extensions.Generics;
using Features.Leagues;

namespace Blackset.LeaguesUI
{
    /// <summary>
    /// Базовая вьюшка данных лиги
    /// </summary>
    public abstract class BaseLeagueView : InitializableMonoBehaviour
    {
        protected LeagueController leagueController;
        
        protected virtual void Awake()
        {
            leagueController = LeagueController.Instance;
            Initialize(leagueController != null);
        }
    }
}