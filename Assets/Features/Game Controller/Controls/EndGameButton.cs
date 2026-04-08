using Extensions.Generics;

namespace Blackset.Game.Controls
{
    /// <summary>
    /// Кнопка завершения игры по текущему режиму
    /// </summary>
    public class EndGameButton : AbstractButton
    {
        public override void OnButtonClick()
        {
            GameController.Instance.EndGame();
        }
    }
}