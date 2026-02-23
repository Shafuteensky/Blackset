using Extensions.Data.InMemoryData.SelectionContext;
using UnityEngine;

namespace Blackset.Opponents
{
    /// <summary>
    /// Контекст выбора контракта соперника дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(SelectedContract), 
        menuName = "Blackset/Opponents/" + nameof(SelectedContract))]
    public class SelectedContract : SelectionContext<OpponentContract> { }
}