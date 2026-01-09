using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Extensions.EditorTools.Bookmarks;
using Extensions.EditorTools.EditorTools;
using Extensions.Logs;

namespace Extensions.EditorTools
{
    /// <summary>
    /// Окно для закладок объектов (Scene + Project).
    /// Полная поддержка JSON, GUID, FileID и восстановления объектов между сессиями.
    /// </summary>
    public sealed class BookmarksWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Bookmarks";
        private const string PROJECT_ASSETS_SECTION = "ProjectAssets";
        private const string UNKNOWN_SCENE_SECTION = "UnknownScene";
        
        [SerializeField]
        private BookmarksDataBase dataBase;

        private GUIStyle _bookmarkStyle;
        private GUIStyle _selectedBookmarkStyle;
        private GUIStyle _dropStyle;
        private Vector2 _scroll = Vector2.zero;
        
        private Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

        [MenuItem("Tools/" + WINDOW_NAME)]
        public static void Open()
        {
            BookmarksWindow window = GetWindow<BookmarksWindow>();
            window.titleContent = new GUIContent(WINDOW_NAME);
            window.Show();
        }

        private void OnGUI()
        {
            if (dataBase == null)
            {
                EditorGUILayout.HelpBox("BookmarksDataBase не назначена", MessageType.Error);
                return;
            }

            if (_bookmarkStyle == null)
            {
                InitStyles();
            }

            DrawAddButton();
            GUILayout.Space(EditorToolsConstraints.SPACE_BLOCK_SIZE);

            _scroll = GUILayout.BeginScrollView(_scroll);
            DrawBookmarksList();
            GUILayout.EndScrollView();
        }

        #region UI

        private void InitStyles()
        {
            _bookmarkStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = EditorToolsConstraints.BASE_FONT_SIZE,
                fixedHeight = EditorToolsConstraints.BASE_ELEMENT_HEIGHT,
                padding = new RectOffset(EditorToolsConstraints.TEXT_PADDING, EditorToolsConstraints.TEXT_PADDING, 0, 0)
            };

