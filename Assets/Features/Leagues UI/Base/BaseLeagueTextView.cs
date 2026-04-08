using TMPro;
using UnityEngine;

namespace Blackset.LeaguesUI
{
    /// <summary>
    /// Базовый текстовый индикатор денных лиги
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public abstract class BaseLeagueTextView : BaseLeagueView
    {
        protected TMP_Text text;

        protected override void Awake()
        {
            base.Awake();
            text = GetComponent<TMP_Text>();
            
            Initialize(text != null);
        }
    }
}