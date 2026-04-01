using Blackset.BalanceConfigs;
using UnityEditor;
using UnityEngine;

namespace Blackset.Editor
{
    /// <summary>
    /// Кастомный визуал для листов конфигураций
    /// </summary>
    [CustomEditor(typeof(BaseBalanceConfig), editorForChildClasses: true)]
    public class BaseBalanceConfigEditor : UnityEditor.Editor
    {
        private static readonly Color HeaderColor = new(0.18f, 0.38f, 0.58f);
        private static readonly Color TextColor   = Color.white;

        public override void OnInspectorGUI()
        {
            DrawCustomHeader();
            EditorGUILayout.Space(4);
            DrawDefaultInspector();
        }

        private void DrawCustomHeader()
        {
            Rect rect = GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true));

            EditorGUI.DrawRect(rect, HeaderColor);

            GUIStyle style = new(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize  = 13,
                normal    = { textColor = TextColor }
            };

            // Берём имя прямо из asset-файла — будет то же, что задано в fileName у CreateAssetMenu
            EditorGUI.LabelField(rect, "⚙ " + target.name, style);
        }
    }
}