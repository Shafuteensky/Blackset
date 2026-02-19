using UnityEngine;
using Extensions.Coroutines;
using Extensions.Generics;
#if UNITY_EDITOR
    using UnityEditor;
#endif
#if DOTWEEN
    using DG.Tweening;
#endif

namespace Extensions.UIWindows
{
    /// <summary>
    /// Кнопка для закрытия окна интерфейса с анимацией DOTWEEN
    /// </summary>
    public class ButtonCloseUIWindowAnimated : AbstractButton
    {
#if DOTWEEN
        [SerializeField]
        protected DOTweenAnimation beforeCloseAnimation;
#endif

        /// <summary>
        /// Закрытие текущего окна и открытие предыдущего с анимацией
        /// </summary>
        public override void OnButtonClick()
        {
            UIWindowsController windowsController = UIWindowsController.Instance;

            if (!windowsController.FocusedWindow.PreviousWindow)
                return;

            windowsController.OpenPreviousWindow();

#if DOTWEEN
            if (beforeCloseAnimation)
            {
                beforeCloseAnimation.DORestart();
                CoroutineDelay.Run(this, beforeCloseAnimation.duration, windowsController.CloseFocusedWindow);
                return;
            }
#endif

            windowsController.CloseFocusedWindow();
        }

#if UNITY_EDITOR
        [ContextMenu("Convert To ButtonCloseUIWindow")]
        protected void ConvertToNonAnimated()
        {
            GameObject go = gameObject;

            Undo.RecordObject(go, "Convert To ButtonCloseUIWindow");

            DestroyImmediate(this, true);
            go.AddComponent<ButtonCloseUIWindow>();

            EditorUtility.SetDirty(go);
        }
#endif
    }
}