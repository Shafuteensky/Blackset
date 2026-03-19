using Blackset.Duel.Sequence;
using Extensions.Generics;

namespace Blackset.DuelUI.Controls
{
    /// <summary>
    /// Кнопка завершения дуэли ничьей
    /// </summary>
    public class EndDuelButton : AbstractButton
    {
        public override void OnButtonClick()
        {
            DuelController.Instance.EndDuel();
        }
    }
}