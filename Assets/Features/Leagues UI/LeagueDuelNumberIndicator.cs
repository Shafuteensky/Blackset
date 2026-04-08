namespace Blackset.LeaguesUI
{
    /// <summary>
    /// Индикатор номера текущей лиги
    /// </summary>
    public sealed class LeagueDuelNumberIndicator : BaseLeagueTextView
    {
        private void OnEnable()
        {
            if (!IsInitialized) return;

            leagueController.LeagueDuelStart += OnLeagueStart;
            
            OnLeagueStart(leagueController.CurrentDuelIndex);
        }
        
        private void OnDisable()
        {
            leagueController.LeagueDuelStart -= OnLeagueStart;
        }

        private void OnLeagueStart(int leagueIndex)
        {
            text.text = (leagueIndex + 1).ToString();
        }
    }
}