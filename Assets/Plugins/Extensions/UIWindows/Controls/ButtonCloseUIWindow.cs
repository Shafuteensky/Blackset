using Extensions.Generics;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Extensions.UIWindows
{
    /// <summary>
    /// Кнопка для закрытия окна интерфейса
    /// </summary>
    public class ButtonCloseUIWindow : GenericButton
    {
        /// <summary>
        /// Закрытие текущего окна в фокусе и открытие предыдущего
        /// </summary>
        public override void OnButtonClick()
        {
            UIWindowsController windowsController = UIWindowsController.Instance;

            if (!windowsController.FocusedWindow.PreviousWindow)
            {
                Debug.LogError($"PreviousWindow is not set to {windowsController.FocusedWindow.name}");
                return;
            }

            windowsController.CloseFocusedWindow();
            windowsController.OpenPreviousWindow();
        }

#if UNITY_EDITOR
        [ContextMenu("Convert To ButtonCloseUIWindowAnimated")]
        protected void ConvertToAnimated()
        {
            GameObject go = gameObject;

            Undo.RecordObject(go, "Convert To ButtonCloseUIWindowAnimated");

            DestroyImmediate(this, true);
            go.AddComponent<ButtonCloseUIWindowAnimated>();

            EditorUtility.SetDirty(go);
        }
#endif
    }
}
