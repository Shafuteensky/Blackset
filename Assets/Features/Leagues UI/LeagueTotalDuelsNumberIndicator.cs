namespace Blackset.LeaguesUI
{
    /// <summary>
    /// Индикатор количества дуэлей в лиге
    /// </summary>
    public sealed class LeagueTotalDuelsNumberIndicator : BaseLeagueTextView
    {
        private void OnEnable()
        {
            if (!IsInitialized) return;
            
            leagueController.LeagueStart += OnLeagueStart;
        }
        
        private void OnDisable()
        {
            leagueController.LeagueStart -= OnLeagueStart;
        }

        private void OnLeagueStart()
        {
            text.text = leagueController.TotalDuelsNumber.ToString();
        }
    }
}