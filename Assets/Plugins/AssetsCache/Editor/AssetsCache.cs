using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Utilities.AssetsCache
{
    public class AssetsCache : EditorWindow
    {
        private const string StrSaveName = "AssetsCacheSave";
        private const string StrShowTab = "Show Tabs";
        private const string StrHideTab = "Hide Tabs";
        private const string StrDone = "Done";
        private const string StrEdit = "Edit";
        private const string StrReset = "Reset";
        private const string StrDrag = "Drag and drop assets here";

        public List<CacheTabInfo> tabs = new() { new CacheTabInfo { id = 0, tabName = "Common" } };
        public List<CacheObjectInfo> objects = new();
        public List<CacheObjectInfo> filterList = new();

        public string searchText = string.Empty;
        private string _oldSearchText;
        private Vector2 _objectsScrollPosition;
        private Vector2 _tabsScrollPosition;
        private EditorCacheStyle _s;
        private int _oldTabIndex = -1;
        private int _tabIndex;
        private bool _isEditMode;
        private bool _isShowTabs;
        private ReorderableList _objectsReorderableList;
        private ReorderableList _tabsReorderableList;

        private float _currentTabViewWidth;
        private bool _isResize;
        private Rect _cursorChangeRect;

        private void OnEnable()
        {
            _oldSearchText = "old";
            InitReOrderTabList();

            _currentTabViewWidth = 110;
            _cursorChangeRect = new Rect(_currentTabViewWidth, 0, 5, position.size.y);
        }

        [MenuItem("Tools/Assets Cache")]
        public static void Init()
        {
            var w = GetWindow<AssetsCache>();
            w.titleContent.text = "Assets Cache";
            w.Read();
            w.Show();
        }

        private void InitReOrderTabList()
        {
            _tabsReorderableList = new ReorderableList(tabs, typeof(CacheTabInfo), true, false, true, true);
            _tabsReorderableList.drawElementCallback += DrawTabElementCallback;
            _tabsReorderableList.onChangedCallback += _ => { Save(); };
            _tabsReorderableList.onAddCallback += _ => { AddNewTab(); };
        }

        private void FilterWhenTabChanged()
        {
            if (_oldTabIndex != _tabIndex)
            {
                if (tabs.Count > 0)
                {
                    _oldTabIndex = _tabIndex;
                    if (tabs[_tabIndex].list == null) tabs[_tabIndex].list = new List<CacheObjectInfo>();
                    objects = tabs[_tabIndex].list;
                    Filter();
                }
                else
                    tabs = new List<CacheTabInfo> { new() { id = 0, tabName = "Common" } };
            }
        }

        private void OnGUI()
        {
            if (_s == null) _s = new EditorCacheStyle();
            FilterWhenTabChanged();
            DisplayTopBar();
            GUILayout.BeginHorizontal();
            if (_isShowTabs)
                DisplayTabs();
            DisplayObjectGroup();
            GUILayout.EndHorizontal();
            Repaint();
        }

        private void DisplayObjects()
        {
            GUILayout.BeginHorizontal();
            _objectsScrollPosition = GUILayout.BeginScrollView(_objectsScrollPosition);
            if (_isEditMode) DisplayReorderList();
            else DisplayListObjects();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        private void DisplayObjectGroup()
        {
            if (objects == null) return;
            DisplayObjects();
            DisplayDetail();
            UpdateDragAndDrop();
        }

        private void DisplayTopBar()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(!_isShowTabs ? StrShowTab : StrHideTab, _s.buttonStyle, _s.expandWidthFalse))
            {
                _isShowTabs = !_isShowTabs;
            }

            searchText = EditorGUILayout.TextField(searchText, _s.toolbarSeachTextField);
            GUILayout.Label($"{filterList.Count.ToString()}/{objects.Count.ToString()}", GUILayout.ExpandWidth(false));
            if (GUILayout.Button(StrReset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                objects.Clear();
                filterList.Clear();
                Focus();
                Save();
            }

            if (_oldSearchText != searchText)
            {
                _oldSearchText = searchText;
                Filter();
            }

            if (!_isEditMode)
            {
                if (GUILayout.Button(StrEdit, _s.buttonStyle, _s.expandWidthFalse))
                {
                    _isEditMode = true;
                }
            }
            else
            {
                if (GUILayout.Button(StrDone, _s.buttonStyle, _s.expandWidthFalse))
                {
                    _isEditMode = false;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DisplayDetail()
        {
        }

        private void DisplayReorderList()
        {
            GUILayout.BeginVertical();
            _objectsReorderableList.DoLayoutList();
            GUILayout.EndVertical();
        }

        private void DisplayListObjects()
        {
            GUILayout.BeginVertical();
            if (filterList.Count == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Label(StrDrag, _s.textEmpty);
                EditorGUI.EndDisabledGroup();
            }

            foreach (var o in filterList)
            {
                GUILayout.BeginHorizontal();
                if (o.previewTexture == null)
                {
                    o.previewTexture = AssetPreview.GetAssetPreview(o.obj);
                    if (o.previewTexture == null)
                    {
                        o.previewTexture = AssetPreview.GetMiniThumbnail(o.obj);
                    }
                }

                GUILayout.Label(o.previewTexture, _s.previewTexture);
                var lastPreviewRect = GUILayoutUtility.GetLastRect();
                if (lastPreviewRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        GUIUtility.hotControl = 0;
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.objectReferences = new[] { o.obj };
                        DragAndDrop.SetGenericData("DRAG_ID", o.obj);
                        DragAndDrop.StartDrag("A");
                    }
                }

                if (GUILayout.Button(new GUIContent(o.GetDisplayName()), _s.buttonStyle, null))
                {
                    tabs[_tabIndex].selected = o;
                    tabs[_tabIndex].editor = Editor.CreateEditor(o.obj);
                    if (o.location == CacheObjectLocation.Scene || o.location == CacheObjectLocation.Prefab)
                    {
                        o.Ping();
                    }
                    else if (o.obj is DefaultAsset)
                    {
                        o.Ping();
                        //EditorCacheHelper.ShowFolderContents(o.obj.GetInstanceID());
                    }
                    else
                    {
                        AssetDatabase.OpenAsset(o.obj);
                    }
                }

                if (GUILayout.Button("P", _s.expandWidthFalse))
                {
                    o.Ping();
                }

                if (GUILayout.Button("-", _s.expandWidthFalse))
                {
                    objects.Remove(o);
                    filterList.Remove(o);
                    Save();
                    Focus();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    break;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private void ResizeScrollView()
        {
            EditorGUIUtility.AddCursorRect(_cursorChangeRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && _cursorChangeRect.Contains(Event.current.mousePosition))
            {
                _isResize = true;
            }

            if (_isResize)
            {
                _currentTabViewWidth = Event.current.mousePosition.x;
                _currentTabViewWidth = Mathf.Clamp(_currentTabViewWidth, 100, 200);
                _cursorChangeRect.Set(_currentTabViewWidth, _cursorChangeRect.y, _cursorChangeRect.width,
                    _cursorChangeRect.height);
            }

            if (Event.current.type == EventType.MouseUp)
                _isResize = false;
        }

        private void DisplayTabs()
        {
            GUILayout.BeginVertical(_s.ProjectBrowserIconAreaBg, GUILayout.Width(_currentTabViewWidth));
            _tabsScrollPosition = GUILayout.BeginScrollView(_tabsScrollPosition);
            if (!_isEditMode)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var buttonStyle = i == _tabIndex ? _s.SelectionRect : _s.RectangleToolSelection;

                    if (_isEditMode && i == _tabIndex)
                    {
                        GUILayout.BeginVertical();
                        tabs[i].tabName = EditorGUILayout.TextField(tabs[i].tabName, _s.expandWidth200);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (GUILayout.Button($"{tabs[i].tabName} ({tabs[i].list.Count})", buttonStyle,
                                GUILayout.Width(_currentTabViewWidth - 5)))
                        {
                            _tabIndex = i;
                        }
                    }
                }
            }
            else
            {
                _tabsReorderableList.DoLayoutList();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            ResizeScrollView();
        }

        private void AddNewTab()
        {
            if (tabs.Count == 0)
            {
                tabs.Add(new CacheTabInfo { id = 0, tabName = "New tab", list = new List<CacheObjectInfo>() });
            }
            else
            {
                tabs.Add(new CacheTabInfo
                    { id = tabs.Max(s => s.id) + 1, tabName = "New tab", list = new List<CacheObjectInfo>() });
            }

            Save();
        }

        private void Filter()
        {
            if (string.IsNullOrEmpty(searchText))
            {
                filterList = objects;
            }
            else
            {
                var temp = searchText.ToLower();
                filterList = objects.Where(s => s.Name.ToLower().Contains(temp)).ToList();
            }

            _objectsReorderableList =
                new ReorderableList(filterList, typeof(CacheObjectInfo), true, false, false, false);
            _objectsReorderableList.drawElementCallback += DrawObjectElementCallback;
            _objectsReorderableList.onChangedCallback += _ => { Save(); };
        }

        private void DrawObjectElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var data = filterList[index];
            var avatarRect = new Rect(rect.x, rect.y, rect.height, rect.height);
            GUI.Label(avatarRect, data.previewTexture);
            var nameRect = new Rect(rect.x + rect.height + 3, rect.y, rect.width - rect.height - 3, rect.height);
            GUI.Label(nameRect, data.GetDisplayName());
        }

        private void DrawTabElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var data = tabs[index];
            var nameRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            data.tabName = GUI.TextField(nameRect, data.tabName);
        }

        void UpdateDragAndDrop()
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
                {
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        var currentPrefab = PrefabStageUtility.GetCurrentPrefabStage();
                        // Debug.Log("- " + obj);
                        string assetPath = obj.name;
                        GetScenePath((obj as GameObject).transform, ref assetPath);
                        var item = new CacheObjectInfo
                        {
                            Name = obj.name,
                            path = assetPath,
                        };

                        CacheObjectLocation location = CacheObjectLocation.Assets;
                        if (currentPrefab != null)
                        {
                            item.prefabPath = currentPrefab.prefabAssetPath;
                            item.location = CacheObjectLocation.Prefab;
                        }
                        else
                        {
                            item.location = CacheObjectLocation.Scene;
                        }

                        if (objects.Find(s => s.obj == item.obj) == null)
                        {
                            objects.Add(item);
                            Save();
                        }
                        else if (objects.Find(s => s.path == item.path) == null)
                        {
                            objects.Add(item);
                            Save();
                        }
                    }

                    Filter();
                }
                else if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
                {
                    foreach (string path in DragAndDrop.paths)
                    {
                        Debug.Log("- " + path);
                    }
                }
                else if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        if (objects.Find(s => s.obj == obj) == null)
                        {
                            objects.Add(new CacheObjectInfo()
                            {
                                Name = obj.name,
                                location = CacheObjectLocation.Assets,
                                obj = obj,
                            });
                            Save();
                        }
                    }

                    Filter();
                }
            }
        }

        private void GetScenePath(Transform obj, ref string path)
        {
            var parent = obj.parent;
            if (parent != null)
            {
                path = parent.name + "/" + path;
                GetScenePath(parent, ref path);
            }
        }

        [Serializable]
        public class CacheObjectInfo
        {
            public Object obj;
            public string path;
            public string prefabPath;
            public string Name;
            public CacheObjectLocation location;
            public Texture2D previewTexture;

            public string GetDisplayName()
            {
                return $"{GetPrefix()} {Name}";
            }

            public string GetPrefix()
            {
                if (location == CacheObjectLocation.Assets)
                    return "A:";
                if (location == CacheObjectLocation.Scene)
                    return "S:";
                return "P:";
            }

            public void Ping()
            {
                if (location == CacheObjectLocation.Assets)
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                else if (location == CacheObjectLocation.Prefab)
                {
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)));
                    var rootGameObjects = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

                    var arrayPath = path.Split('/').ToList();
                    arrayPath.RemoveAt(0);
                    arrayPath.RemoveAt(0);
                    var newPath = string.Join("/", arrayPath);
                    var obj = rootGameObjects.transform.Find(newPath);
                    if (obj != null)
                    {
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
                else if (location == CacheObjectLocation.Scene)
                {
                    if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                    {
                        PrefabUtility.UnloadPrefabContents(
                            PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot);
                    }

                    var arrayPath = path.Split('/').ToList();
                    arrayPath.RemoveAt(0);
                    var newPath = string.Join("/", arrayPath);

                    if (arrayPath.Count == 0)
                        newPath = path;

                    Transform obj = null;
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        bool check = false;
                        var scene = SceneManager.GetSceneAt(i);
                        var rootGameObjects = scene.GetRootGameObjects().ToList();
                        foreach (var gameObject in rootGameObjects)
                        {
                            obj = gameObject.transform.Find(newPath);
                            if (obj != null)
                            {
                                check = true;
                                break;
                            }
                        }

                        if (check)
                            break;

                        var tempObj = rootGameObjects.Find(s => s.name == newPath);
                        if (tempObj != null)
                        {
                            obj = tempObj.transform;
                            break;
                        }
                    }

                    if (obj != null)
                    {
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }
        }

        private void Save()
        {
            string data = JsonUtility.ToJson(new EditorCacheSave { tabs = tabs });
            EditorPrefs.SetString(Application.dataPath + StrSaveName, data);
        }

        private void Read()
        {
            var data = EditorPrefs.GetString(Application.dataPath + StrSaveName);
            EditorCacheSave editorCacheSave;
            if (string.IsNullOrEmpty(data))
            {
                editorCacheSave = new EditorCacheSave
                    { tabs = new List<CacheTabInfo> { new() { id = 0, tabName = "Common" } } };
            }
            else
            {
                editorCacheSave = JsonUtility.FromJson<EditorCacheSave>(data);
            }

            tabs.Clear();
            tabs.AddRange(editorCacheSave.tabs);
            InitReOrderTabList();
        }

        [Serializable]
        public class EditorCacheSave
        {
            public List<CacheTabInfo> tabs = new();
        }

        public enum CacheObjectLocation
        {
            Assets,
            Scene,
            Prefab,
        }

        [Serializable]
        public class CacheTabInfo
        {
            public int id;
            public string tabName;
            public List<CacheObjectInfo> list;
            public CacheObjectInfo selected;
            public Editor editor;
        }
    }

    public class EditorCacheStyle
    {
        public readonly GUILayoutOption expandWidthFalse = GUILayout.ExpandWidth(false);
        public readonly GUILayoutOption expandWidth200 = GUILayout.Width(100);
        public readonly GUILayoutOption expandWidth210 = GUILayout.Width(110);
        public readonly GUIStyle previewTexture;
        public readonly GUIStyle buttonStyle;
        public readonly GUIStyle textEmpty;
        public readonly GUIStyle toolBar;

        public readonly GUIStyle buttonLeftSelected;
        public readonly GUIStyle buttonMidSelected;
        public readonly GUIStyle buttonRightSelected;

        public readonly GUIStyle toolbarSeachTextField;
        public readonly GUIStyle RectangleToolSelection;
        public readonly GUIStyle ProjectBrowserIconAreaBg;
        public readonly GUIStyle RectangleToolVBar;
        public readonly GUIStyle SelectionRect;

        public EditorCacheStyle()
        {
            toolbarSeachTextField = GUI.skin.GetStyle("ToolbarSeachTextField");
            ProjectBrowserIconAreaBg = GUI.skin.GetStyle("ProjectBrowserIconAreaBg");
            RectangleToolSelection = GUI.skin.GetStyle("RectangleToolSelection");
            RectangleToolVBar = GUI.skin.GetStyle("RectangleToolVBar");
            SelectionRect = GUI.skin.GetStyle("SelectionRect");
            previewTexture = new GUIStyle { fixedWidth = 20, fixedHeight = 20 };
            buttonLeftSelected = new GUIStyle(EditorStyles.miniButtonLeft);
            buttonMidSelected = new GUIStyle(EditorStyles.miniButtonMid);
            buttonRightSelected = new GUIStyle(EditorStyles.miniButtonRight);
            buttonLeftSelected.normal.textColor = buttonMidSelected.normal.textColor =
                buttonRightSelected.normal.textColor = Color.yellow;
            buttonLeftSelected.onHover.textColor = buttonMidSelected.onHover.textColor =
                buttonRightSelected.onHover.textColor = Color.yellow;

            buttonLeftSelected.focused.textColor = buttonMidSelected.focused.textColor =
                buttonRightSelected.focused.textColor = Color.yellow;

            buttonLeftSelected.fontStyle = buttonMidSelected.fontStyle =
                buttonRightSelected.fontStyle = FontStyle.Bold;

            ColorUtility.TryParseHtmlString("#363636", out var bgColor);
            buttonStyle = new GUIStyle("Button");
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            textEmpty = new GUIStyle(EditorStyles.label);
            textEmpty.alignment = TextAnchor.MiddleCenter;
            toolBar = new GUIStyle(EditorStyles.toolbar);
            toolBar.fixedHeight = 60;
        }
    }

    public static class EditorCacheHelper
    {
        public static void ShowFolderContents(int folderInstanceID)
        {
            Assembly editorAssembly = typeof(Editor).Assembly;
            Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");
            MethodInfo showFolderContents = projectBrowserType.GetMethod(
                "ShowFolderContents", BindingFlags.Instance | BindingFlags.NonPublic);
            Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);
            if (projectBrowserInstances.Length > 0)
            {
                for (int i = 0; i < projectBrowserInstances.Length; i++)
                    ShowFolderContentsInternal(projectBrowserInstances[i], showFolderContents, folderInstanceID);
            }
            else
            {
                EditorWindow projectBrowser = OpenNewProjectBrowser(projectBrowserType);
                ShowFolderContentsInternal(projectBrowser, showFolderContents, folderInstanceID);
            }
        }

        private static void ShowFolderContentsInternal(Object projectBrowser, MethodInfo showFolderContents,
            int folderInstanceID)
        {
            SerializedObject serializedObject = new SerializedObject(projectBrowser);
            bool inTwoColumnMode = serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;

            if (!inTwoColumnMode)
            {
                MethodInfo setTwoColumns = projectBrowser.GetType().GetMethod(
                    "SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic);
                setTwoColumns.Invoke(projectBrowser, null);
            }

            bool revealAndFrameInFolderTree = true;
            showFolderContents.Invoke(projectBrowser, new object[] { folderInstanceID, revealAndFrameInFolderTree });
        }

        private static EditorWindow OpenNewProjectBrowser(Type projectBrowserType)
        {
            EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
            projectBrowser.Show();
            MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
            init.Invoke(projectBrowser, null);
            return projectBrowser;
        }
    }
}