            _selectedBookmarkStyle = new GUIStyle(_bookmarkStyle)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.green }
            };
            
            _dropStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = EditorToolsConstraints.BASE_FONT_SIZE,
                fixedHeight = EditorToolsConstraints.BASE_ELEMENT_HEIGHT,
                padding = new RectOffset(EditorToolsConstraints.TEXT_PADDING, EditorToolsConstraints.TEXT_PADDING, 0, 0)
            };
        }

        private void DrawAddButton()
        {
            Object selected = Selection.activeObject;
            GUI.enabled = selected != null;

            if (GUILayout.Button("+ Добавить выбранный объект +", GUILayout.Height(EditorToolsConstraints.BASE_ELEMENT_HEIGHT)))
            {
                AddBookmark(selected);
            }

            GUI.enabled = true;
        }

        private void DrawDragAndDropZone()
        {
            Rect dropRect = GUILayoutUtility.GetRect(80, EditorToolsConstraints.BASE_ELEMENT_HEIGHT, GUILayout.ExpandWidth(false));
            GUI.Box(dropRect, "Drop", _dropStyle);  
            
            Event evt = Event.current;

            if (dropRect.Contains(evt.mousePosition))
            {
                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        AddBookmark(obj);
                    }
                    evt.Use();
                }
            }
        }

        #endregion

        #region Bookmark List

        private void DrawBookmarksList()
        {
            IReadOnlyList<BookmarkData> items = dataBase.DataBase;

            if (items.Count == 0)
            {
                GUILayout.Label("Закладок пока нет");
                return;
            }

            Object selected = Selection.activeObject;
            string activeScene = SceneManager.GetActiveScene().path;

            Dictionary<string, List<BookmarkData>> groups = new Dictionary<string, List<BookmarkData>>();

            foreach (var b in items)
            {
                string key;

                if (b.Type == BookmarkType.ProjectAsset)
                {
                    key = PROJECT_ASSETS_SECTION;
                }
                else
                {
                    key = string.IsNullOrEmpty(b.ScenePath) ? UNKNOWN_SCENE_SECTION : b.ScenePath;
                }

                if (!groups.ContainsKey(key))
                    groups[key] = new List<BookmarkData>();

                groups[key].Add(b);
            }

            List<string> sortedKeys = new List<string>(groups.Keys);

            sortedKeys.Sort((a, b) =>
            {
                if (a == PROJECT_ASSETS_SECTION) return 1;
                if (b == PROJECT_ASSETS_SECTION) return -1;

                if (a == UNKNOWN_SCENE_SECTION) return 1;
                if (b == UNKNOWN_SCENE_SECTION) return -1;

                return a.CompareTo(b);
            });

            foreach (string key in sortedKeys)
            {
                string foldoutLabel;

                if (key == PROJECT_ASSETS_SECTION)
                {
                    foldoutLabel = $"Ассеты проекта ({groups[key].Count})";
                }
                else
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(key);
                    bool isCurrent = key == activeScene;

                    foldoutLabel = isCurrent
                        ? $"Сцена: {sceneName} (текущая, {groups[key].Count})"
                        : $"Сцена: {sceneName} ({groups[key].Count})";
                }

                if (!_foldouts.ContainsKey(key))
                    _foldouts[key] = true;

                _foldouts[key] = EditorGUILayout.Foldout(_foldouts[key], foldoutLabel, true);

                if (_foldouts[key])
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < groups[key].Count; i++)
                    {
                        BookmarkData data = groups[key][i];
                        DrawBookmarkRow(data, selected);
                    }

                    EditorGUI.indentLevel--;
                    GUILayout.Space(6);
                }
            }
        }

        private void DrawBookmarkRow(BookmarkData data, Object selected)
        {
            Object resolved = ResolveUnityObject(data);

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = GetRowColor(data, resolved);

            EditorGUILayout.BeginHorizontal(GUILayout.Height(EditorToolsConstraints.BASE_ELEMENT_HEIGHT));

            DrawIcon(resolved);

            bool isSelected = resolved != null && resolved == selected;

            DrawNameButton(data, resolved, isSelected);
            DrawSceneActionIfNeeded(data, resolved);
            DrawRemoveButton(data);

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = prevColor;
        }

        private void DrawIcon(Object resolved)
        {
            Texture icon = resolved != null
                ? EditorGUIUtility.ObjectContent(resolved, resolved.GetType()).image
                : EditorGUIUtility.IconContent("console.erroricon").image;

            GUILayout.Label(icon, GUILayout.Width(EditorToolsConstraints.BASE_ELEMENT_HEIGHT), GUILayout.Height(EditorToolsConstraints.BASE_ELEMENT_HEIGHT));
        }

        private void DrawNameButton(BookmarkData data, Object resolved, bool isSelected)
        {
            GUI.enabled = resolved != null;

            if (GUILayout.Button(data.Name, isSelected ? _selectedBookmarkStyle : _bookmarkStyle))
            {
                Selection.activeObject = resolved;
                EditorGUIUtility.PingObject(resolved);
            }

            GUI.enabled = true;
        }

        private void DrawSceneActionIfNeeded(BookmarkData data, Object resolved)
        {
            if (data.Type != BookmarkType.SceneObject)
            {
                return;
            }

            if (!SceneFileExists(data))
            {
                return;
            }

            if (!IsSceneLoaded(data.ScenePath))
            {
                GUI.backgroundColor = EditorToolsConstraints.COLOR_YELLOW;

                if (GUILayout.Button("Открыть сцену", GUILayout.Width(110), GUILayout.Height(EditorToolsConstraints.BASE_ELEMENT_HEIGHT)))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(data.ScenePath);
                    }
                }

                GUI.backgroundColor = Color.white;
            }
        }

        private void DrawRemoveButton(BookmarkData data)
        {
            GUI.backgroundColor = EditorToolsConstraints.COLOR_RED;

            if (GUILayout.Button("✕", GUILayout.Width(EditorToolsConstraints.BASE_ELEMENT_HEIGHT), GUILayout.Height(EditorToolsConstraints.BASE_ELEMENT_HEIGHT)))
            {
                dataBase.Remove(data);
            }

            GUI.backgroundColor = Color.white;
        }

        #endregion

        #region Bookmark Logic

        private void AddBookmark(Object obj)
        {
            if (obj == null)
                return;

            BookmarkData data = new BookmarkData
            {
                Name = obj.name
            };

            string assetPath = AssetDatabase.GetAssetPath(obj);
            bool isSceneObj = obj is GameObject || obj is Component;

            if (!string.IsNullOrEmpty(assetPath) && !isSceneObj)
            {
                data.Type = BookmarkType.ProjectAsset;
                data.AssetPath = assetPath;
            }
            else
            {
                GameObject go = GetSceneGameObject(obj);
                if (go == null)
                {
                    ServiceDebug.LogWarning("Невозможно сохранить закладку на объект сцены");
                    return;
                }

                data.Type = BookmarkType.SceneObject;
                data.ScenePath = go.scene.path;

                GlobalObjectId gid = GlobalObjectId.GetGlobalObjectIdSlow(go);
                data.Id = gid.ToString();
            }

            dataBase.Add(data);
        }

        private GameObject GetSceneGameObject(Object obj)
        {
            if (obj is GameObject go)
                return go;

            if (obj is Component comp)
                return comp.gameObject;

            return null;
        }

        private Object ResolveUnityObject(BookmarkData data)
        {
            if (data.Type == BookmarkType.ProjectAsset)
            {
                return AssetDatabase.LoadAssetAtPath<Object>(data.AssetPath);
            }

            if (!IsSceneLoaded(data.ScenePath))
            {
                return null;
            }

            if (string.IsNullOrEmpty(data.Id))
            {
                return null;
            }

            if (!GlobalObjectId.TryParse(data.Id, out GlobalObjectId gid))
            {
                return null;
            }

            return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
        }

        #endregion

        #region Helpers

        private bool IsSceneLoaded(string scenePath)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.path == scenePath)
                    return true;
            }
            return false;
        }

        private bool SceneFileExists(BookmarkData data)
        {
            return !string.IsNullOrEmpty(data.ScenePath) &&
                   System.IO.File.Exists(data.ScenePath);
        }

        private Color GetRowColor(BookmarkData data, Object resolved)
        {
            if (data.Type == BookmarkType.ProjectAsset)
                return EditorToolsConstraints.COLOR_CYAN;

            if (!SceneFileExists(data))
                return EditorToolsConstraints.COLOR_RED;

            if (!IsSceneLoaded(data.ScenePath))
                return EditorToolsConstraints.COLOR_YELLOW;

            if (resolved == null)
                return EditorToolsConstraints.COLOR_LIGHT_RED;

            return EditorToolsConstraints.COLOR_LIGHT_GREEN;
        }

        #endregion
    }
}
