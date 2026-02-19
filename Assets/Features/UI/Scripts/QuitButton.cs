using Extensions.Generics;
using UnityEditor;
using UnityEngine;

namespace Blackset.UI
{
    /// <summary>
    /// Кнопка выхода из игры
    /// </summary>
    public class QuitButton : AbstractButton
    {
        public override void OnButtonClick()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}