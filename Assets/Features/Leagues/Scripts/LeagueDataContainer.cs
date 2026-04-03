using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Features.Leagues
{
    /// <summary>
    /// Контейнер мета-данных игрока
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(LeagueDataContainer),
        menuName = "Blackset/League/" + nameof(LeagueDataContainer))]
    public class LeagueDataContainer : InMemorySingleDataContainer<LeagueContract>
    {
        public void GenerateNewLeague()
        {
            Data = new LeagueContract();
        }
    }
}