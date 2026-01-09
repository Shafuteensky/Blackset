using DG.Tweening;
using UnityEngine;
using Extensions.Coroutines;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Extensions.UIWindows
{
    /// <summary>
    /// Кнопка для закрытия окна интерфейса с анимацией
    /// </summary>
    public class ButtonCloseUIWindowAnimated : GenericButton
    {
        [SerializeField]
        protected DOTweenAnimation beforeCloseAnimation;

        /// <summary>
        /// Закрытие текущего окна и открытие предыдущего с анимацией
        /// </summary>
        public override void OnButtonClick()
        {
            UIWindowsController windowsController = UIWindowsController.Instance;

            if (!windowsController.FocusedWindow.PreviousWindow)
                return;

            windowsController.OpenPreviousWindow();

            if (beforeCloseAnimation)
            {
                beforeCloseAnimation.DORestart();
                CoroutineDelay.Run(this, beforeCloseAnimation.duration, windowsController.CloseFocusedWindow);
                return;
            }

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