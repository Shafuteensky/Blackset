using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Индикатор статуса раскрытия предмета
    /// </summary>
    public class KnownIndicator : BaseKnownStateCallback
    {
        [Header("Элементы"), Space]
        [Tooltip("Индикатор знания")]
        [SerializeField] private GameObject knownIndicator;

        private void Awake()
        {
            if (knownIndicator != null) knownIndicator.SetActive(false);
        }

        protected override void OnStateUpdate()
        {
            if (knownIndicator != null) knownIndicator.SetActive(IsItemRevealed());
        }
    }
}