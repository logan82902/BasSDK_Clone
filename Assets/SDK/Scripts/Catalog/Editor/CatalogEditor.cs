using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TriInspector;
using TriInspector.Editors;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ThunderRoad
{
    public class CatalogEditor : EditorWindow, IHasCustomMenu
    {
        private const float indentWidth = 20f;
        private const float minTreeWidth = 250f;
        public static GUIStyle customToolbarStyle => new(EditorStyles.toolbarButton) { fontSize = EditorStyles.toolbarButton.fontSize + 1, fixedHeight = EditorStyles.toolbarButton.fixedHeight * 1.2f, imagePosition = ImagePosition.ImageLeft };
        public static GUIStyle customDropdownStyle => new(EditorStyles.toolbarDropDown) { fontSize = EditorStyles.toolbarButton.fontSize + 1, alignment = TextAnchor.MiddleCenter, fixedHeight = EditorStyles.toolbarButton.fixedHeight * 1.2f, imagePosition = ImagePosition.ImageLeft };
        public float toolbarHeight => customToolbarStyle.CalcHeight(new GUIContent("Toolbar"), treePaneWidth);
        private bool resizing;
        private float treePaneWidth = 300f;
        private float treeResizeStartWidth = 250f;
        private float treeResizeStartX;

        private SearchField searchField;
        private string searchString;

        private Vector2 treeScrollPosition;

        private PaneMode currentPaneMode;
        private bool settingsOpen = false;

        private Vector2 leftScrollPosition;
        private object leftData;
        private JsonContainer leftContainerA;
        private Editor leftEditor;

        private Vector2 rightScrollPosition;
        private object rightData;
        private JsonContainer rightContainerA;
        private Editor rightEditor;

        private Side lastSetSide = Side.Right;

        private Dictionary<string, GameData> initialGameDatas;
        private Dictionary<string, CatalogData> loadedJsons;
        private float initialStoreStart = 0f;
        private List<string> storeInitialQueue;
        private SlowStoreState slowStoreState;
        private bool hasModsLoaded = false;
        private bool storing = false;
        private Dictionary<string, CatalogData> initialJsons;
        private TreeBranch rootBranch;
        private Dictionary<string, bool> openFoldouts;

        private ItemWizardContainer itemWizardContainer;

        private bool progressBarOpen = false;
        private string progressBarTitle = "";
        private float progressBarPercent = 0f;
        private string progressBarText = "";

        private enum SlowStoreState
        {
            NotStarted,
            Working,
            Finished
        }

        private enum PaneMode
        {
            SinglePane,
            TwoJSONs,
            ChangeView,
            TextView,
            ItemJSONWizard
        }

        private enum Icon
        {
            Clone,
            Delete,
            LoadJson,
            LoadJsonMods,
            New,
            OpenFolder,
            OpenScript,
            Refresh,
            Reset,
            Save,
            SaveCategory,
            SaveJson,
            Unload,
            View,
            Override,
        }

        private class TreeBranch
        {
            public delegate void BranchCreated(TreeBranch branch);
            public static event BranchCreated onNewBranch;

            public string branchPath;
            public List<TreeBranch> childBranches;
            public List<string> dataGUIDs;

            public TreeBranch(string branchPath, Dictionary<string, CatalogData> loadedJsons)
            {
                this.branchPath = branchPath;
                childBranches = new List<TreeBranch>();
                dataGUIDs = new List<string>();
                onNewBranch?.Invoke(this);
                dataGUIDs.OrderBy(catalogGUID => loadedJsons[catalogGUID]);
            }

            public static void CleanStaticEvent() => onNewBranch = null;
        }

        private static Texture2D GetIcon(Icon icon, float size)
        {
            Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/SDK/UI/CatalogEditorIcons/{icon.ToString().ToLowerInvariant()}.png");
            EditorGUIUtility.SetIconSize(new Vector2(size, size));
            return iconTexture;
        }

        [MenuItem("ThunderRoad (SDK)/Catalog Editor")]
        private static void Open()
        {
            CatalogEditor window = GetWindow<CatalogEditor>();
            window.titleContent = new GUIContent("Catalog Editor");
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
        }

        private void OnEnable()
        {
            searchField = new SearchField();
            currentPaneMode = PaneMode.SinglePane;
        }

        private void Update()
        {
            // return early if we try to update before these things have values set by Unity
            try
            {
                if (EditorStyles.toolbarButton == null || EditorStyles.toolbarDropDown == null) return;
            } catch (Exception)
            {
                return;
            }
            if (!storing && storeInitialQueue != null && storeInitialQueue.Count > 0)
            {
                slowStoreState = SlowStoreState.Working;
                storing = true;
                _ = Task.Run(() =>
                {
                    CreateInitialJSON(loadedJsons[storeInitialQueue[0]]);
                    storeInitialQueue.RemoveAt(0);
                    storing = false;
                    if (storeInitialQueue.Count == 0)
                    {
                        slowStoreState = SlowStoreState.Finished;
                    }
                });
            }
            if (slowStoreState == SlowStoreState.Finished)
            {
                slowStoreState = SlowStoreState.NotStarted;
                int initialCount = initialJsons.Count;
                int loadedCount = loadedJsons.Count;
                string message = $"Finished initial JSON cloning, took {Time.realtimeSinceStartup - initialStoreStart} seconds, initial JSONs cloned: {initialCount} ({loadedCount} total JSONs, if these numbers do not match an error has occurred!)";
                if (initialCount != loadedCount)
                {
                    Debug.LogError(message);
                }
                else
                {
                    Debug.Log(message);
                }
            }
            if (progressBarOpen)
            {
                EditorUtility.DisplayProgressBar(progressBarTitle, progressBarText, progressBarPercent);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
            minSize = new Vector2(treePaneWidth + HorizontalToolbar(false), 0f);
            if (resizing)
            {
                Repaint();
            }
            if (Catalog.IsJsonLoaded())
            {
                if (loadedJsons.IsNullOrEmpty())
                {
                    InitializeJSONsForEditing();
                }
                if (rootBranch == null)
                {
                    BuildTree();
                }
            }
            else
            {
                searchField = new SearchField();
                loadedJsons = null;
                rootBranch = null;
                leftContainerA = null;
                leftEditor = null;
                rightContainerA = null;
                rightEditor = null;
                settingsOpen = false;
                openFoldouts = new Dictionary<string, bool>();
            }
        }

        private void OnDisable()
        {
            searchField = null;
        }

        private void OnGUI()
        {
            if (currentPaneMode != PaneMode.ItemJSONWizard)
            {
                itemWizardContainer = null;
            }
            using (new GUILayout.HorizontalScope())
            {
                // tree side
                using (new GUILayout.VerticalScope(GUILayout.Width(treePaneWidth)))
                {
                    GUILayout.Space(4f);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(5f);
                        searchString = searchField.OnToolbarGUI(searchString);
                        GUILayout.Space(5f);
                    }
                    GUILayout.Space(1f);
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.Space(15f, false);
                        Rect scrollRect = GUILayoutUtility.GetRect(treePaneWidth - 15f, position.height - toolbarHeight);
                        Rect viewRect = new Rect(0f, treeScrollPosition.y, scrollRect.width - 15f, scrollRect.height);

                        bool onlyItems = currentPaneMode == PaneMode.ItemJSONWizard;
                        bool searching = !loadedJsons.IsNullOrEmpty() && !searchString.IsNullOrEmptyOrWhitespace();

                        float elementHeight = EditorGUIUtility.singleLineHeight * 1.5f + 2f;
                        List<CatalogData> searchResults = searching ? GetSearchResults(onlyItems) : null;
                        Rect contentRect = new Rect(0f, 0f, scrollRect.width - 15f, searching ? searchResults.Count * elementHeight : GetExpandedFoldoutHeight(elementHeight, onlyItems));
                        treeScrollPosition = GUI.BeginScrollView(scrollRect, treeScrollPosition, contentRect, false, true);
                        if (Catalog.IsJsonLoaded())
                        {
                            if (searching)
                            {
                                DrawSearchResults(viewRect, searchResults);
                            }
                            else
                            {
                                DrawTreeView(viewRect, onlyItems);
                            }
                        }
                        GUI.EndScrollView();
                    }
                }

                ResizeFunction();

                // content side
                using (new GUILayout.VerticalScope())
                {
                    HorizontalToolbar(true);
                    EditorGUI.DrawRect(new Rect(0f, toolbarHeight, position.width, 1f), new Color(0f, 0f, 0f, 0.25f));
                    GUILayout.Space(1f);
                    DrawDataPanes();
                }
            }
        }

        private float GetExpandedFoldoutHeight(float elementHeight, bool onlyItems)
        {
            float result = 0f;
            float RecursiveAddHeight(TreeBranch branch)
            {
                float branchResult = elementHeight;
                if (!openFoldouts.TryGetValue(branch.branchPath, out bool isOpen) || !isOpen)
                {
                    return branchResult;
                }
                foreach (TreeBranch child in branch.childBranches)
                {
                    branchResult += RecursiveAddHeight(child);
                }
                branchResult += elementHeight * branch.dataGUIDs.Count;
                return branchResult;
            }
            if (rootBranch == null) return 2 * elementHeight;
            if (openFoldouts.IsNullOrEmpty()) openFoldouts = new Dictionary<string, bool>();
            if (!onlyItems)
            {
                result = 2 * elementHeight;
                foreach (TreeBranch rootChild in rootBranch.childBranches)
                {
                    result += RecursiveAddHeight(rootChild);
                }
            }
            else
            {
                foreach (TreeBranch branch in rootBranch.childBranches)
                {
                    if (branch.branchPath.Contains("Items"))
                    {
                        foreach (TreeBranch childBranch in branch.childBranches)
                        {
                            result += RecursiveAddHeight(childBranch);
                        }
                    }
                }
            }
            return result;
        }

        private List<CatalogData> GetSearchResults(bool onlyItems = false)
        {
            List<CatalogData> results = new();
            foreach (CatalogData catalogData in loadedJsons.Values)
            {
                // not sure how this could happen, but if it does, print an error and skip it
                if (catalogData == null)
                {
                    Debug.LogError("Catalog Editor: Null catalog data in loaded JSONs!");
                    continue;
                }
                if (onlyItems && catalogData is not ItemData)
                {
                    continue;
                }
                if (searchString.IsNullOrEmptyOrWhitespace() || catalogData.id.ToLowerInvariant().Contains(searchString.ToLowerInvariant()))
                {
                    results.Add(catalogData);
                }
            }
            return results;
        }

        private void DrawSearchResults(Rect viewRect, List<CatalogData> searchResults)
        {
            bool firstElement = true;
            float height = 0f;
            foreach (CatalogData catalogData in searchResults)
            {
                if (catalogData.isOverrider) continue;
                DrawCatalogEntry(catalogData, 0, !firstElement, height, viewRect);
                height += (EditorGUIUtility.singleLineHeight * 1.5f + 2f) * (1 + (catalogData.overriderDatas?.Count ?? 0));
                firstElement = false;
            }
        }

        private void DrawTreeView(Rect viewRect, bool onlyItems = false)
        {
            GUIStyle buttonStyle = new(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f };
            GUIStyle groupStyle = new(EditorStyles.toolbarDropDown) { alignment = TextAnchor.MiddleLeft, fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f };
            GUIStyle groupStyleBold = new(EditorStyles.toolbarDropDown) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f };
            Rect settingsRect = new(0f, 0f, treePaneWidth - 30f, buttonStyle.fixedHeight);
            if (GUI.Button(settingsRect, "Settings", buttonStyle))
            {
                currentPaneMode = PaneMode.SinglePane;
                SetSide(Side.Left, ThunderRoadSettings.current);
                settingsOpen = true;
                leftContainerA = null;
                rightEditor = null;
                rightContainerA = null;
            }
            float lineX = 0f;
            float height = buttonStyle.fixedHeight;
            float lineWidth = treePaneWidth - 30f;

            void DrawSeparator(int indentLevel)
            {
                Rect darkLine = new(lineX + indentLevel * indentWidth, height, lineWidth - indentLevel * indentWidth, 1f);
                Rect lightLine = new(darkLine) { y = darkLine.y + 1 };
                EditorGUI.DrawRect(darkLine, new Color(0f, 0f, 0f, 0.1f));
                EditorGUI.DrawRect(lightLine, new Color(1f, 1f, 1f, 0.25f));
                height += 2f;
            }

            DrawSeparator(0);
            if (openFoldouts == null) openFoldouts = new Dictionary<string, bool>();
            if (Catalog.IsJsonLoaded() && rootBranch != null)
            {
                if (!onlyItems)
                {
                    Rect gameButtonRect = new Rect(0f, height, treePaneWidth - 30f, buttonStyle.fixedHeight);
                    if (GUI.Button(gameButtonRect, "Game", buttonStyle))
                    {
                        GUI.FocusControl(null);
                        SetSide(Side.Right, initialGameDatas[GameData.defaultEditorId]);
                        SetSide(Side.Left, Catalog.gameData);
                    }
                    height += buttonStyle.fixedHeight;
                    DrawSeparator(0);
                    if ((Catalog.gameData.overriders?.Count ?? 0) > 0)
                    {
                        foreach (GameData overrider in Catalog.gameData.overriders)
                        {
                            Rect overriderButtonRect = new Rect(indentWidth, height, treePaneWidth - (30f + indentWidth), buttonStyle.fixedHeight);
                            if (GUI.Button(overriderButtonRect, $"Game ({overrider.GetFolderName()})", buttonStyle))
                            {
                                GUI.FocusControl(null);
                                SetSide(Side.Right, initialGameDatas[overrider.editorId]);
                                SetSide(Side.Left, overrider);
                            }
                            height += buttonStyle.fixedHeight;
                            DrawSeparator(1);
                        }
                    }
                }

                void RecursiveTreeBranchDraw(TreeBranch branch, int indent)
                {
                    string branchName = branch.branchPath.Contains('/') ? branch.branchPath.Split('/')[^1] : branch.branchPath;
                    if (!openFoldouts.TryGetValue(branch.branchPath, out bool isOpen)) isOpen = false;
                    float indentSpace = indent * indentWidth;
                    Rect branchRect = new Rect(indentSpace, height, treePaneWidth - (30f + indentSpace), buttonStyle.fixedHeight);
                    isOpen = GUI.Button(branchRect, branchName, isOpen ? groupStyleBold : groupStyle) ? !isOpen : isOpen;
                    height += buttonStyle.fixedHeight;
                    DrawSeparator(indent);
                    openFoldouts[branch.branchPath] = isOpen;
                    if (isOpen)
                    {
                        int childIndent = indent + 1;
                        foreach (TreeBranch child in branch.childBranches)
                        {
                            RecursiveTreeBranchDraw(child, childIndent);
                        }
                        foreach (string dataGUID in branch.dataGUIDs)
                        {
                            if (loadedJsons.TryGetValue(dataGUID, out CatalogData catalogData))
                            {
                                if (catalogData.isOverrider) continue;
                                DrawCatalogEntry(catalogData, childIndent, true, height, viewRect);
                                height += (buttonStyle.fixedHeight + 2f) * (1 + (catalogData.overriderDatas?.Count ?? 0));
                            }
                        }
                    }
                }

                if (onlyItems)
                {
                    foreach (TreeBranch branch in rootBranch.childBranches)
                    {
                        if (branch.branchPath.Contains("Items"))
                        {
                            foreach (TreeBranch childBranch in branch.childBranches)
                            {
                                RecursiveTreeBranchDraw(childBranch, 0);
                            }
                        }
                    }
                }
                else
                {
                    foreach (TreeBranch branch in rootBranch.childBranches)
                    {
                        RecursiveTreeBranchDraw(branch, 0);
                    }
                }
            }
        }

        private void DrawCatalogEntry(CatalogData catalogData, int indent, bool drawSeparator, float height, Rect viewRect)
        {
            GUIStyle buttonStyle = new(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f, clipping = TextClipping.Clip, imagePosition = ImagePosition.ImageLeft };
            GUIStyle lrStyle = new(EditorStyles.toolbarButton) { fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f };
            float indentSpace = indent * indentWidth;
            float buttonWidth = treePaneWidth - (30f + indentSpace);

            void DrawSeparator()
            {
                Rect darkLine = new Rect(indentSpace, height + buttonStyle.fixedHeight, buttonWidth, 1f);
                Rect lightLine = new Rect(darkLine) { y = darkLine.y + 1 };
                EditorGUI.DrawRect(darkLine, new Color(0f, 0f, 0f, 0.1f));
                EditorGUI.DrawRect(lightLine, new Color(1f, 1f, 1f, 0.25f));
            }
            Rect buttonRect = new Rect(indentSpace, height, buttonWidth, buttonStyle.fixedHeight);
            if (!buttonRect.Overlaps(viewRect))
            {
                GUI.Box(buttonRect, GUIContent.none);
                return;
            }
            EditorGUIUtility.SetIconSize(new Vector2(buttonStyle.fontSize, buttonStyle.fontSize));
            if (currentPaneMode == PaneMode.TwoJSONs)
            {
                buttonRect.width -= 40f;
                if (GUI.Button(buttonRect, catalogData.GetCatalogEditorButtonContent(), buttonStyle))
                {
                    GUI.FocusControl(null);
                    if (!initialJsons.ContainsKey(catalogData.editorGuid)) CreateInitialJSON(catalogData);
                    SetSide(lastSetSide == Side.Right ? Side.Left : Side.Right, catalogData);
                }
                Rect sideButton = new Rect(buttonRect.x + buttonRect.width, buttonRect.y, 20f, buttonRect.height);
                if (GUI.Button(sideButton, "L", lrStyle))
                {
                    GUI.FocusControl(null);
                    if (!initialJsons.ContainsKey(catalogData.editorGuid)) CreateInitialJSON(catalogData);
                    SetSide(Side.Left, catalogData);
                }
                sideButton.x += 20f;
                if (GUI.Button(sideButton, "R", lrStyle))
                {
                    GUI.FocusControl(null);
                    if (!initialJsons.ContainsKey(catalogData.editorGuid)) CreateInitialJSON(catalogData);
                    SetSide(Side.Right, catalogData);
                }
            }
            else
            {
                if (GUI.Button(buttonRect, catalogData.GetCatalogEditorButtonContent(), buttonStyle))
                {
                    GUI.FocusControl(null);
                    if (!initialJsons.ContainsKey(catalogData.editorGuid)) CreateInitialJSON(catalogData);
                    if (currentPaneMode != PaneMode.ItemJSONWizard && initialJsons.TryGetValue(catalogData.editorGuid, out CatalogData initialData))
                    {
                        SetSide(Side.Right, initialData);
                    }
                    else if (catalogData is ItemData itemData && itemWizardContainer != null)
                    {
                        itemWizardContainer.baseData = itemData;
                    }
                    SetSide(Side.Left, catalogData);
                }
            }
            if (drawSeparator) DrawSeparator();
            if (catalogData.overriderDatas.IsNullOrEmpty()) return;
            int overriderIndex = 0;
            foreach (CatalogData overrider in catalogData.overriderDatas)
            {
                DrawCatalogEntry(loadedJsons[overrider.editorGuid], indent + 1, true, height + ((buttonStyle.fixedHeight + 2f) * (1 + overriderIndex)), viewRect);
                overriderIndex++;
            }
        }

        private void SetSide(Side side, object data, bool setLast = true)
        {
            settingsOpen = false;
            if (setLast) lastSetSide = side;
            if (side == Side.Left)
            {
                leftData = data;
                ScriptableObject container = CreateContainer(data);
                if (container is JsonContainer jsonContainer)
                {
                    leftContainerA = jsonContainer;
                }
                leftEditor = container != null ? TriEditor.CreateEditor(container) : null;
            }
            else
            {
                rightData = data;
                ScriptableObject container = CreateContainer(data);
                if (container is JsonContainer jsonContainer)
                {
                    rightContainerA = jsonContainer;
                }
                rightEditor = container != null ? TriEditor.CreateEditor(container) : null;
            }
        }

        private ScriptableObject CreateContainer(object data)
        {
            if (data == null) return null;
            if (data is ScriptableObject scriptableObject)
            {
                return scriptableObject;
            }
            else
            {
                if (data is CatalogData catalogData)
                {
                    CatalogDataContainer catalogDataContainer = CreateInstance<CatalogDataContainer>();
                    catalogDataContainer.catalogData = catalogData;
                    return catalogDataContainer;
                }
                else if (data is GameData gameData)
                {
                    GameDataContainer gameDataContainer = CreateInstance<GameDataContainer>();
                    gameDataContainer.gameData = gameData;
                    return gameDataContainer;
                }
            }
            return null;
        }

        private Editor GetSide(Side side)
        {
            if (side == Side.Left)
            {
                if (leftEditor == null || leftContainerA == null)
                {
                    SetSide(Side.Left, leftData, false);
                }
                return leftEditor;
            }
            else
            {
                if (rightEditor == null || rightContainerA == null)
                {
                    SetSide(Side.Right, rightData, false);
                }
                return rightEditor;
            }
        }

        private void ResizeFunction()
        {
            Rect separatorRect = GUILayoutUtility.GetLastRect();
            separatorRect.xMin = separatorRect.xMax + 1;
            separatorRect.xMax += 2;
            //GUI.Box(separatorRect, "");
            separatorRect.xMin -= 1;
            separatorRect.xMax += 5;
            if (resizing)
            {
                EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.ResizeHorizontal);
                treePaneWidth = Mathf.Clamp(treeResizeStartWidth + (Event.current.mousePosition.x - treeResizeStartX), minTreeWidth, minTreeWidth * 3f);
                if (Event.current.type == EventType.MouseUp)
                {
                    resizing = false;
                }
            }
            else if (separatorRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.ResizeHorizontal);
                if (Event.current.type == EventType.MouseDown)
                {
                    resizing = true;
                    treeResizeStartWidth = treePaneWidth;
                    treeResizeStartX = Event.current.mousePosition.x;
                }
            }
        }

        private float HorizontalToolbar(bool draw)
        {
            float toolbarWidth = 0f;
            int widthAddition = 20;
            if (draw) GUILayout.BeginHorizontal(GUILayout.Height(toolbarHeight));
            // toolbar
            GUIContent viewButtonContent = new(" View", GetIcon(Icon.View, customDropdownStyle.fontSize), "Change Catalog Editor view configuration");
            float viewButtonWidth = customDropdownStyle.CalcSize(viewButtonContent).x + widthAddition;
            toolbarWidth += viewButtonWidth;
            if (draw && GUILayout.Button(viewButtonContent, customDropdownStyle, GUILayout.Width(viewButtonWidth)))
            {
                GenericMenu viewMenu = new GenericMenu();
                viewMenu.AddItem(new GUIContent("One pane"), currentPaneMode == PaneMode.SinglePane, SetPaneStyle, PaneMode.SinglePane);
                if (!settingsOpen)
                {
                    viewMenu.AddItem(new GUIContent("Two JSONs"), currentPaneMode == PaneMode.TwoJSONs, SetPaneStyle, PaneMode.TwoJSONs);
                    viewMenu.AddItem(new GUIContent("Change View"), currentPaneMode == PaneMode.ChangeView, SetPaneStyle, PaneMode.ChangeView);
                    viewMenu.AddItem(new GUIContent("Text Output View"), currentPaneMode == PaneMode.TextView, SetPaneStyle, PaneMode.TextView);
                }
                else
                {
                    viewMenu.AddDisabledItem(new GUIContent("Two JSONs"), currentPaneMode == PaneMode.TwoJSONs);
                    viewMenu.AddDisabledItem(new GUIContent("Change View"), currentPaneMode == PaneMode.ChangeView);
                    viewMenu.AddDisabledItem(new GUIContent("Text Output View"), currentPaneMode == PaneMode.TextView);
                }
                if (Catalog.IsJsonLoaded())
                {
                    viewMenu.AddItem(new GUIContent("Item JSON Wizard"), currentPaneMode == PaneMode.ItemJSONWizard, SetPaneStyle, PaneMode.ItemJSONWizard);
                }
                else
                {
                    viewMenu.AddDisabledItem(new GUIContent("Item JSON Wizard"), currentPaneMode == PaneMode.ItemJSONWizard);
                }
                viewMenu.ShowAsContext();
            }
            if (!Catalog.IsJsonLoaded() || Application.isPlaying) GUI.enabled = false;
            string newButtonTooltip = "Create a new catalog object";
            if (!hasModsLoaded)
            {
                GUI.enabled = false;
                newButtonTooltip = "Create new is only enabled if mods are loaded";
            }
            var newButtonContent = new GUIContent(" New", GetIcon(Icon.New, customToolbarStyle.fontSize), newButtonTooltip);
            float newButtonWidth = customToolbarStyle.CalcSize(newButtonContent).x + widthAddition;
            toolbarWidth += newButtonWidth;
            if (draw && GUILayout.Button(newButtonContent, customToolbarStyle, GUILayout.Width(newButtonWidth)))
            {
                GenericMenu addNewMenu = new GenericMenu();
                Assembly defaultAssembly = typeof(CatalogData).Assembly;
                foreach (Type type in defaultAssembly.GetTypes())
                {
                    //ignore abstract classes
                    if (type.IsAbstract) continue;
                    if (!type.IsSubclassOf(typeof(CatalogData))) continue;
                    string typePath = type.Name;
                    Type parent = type.BaseType;
                    while (true)
                    {
                        if (parent == typeof(CatalogData)) break;
                        if (parent.GetCustomAttribute<SkipInTypeDropdownAttribute>() == null) typePath = $"{parent.Name}/{typePath}";
                        parent = parent.BaseType;
                    }
                    typePath = Regex.Replace(typePath, @"`\d+", "");
                    addNewMenu.AddItem(new GUIContent(typePath), false, CreateNewData, type);
                }
                bool addedSeparator = false;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // skip defauly assembly so we don't add duplicates
                    if (assembly == defaultAssembly) continue;
                    foreach (Type type in assembly.GetTypes())
                    {
                        //ignore abstract classes
                        if (type.IsAbstract) continue;
                        if (!type.IsSubclassOf(typeof(CatalogData))) continue;
                        string typePath = type.Name;
                        Type parent = type.BaseType;
                        while (true)
                        {
                            if (parent == typeof(CatalogData)) break;
                            if (parent.GetCustomAttribute<SkipInTypeDropdownAttribute>() == null) typePath = $"{parent.Name}/{typePath}";
                            parent = parent.BaseType;
                        }
                        typePath = $"{assembly.GetName().Name}/{Regex.Replace(typePath, @"`\d+", "")}";
                        // we want to have a separator between the default assembly and any additional assemblies, but we should only show it if there's a least one entry there
                        if (!addedSeparator)
                        {
                            addNewMenu.AddSeparator("");
                            addedSeparator = true;
                        }
                        addNewMenu.AddItem(new GUIContent(typePath), false, CreateNewData, type);
                    }
                }
                addNewMenu.ShowAsContext();
            }
            GUI.enabled = Catalog.IsJsonLoaded() && !Application.isPlaying;
            if (!hasModsLoaded) GUI.enabled = false;
            GUIContent saveButtonContent = new(" Save All JSON", GetIcon(Icon.SaveJson, customToolbarStyle.fontSize), "Save all json to StreamingAssets");
            float saveButtonWidth = customToolbarStyle.CalcSize(saveButtonContent).x + widthAddition;
            toolbarWidth += saveButtonWidth;
            if (draw && GUILayout.Button(saveButtonContent, customToolbarStyle, GUILayout.Width(saveButtonWidth)))
            {
                Catalog.SaveAllJson();
                BuildTree();
            }
            GUIContent refreshButtonContent = new(" Refresh", GetIcon(Icon.Refresh, customToolbarStyle.fontSize), "Refresh the tree view and Catalog");
            float refreshButtonWidth = customToolbarStyle.CalcSize(refreshButtonContent).x + widthAddition;
            toolbarWidth += refreshButtonWidth;
            if (draw && GUILayout.Button(refreshButtonContent, customToolbarStyle, GUILayout.Width(refreshButtonWidth)))
            {
                if (Catalog.gameData != null)
                {
                    Catalog.Refresh();
                    BuildTree();
                }
            }
            GUI.enabled = !Application.isPlaying;
            int loadJSONs = 0;
            string loadButtonText = " Load JSON";
            loadButtonText = " Load JSON (Read-only)";
            GUIContent loadButtonContent = new(loadButtonText, GetIcon(Icon.LoadJson, customToolbarStyle.fontSize), "Load JSON files from StreamingAssets");
            float loadButtonWidth = customToolbarStyle.CalcSize(loadButtonContent).x + widthAddition;
            toolbarWidth += loadButtonWidth;
            if (draw && GUILayout.Button(loadButtonContent, customToolbarStyle, GUILayout.Width(loadButtonWidth)))
            {
                loadJSONs = 1;
            }
            GUIContent loadModsButtonContent = new(" Load JSON with Mods", GetIcon(Icon.LoadJsonMods, customToolbarStyle.fontSize), "Load JSON files (including mod files) from StreamingAssets");
            float loadModsButtonWidth = customToolbarStyle.CalcSize(loadModsButtonContent).x + widthAddition;
            toolbarWidth += loadModsButtonWidth;
            if (draw && GUILayout.Button(loadModsButtonContent, customToolbarStyle, GUILayout.Width(loadModsButtonWidth)))
            {
                loadJSONs = 2;
            }
            if (loadJSONs > 0)
            {
                hasModsLoaded = false;
                storeInitialQueue = new List<string>();
                slowStoreState = SlowStoreState.NotStarted;
                storing = false;
                bool loadMods = loadJSONs == 2;
                if (loadMods) hasModsLoaded = true;
                // these events are all unsubscribed directly below
#pragma warning disable UDR0005 // Domain Reload Analyzer
                Catalog.OnJSONOpenProgress += CatalogJSONReadProgress;
                Catalog.OnJSONDeserializeProgress += CatalogJSONDeserializeProgress;
                Catalog.OnJSONLoadProgress += CatalogJSONLoadProgress;
                Catalog.OnRefreshProgress += CatalogJSONRefreshProgress;
#pragma warning restore UDR0005 // Domain Reload Analyzer
                Catalog.loadOverridesSeparately = loadMods;
                Catalog.EditorLoadAllJson(true, true, loadMods);
                Catalog.OnJSONOpenProgress -= CatalogJSONReadProgress;
                Catalog.OnJSONDeserializeProgress -= CatalogJSONDeserializeProgress;
                Catalog.OnJSONLoadProgress -= CatalogJSONLoadProgress;
                Catalog.OnRefreshProgress -= CatalogJSONRefreshProgress;
                Catalog.loadOverridesSeparately = false;
                InitializeJSONsForEditing();
                BuildTree();
                leftEditor = null;
                rightEditor = null;
                progressBarOpen = false;
            }
            if (!Catalog.IsJsonLoaded() || Application.isPlaying)
            {
                GUI.enabled = false;
            }
            GUIContent unloadJSON = new(" Unload JSON", GetIcon(Icon.Unload, customToolbarStyle.fontSize), "Unload all JSON");
            float unloadWidth = customToolbarStyle.CalcSize(unloadJSON).x + widthAddition;
            toolbarWidth += unloadWidth;
            if (draw && GUILayout.Button(unloadJSON, customToolbarStyle, GUILayout.Width(unloadWidth)))
            {
                ModManager.loadedMods.Clear();
                ModManager.gameModsLoaded = false;
                Catalog.Clear();
                storeInitialQueue = new List<string>();
                slowStoreState = SlowStoreState.NotStarted;
                storing = false;
                hasModsLoaded = false;
                loadedJsons = null;
                rootBranch = null;
                leftEditor = null;
                rightEditor = null;
            }
            if (draw) GUILayout.EndHorizontal();
            GUI.enabled = true;
            return toolbarWidth;
        }

        private void DrawDataPanes()
        {
            if (leftEditor == null && rightEditor == null)
            {
                EditorGUILayout.LabelField("No JSON open",
                    new GUIStyle(GUI.skin.label) { fontSize = GUI.skin.label.fontSize * 5, clipping = TextClipping.Overflow, alignment = TextAnchor.MiddleCenter },
                    GUILayout.ExpandHeight(true));
                return;
            }
            // settings open
            if (leftEditor != null && settingsOpen)
            {
                leftEditor?.OnInspectorGUI();
                return;
            }
            // dataDisplay
            Rect fullPaneRect = new Rect(treePaneWidth, toolbarHeight, position.width - treePaneWidth, position.height - toolbarHeight);
            Rect paneMinusScrollbar = new Rect(fullPaneRect) { width = fullPaneRect.width - 15f };
            if (currentPaneMode == PaneMode.ItemJSONWizard)
            {
                // top pane ("left")
                using (new GUILayout.VerticalScope(GUILayout.Height((position.height - toolbarHeight) / 2f)))
                {
                    if (itemWizardContainer == null) itemWizardContainer = CreateInstance<ItemWizardContainer>();
                    if (rightEditor == null) rightEditor = TriEditor.CreateEditor(itemWizardContainer);
                    if (leftData is CatalogData catalogData && catalogData is ItemData itemData)
                    {
                        itemWizardContainer.baseData = itemData;
                    }
                    rightEditor?.OnInspectorGUI();
                    MessageType message = itemWizardContainer.IsReadyForExport(out string text);
                    Color startColor = GUI.color;
                    GUI.color = message == MessageType.Error ? Color.red : (message == MessageType.Warning ? Color.yellow : startColor);
                    if (message != MessageType.None)
                    {
                        EditorGUILayout.HelpBox(text, message, true);
                    }
                    if (message != MessageType.Error && GUILayout.Button("Create Item JSON", GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f), GUILayout.ExpandWidth(true)))
                    {
                        CatalogData newCatalogData = itemWizardContainer.GetModifiedData();
                        Catalog.GetDataList(Category.Item).Add(newCatalogData);
                        newCatalogData.OnCatalogRefresh();
                        newCatalogData.editorGuid = GUID.Generate().ToString();
                        loadedJsons[newCatalogData.editorGuid] = newCatalogData;
                        loadedJsons.OrderBy(catalogData => catalogData.Value.id);
                        Catalog.SaveToJson(newCatalogData);
                        SetSide(Side.Left, newCatalogData);
                        BuildTree();
                    }
                    GUI.color = startColor;
                }
                // bottom pane ("right")
                using (var scrollScope = new GUILayout.ScrollViewScope(leftScrollPosition, false, true, GUILayout.Height((position.height - toolbarHeight) / 2f)))
                {
                    leftScrollPosition = scrollScope.scrollPosition;
                    TryCatchThrowUnityRequiredExceptions(() => DrawEditorPane(GetSide(Side.Left), leftData));
                }
            }
            else if (currentPaneMode != PaneMode.SinglePane)
            {
                // split
                using (new GUILayout.HorizontalScope())
                {
                    // left pane
                    float paneWidth = (position.width - treePaneWidth) / 2f;
                    using (var scrollScope = new GUILayout.ScrollViewScope(leftScrollPosition, false, true, GUILayout.Width(paneWidth)))
                    {
                        leftScrollPosition = scrollScope.scrollPosition;
                        TryCatchThrowUnityRequiredExceptions(() => DrawEditorPane(GetSide(Side.Left), leftData));
                    }
                    // right pane
                    using (var scrollScope = new GUILayout.ScrollViewScope(rightScrollPosition, false, true, GUILayout.Width(paneWidth)))
                    {
                        rightScrollPosition = scrollScope.scrollPosition;
                        if (currentPaneMode == PaneMode.TextView && leftData != null)
                        {
                            GUI.enabled = false;
                            string serializedData = "";
                            if (leftData is CatalogData catalogData)
                            {
                                serializedData = Catalog.ConvertDataToJson(catalogData);
                            }
                            else if (leftData is GameData gameData)
                            {
                                serializedData = GameDataToJsonString(gameData);
                            }
                            else
                            {
                                serializedData = JsonConvert.SerializeObject(leftData, leftData.GetType(), Catalog.GetJsonNetSerializerSettings());
                            }
                            EditorGUILayout.TextArea(serializedData, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                            GUI.enabled = true;
                        }
                        else
                        {
                            if (currentPaneMode == PaneMode.ChangeView) GUI.enabled = false;
                            TryCatchThrowUnityRequiredExceptions(() => DrawEditorPane(GetSide(Side.Right), rightData));
                            GUI.enabled = true;
                        }
                    }
                }
            }
            else
            {
                // unsplit
                rightEditor = null;
                using (var scrollScope = new GUILayout.ScrollViewScope(leftScrollPosition, false, true))
                {
                    leftScrollPosition = scrollScope.scrollPosition;
                    TryCatchThrowUnityRequiredExceptions(() => DrawEditorPane(GetSide(Side.Left), leftData));
                }
            }
        }

        private static void TryCatchThrowUnityRequiredExceptions(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                if (IsExitGUIException(e))
                {
                    throw;
                }
                Debug.LogError(e);
            }
        }

        private static bool IsExitGUIException(Exception exception)
        {
            while (exception is TargetInvocationException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception is ExitGUIException;
        }

        private void DrawEditorPane(Editor editor, object data)
        {
            if (editor == null) return;
            if (data == null) return;
            GUIStyle typeHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
            };
            if (currentPaneMode != PaneMode.ItemJSONWizard)
            {
                Type type = data.GetType();
                string typePath = type.Name;
                Type parent = type.BaseType;
                while (true)
                {
                    if (parent == typeof(CatalogData) || parent == null) break;
                    typePath = $"{typePath} : {parent.Name}";
                    parent = parent.BaseType;
                }
                typePath = Regex.Replace(typePath, @"`\d+", "") + " : CatalogData";
                GUIContent typeNameContent = new(data.GetType().FullName, typePath);
                EditorGUILayout.LabelField(typeNameContent, typeHeaderStyle, GUILayout.ExpandWidth(true));
            }
            else
            {
                GUIContent typeNameContent = new("Base Item");
                EditorGUILayout.LabelField(typeNameContent, typeHeaderStyle, GUILayout.ExpandWidth(true));
            }
            Rect typeRect = GUILayoutUtility.GetLastRect();
            GUIStyle objectNameStyle = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField(data is CatalogData catData ? catData.id : "Game Data", objectNameStyle, GUILayout.ExpandWidth(true));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.DrawRect(new Rect() { x = lastRect.x + 5f, y = lastRect.y + lastRect.height, width = lastRect.width - 5f, height = 1f }, new Color(0f, 0f, 0f, 0.4f));
            if (currentPaneMode != PaneMode.ItemJSONWizard)
            {
                using (new GUILayout.HorizontalScope())
                {
                    float widthAddition = 20f;
                    GUILayout.FlexibleSpace();
                    bool showNonGameDataButtons = data is CatalogData;
                    bool hasOwner = data is CatalogData cData && cData.owner != null;
                    GUI.enabled = hasModsLoaded;
                    if (showNonGameDataButtons)
                    {
                        GUIContent saveCatContent = new(" Save Category", GetIcon(Icon.SaveCategory, customToolbarStyle.fontSize), $"Save all JSONs in the selected data's category");
                        if (GUILayout.Button(saveCatContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(saveCatContent).x + widthAddition)))
                        {
                            foreach (CatalogData catalogData in Catalog.GetDataList(Catalog.GetCategory(data.GetType())))
                            {
                                Catalog.SaveToJson(catalogData);
                            }
                        }
                    }
                    if (!hasOwner || data == Catalog.gameData) GUI.enabled = false;
                    GUIContent saveContent = new(" Save", GetIcon(Icon.Save, customToolbarStyle.fontSize), "Save the currently open JSON");
                    if (GUILayout.Button(saveContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(saveContent).x + widthAddition)))
                    {
                        if (data is CatalogData catalogData)
                        {
                            Catalog.SaveToJson(catalogData);
                            if (!initialJsons.ContainsKey(catalogData.editorGuid))
                            {
                                CatalogData initialData = catalogData.CloneJson();
                                initialJsons[catalogData.editorGuid] = initialData;
                                initialData.sourceFolders = catalogData.sourceFolders;
                                initialData.realFilePath = catalogData.realFilePath;
                                initialData.filePath = catalogData.filePath;
                                initialData.version = catalogData.version;
                                initialData.hashId = catalogData.hashId;
                                if (currentPaneMode == PaneMode.ChangeView)
                                {
                                    SetSide(Side.Right, initialData, false);
                                }
                            }
                        }
                        else
                        {
                            if (data is GameData gameData && gameData.isOverrider)
                            {
                                if (File.Exists(gameData.filePath))
                                {
                                    File.Delete(gameData.filePath);
                                }
                                File.WriteAllText(gameData.filePath, GameDataToJsonString(gameData));
                            }
                            else
                            {
                                Catalog.SaveGameData();
                            }
                        }
                    }
                    GUI.enabled = !Application.isPlaying && hasModsLoaded;
                    if (showNonGameDataButtons)
                    {
                        string tooltip = hasModsLoaded ? "Create a JSON with the same ID to override this JSON" : "Overriding is only enabled if loaded with mods";
                        GUIContent createOverrideContent = new(" Create Override", GetIcon(Icon.Override, customToolbarStyle.fontSize), tooltip);
                        if (GUILayout.Button(createOverrideContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(createOverrideContent).x + widthAddition)))
                        {
                            CreateOverrider((CatalogData)data);
                        }
                    }
                    else
                    {
                        string tooltip = hasModsLoaded ? "Create a JSON to override this JSON" : "Overriding is only enabled if loaded with mods";
                        GUIContent createOverrideContent = new(" Create Override", GetIcon(Icon.Override, customToolbarStyle.fontSize), tooltip);
                        if (GUILayout.Button(createOverrideContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(createOverrideContent).x + widthAddition)))
                        {
                            CreateGameDataOverrider();
                        }
                    }
                    if (showNonGameDataButtons)
                    {
                        GUI.enabled = !Application.isPlaying;
                        if (!hasModsLoaded) GUI.enabled = false;
                        GUIContent cloneContent = new(" Clone", GetIcon(Icon.Clone, customToolbarStyle.fontSize), "Make a clone of the currently open JSON");
                        if (GUILayout.Button(cloneContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(cloneContent).x + widthAddition)))
                        {
                            CloneData(data as CatalogData);
                        }
                    }
                    if (!hasOwner || data == Catalog.gameData) GUI.enabled = false;
                    GUIContent resetContent = new(" Reset", GetIcon(Icon.Reset, customToolbarStyle.fontSize), "Reset all modified values in currently open JSON");
                    if (GUILayout.Button(resetContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(resetContent).x + widthAddition)))
                    {
                        ResetData(data);
                    }
                    if (showNonGameDataButtons)
                    {
                        GUIContent deleteContent = new(" Delete", GetIcon(Icon.Delete, customToolbarStyle.fontSize), "Delete the currently open JSON");
                        if (GUILayout.Button(deleteContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(deleteContent).x + widthAddition)))
                        {
                            DeleteData(data as CatalogData);
                        }
                    }
                    GUI.enabled = true;
                    GUIContent folderContent = new(" Open JSON Folder", GetIcon(Icon.OpenFolder, customToolbarStyle.fontSize), "Open the folder containing the file for the currently open JSON");
                    if (GUILayout.Button(folderContent, customToolbarStyle, GUILayout.Width(customToolbarStyle.CalcSize(folderContent).x + widthAddition)))
                    {
                        if (data is CatalogData catalogData)
                        {
                            System.Diagnostics.Process.Start(catalogData.realFilePath.Substring(0, catalogData.realFilePath.LastIndexOf('\\')));
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(FileManager.GetFullPath(FileManager.Type.JSONCatalog, FileManager.Source.Default, Catalog.gameData.GetLatestOverrideFolder()));
                        }
                    }
                }
            }
            else
            {
                EditorGUI.DrawRect(new Rect() { x = typeRect.x + 5f, y = typeRect.y, width = typeRect.width - 5f, height = 1f }, new Color(0f, 0f, 0f, 0.4f));
                GUI.enabled = false;
            }
            editor?.OnInspectorGUI();
            GUI.enabled = true;
        }

        private string GameDataToJsonString(GameData gameData)
        {
            if (!gameData.isOverrider) return JsonConvert.SerializeObject(gameData, typeof(GameData), Catalog.GetJsonNetSerializerSettings());
            var jCatalogData = JObject.FromObject(gameData, Catalog.GetJsonNetSerializer());
            var jBaseData = JObject.FromObject(Catalog.gameData, Catalog.GetJsonNetSerializer());
            JObject strippedGameData = JsonObjectStripper.StripMatchingRecursive(jCatalogData, jBaseData);
            return JsonConvert.SerializeObject(strippedGameData, strippedGameData.GetType(), Catalog.GetJsonNetSerializerSettings());
        }

        private void CreateNewData(object type)
        {
            Type selectedType = type as Type;
            CatalogData newCatalogData = (CatalogData)Activator.CreateInstance(selectedType);
            newCatalogData.id = $"New{selectedType.Name}";
            string defaultFolderPath = FileManager.GetFullPath(FileManager.Type.JSONCatalog, hasModsLoaded ? FileManager.Source.Mods : FileManager.Source.Default);
            string filePath = EditorUtility.SaveFilePanel("New JSON", defaultFolderPath, newCatalogData.GetFileNameString(), "json");
            if (filePath.IsNullOrEmptyOrWhitespace())
            {
                Debug.LogWarning("Cancelled create new JSON");
                return;
            }
            string idFromFile = filePath.Substring(filePath.LastIndexOf('/') + 1).Replace(".json", "");
            if (idFromFile.Contains("_")) idFromFile = idFromFile.Substring(idFromFile.LastIndexOf("_") + 1);
            newCatalogData.id = idFromFile;
            if (Catalog.TryGetCategory(selectedType, out Category category))
            {
                Catalog.GetDataList(category).Add(newCatalogData);
                newCatalogData.OnCatalogRefresh();
                newCatalogData.editorGuid = GUID.Generate().ToString();
                newCatalogData.filePath = $"{filePath}";
                if (hasModsLoaded) newCatalogData.owner = new ModManager.ModData() { Name = "New JSON", Author = "User" };
                loadedJsons[newCatalogData.editorGuid] = newCatalogData;
                loadedJsons.OrderBy(catalogData => catalogData.Value.id);
                SetSide(Side.Left, newCatalogData);
                BuildTree();
            }
            else
            {
                Debug.LogError($"Could not find category for type {selectedType}, unable to add to the catalog");
            }
        }

        private void CreateOverrider(CatalogData catalogData)
        {
            string defaultFolderPath = FileManager.GetFullPath(FileManager.Type.JSONCatalog, hasModsLoaded ? FileManager.Source.Mods : FileManager.Source.Default);
            string filePath = EditorUtility.SaveFilePanel("Override JSON", defaultFolderPath, catalogData.GetFileNameString(), "json");
            if (filePath.IsNullOrEmptyOrWhitespace())
            {
                Debug.LogWarning("Cancelled create override JSON");
                return;
            }
            Type catalogDataType = catalogData.GetType();
            CatalogData newCatalogData = catalogData.CloneJson();
            newCatalogData.id = catalogData.id;
            newCatalogData.overrideID = catalogData.id;
            newCatalogData.editorGuid = GUID.Generate().ToString();
            newCatalogData.filePath = $"{filePath}";
            newCatalogData.owner = new ModManager.ModData() { Name = "New JSON", Author = "User" };
            loadedJsons[newCatalogData.editorGuid] = newCatalogData;
            CreateInitialJSON(newCatalogData);
            loadedJsons.OrderBy(c => c.Value.id);
            loadedJsons[catalogData.editorGuid].overriderDatas ??= new List<CatalogData>();
            loadedJsons[catalogData.editorGuid].overriderDatas.Add(newCatalogData);
            SetSide(Side.Left, newCatalogData);
            BuildTree();
        }

        private void CreateGameDataOverrider()
        {
            string defaultFolderPath = FileManager.GetFullPath(FileManager.Type.JSONCatalog, hasModsLoaded ? FileManager.Source.Mods : FileManager.Source.Default);
            string filePath = EditorUtility.SaveFilePanel("Override Game JSON", defaultFolderPath, "Game", "json");
            if (filePath.IsNullOrEmptyOrWhitespace())
            {
                Debug.LogWarning("Cancelled create Game JSON overrider");
                return;
            }
            GameData newGameData = Catalog.gameData.CloneJson();
            newGameData.editorId = $"{GameData.overrideEditorId}_{Catalog.gameData.overriders?.Count ?? 0}";
            newGameData.filePath = $"{filePath}";
            initialGameDatas[newGameData.editorId] = newGameData.CloneJson();
            if (Catalog.gameData.overriders == null) Catalog.gameData.overriders = new List<GameData>();
            Catalog.gameData.overriders.Add(newGameData);
            SetSide(Side.Left, newGameData);
        }

        private void CloneData(CatalogData catalogData)
        {
            Side openSide = catalogData == leftData ? Side.Left : Side.Right;
            if (Catalog.TryGetCategory(catalogData.GetType(), out Category category))
            {
                CatalogData clone = catalogData.CloneJson();
                clone.id = $"{clone.id} Copy";
                Catalog.GetDataList(category).Add(clone);
                clone.OnCatalogRefresh();
                clone.editorGuid = GUID.Generate().ToString();
                clone.sourceFolders = catalogData.sourceFolders;
                if (hasModsLoaded) clone.owner = new ModManager.ModData() { Name = "New JSON", Author = "User" };
                loadedJsons[clone.editorGuid] = clone;
                CatalogData cloneInitial = clone.CloneJson();
                cloneInitial.editorGuid = clone.editorGuid;
                initialJsons[clone.editorGuid] = cloneInitial;
                SetSide(openSide, clone);
            }
            loadedJsons.OrderBy(catalogData => catalogData.Value.id);
            BuildTree();
        }

        private void ResetData(object data)
        {
            Side openSide = data == leftData ? Side.Left : Side.Right;
            if (data is CatalogData catalogData && initialJsons.TryGetValue(catalogData.editorGuid, out CatalogData initialData))
            {
                if (Catalog.TryGetCategory(catalogData.GetType(), out Category category))
                {
                    Catalog.GetDataList(category).Remove(catalogData);
                    loadedJsons.Remove(catalogData.editorGuid);
                    initialJsons.Remove(catalogData.editorGuid);

                    CatalogData resetData = initialData.CloneJson();
                    resetData.editorGuid = initialData.editorGuid;
                    resetData.overrideID = catalogData.overrideID;
                    resetData.owner = catalogData.owner;
                    resetData.sourceFolders = catalogData.sourceFolders;
                    resetData.filePath = catalogData.filePath;
                    resetData.realFilePath = catalogData.realFilePath;
                    resetData.version = catalogData.version;
                    resetData.hashId = catalogData.hashId;

                    initialJsons[resetData.editorGuid] = initialData;
                    loadedJsons[resetData.editorGuid] = resetData;
                    Catalog.GetDataList(category).Add(resetData);

                    resetData.OnCatalogRefresh();
                    resetData.CatalogEditorRefresh();

                    SetSide(openSide, resetData, false);
                }
            }
            else if (data is GameData gameData)
            {
                if (gameData.isOverrider)
                {
                    Catalog.gameData.overriders.Remove(gameData);
                    GameData resetGameData = initialGameDatas[gameData.editorId].CloneJson();
                    resetGameData.sourceFolders = gameData.sourceFolders;
                    resetGameData.filePath = gameData.filePath;
                    Catalog.gameData.overriders.Add(resetGameData);
                    SetSide(openSide, resetGameData, false);
                }
                else
                {
                    GameData resetGameData = initialGameDatas[GameData.overrideEditorId].CloneJson();
                    resetGameData.sourceFolders = Catalog.gameData.sourceFolders;
                    resetGameData.filePath = Catalog.gameData.filePath;
                    resetGameData.overriders = new List<GameData>();
                    resetGameData.overriders.AddRange(Catalog.gameData.overriders);
                    Catalog.gameData = resetGameData;
                    SetSide(openSide, Catalog.gameData, false);
                }
            }
        }

        private void DeleteData(CatalogData catalogData)
        {
            if (Catalog.TryGetCategory(catalogData.GetType(), out Category category))
            {
                initialJsons.TryGetValue(catalogData.editorGuid, out CatalogData initialData);
                if (catalogData.isOverrider && Catalog.GetData(category, catalogData.overrideID) is CatalogData baseData)
                {
                    loadedJsons[baseData.editorGuid].overriderDatas?.Remove(catalogData);
                    loadedJsons[baseData.editorGuid].overriderDatas?.Remove(initialData);
                    initialJsons[baseData.editorGuid].overriderDatas?.Remove(catalogData);
                    initialJsons[baseData.editorGuid].overriderDatas?.Remove(initialData);
                }
                initialJsons.Remove(catalogData.editorGuid);
                if (File.Exists(catalogData.realFilePath))
                {
                    File.Delete(catalogData.realFilePath);
                }
                Catalog.GetDataList(category).Remove(catalogData);
                if (leftData == catalogData)
                {
                    leftEditor = null;
                    leftContainerA = null;
                    leftData = null;
                }
                if (rightData == catalogData || rightData == initialData)
                {
                    rightEditor = null;
                    rightContainerA = null;
                    rightData = null;
                }
                loadedJsons.Remove(catalogData.editorGuid);
            }
            BuildTree();
        }

        private void SetPaneStyle(object selection)
        {
            currentPaneMode = (PaneMode)selection;
            rightData = null;
            rightContainerA = null;
            rightEditor = null;
            if (currentPaneMode == PaneMode.ChangeView)
            {
                if (leftData is CatalogData catalogData)
                {
                    if (initialJsons.TryGetValue(catalogData.editorGuid, out CatalogData initialData))
                    {
                        SetSide(Side.Right, initialData);
                    }
                }
                else if (leftData is GameData gameData)
                {
                    SetSide(Side.Right, initialGameDatas[gameData.editorId]);
                }
            }
            if (currentPaneMode == PaneMode.ItemJSONWizard)
            {
                itemWizardContainer = CreateInstance<ItemWizardContainer>();
                SetSide(Side.Right, itemWizardContainer);
                if (leftData is not CatalogData catalogData || catalogData is not ItemData itemData)
                {
                    leftData = null;
                    leftContainerA = null;
                    leftEditor = null;
                }
            }
        }

        private void SetProgressBar(bool active, string title, string message, float percent)
        {
            progressBarOpen = active;
            progressBarTitle = title;
            progressBarText = message;
            progressBarPercent = percent;
        }

        private void CatalogJSONReadProgress(int total, int current, int count) => SetProgressBar(total != current, $"Reading JSON", $"Reading {current}/{total} | JSONs found: {count}", (float)current / total);

        private void CatalogJSONDeserializeProgress(int total, int deserialized) => SetProgressBar(true, $"Converting JSON", $"Converting JSONs | Total: {total} | Converted: {deserialized}", (float)deserialized / total);

        private void CatalogJSONLoadProgress(int total, int current) => SetProgressBar(total != current, $"Loading JSON", $"Loading {current}/{total}", (float)current / total);

        private void CatalogJSONRefreshProgress(int total, int current, string message) => SetProgressBar(total != current, $"Refreshing JSON", $"Refreshing {message} [{current}/{total}]", (float)current / total);

        private void InitializeJSONsForEditing()
        {
            EditorUtility.DisplayProgressBar($"Editor JSON Load", $"Starting initial JSON store...", 0f);
            initialGameDatas = new Dictionary<string, GameData>();
            initialGameDatas[Catalog.gameData.editorId] = Catalog.gameData.CloneJson();
            if ((Catalog.gameData.overriders?.Count ?? 0) > 0)
            {
                foreach (var overriderGameData in Catalog.gameData.overriders)
                {
                    initialGameDatas[overriderGameData.editorId] = overriderGameData.CloneJson();
                }
            }
            loadedJsons = new Dictionary<string, CatalogData>();
            initialJsons = new Dictionary<string, CatalogData>();
            IOrderedEnumerable<Category> categories = ((Category[])Enum.GetValues(typeof(Category))).OrderBy(cat =>
            {
                Type categoryType = null;
                foreach ((Type Type, Category Category) tuple in Catalog.baseTypeCategories)
                {
                    if (cat == tuple.Category)
                    {
                        categoryType = tuple.Type;
                        break;
                    }
                }
                if (categoryType != null && categoryType.TryGetCustomAttribute(out EditorLoadPriorityAttribute priorityAttribute))
                {
                    return priorityAttribute.Priority;
                }
                return 0;
            }).ThenBy(x => x.ToString());
            int categoryCount = categories.Count();
            int currentCat = 0;
            storeInitialQueue ??= new List<string>();
            foreach (Category category in categories)
            {
                //Get the catalog data for this category
                CatalogCategory catalogCategory = Catalog.data[(int)category];
                if (catalogCategory == null)
                {
                    continue;
                }
                currentCat++;
                string categoryName = category.ToString();
                string categoryNameSlash = $"{categoryName}s/";
                int dataCount = catalogCategory.catalogDatas.Count;
                int currentJSON = 0;
                foreach (CatalogData catalogData in catalogCategory.catalogDatas.OrderBy(i => i.SortKey()))
                {
                    currentJSON++;
                    EditorUtility.DisplayProgressBar($"Editor JSON Load", $"Store initial JSONs | Category {currentCat}/{categoryCount} {Utils.AddSpacesToSentence(category.ToString(), true)} {currentJSON}/{dataCount}", (float)currentJSON / dataCount);
                    catalogData.CatalogEditorRefresh();
                    if (catalogData.editorGuid.IsNullOrEmptyOrWhitespace()) catalogData.editorGuid = GUID.Generate().ToString();
                    loadedJsons[catalogData.editorGuid] = catalogData;
                    storeInitialQueue.Add(catalogData.editorGuid);
                }
            }
            loadedJsons.OrderBy(catalogData => catalogData.Value.id);
            initialStoreStart = Time.realtimeSinceStartup;
        }

        private void CreateInitialJSON(CatalogData catalogData)
        {
            storing = true;
            if (!initialJsons.ContainsKey(catalogData.editorGuid))
            {
                CatalogData initialData = catalogData.CloneJson();
                initialJsons[catalogData.editorGuid] = initialData;
                initialData.editorGuid = catalogData.editorGuid;
                initialData.changers = catalogData.changers;
                initialData.sourceFolders = catalogData.sourceFolders;
                initialData.realFilePath = catalogData.realFilePath;
                initialData.filePath = catalogData.filePath;
                initialData.version = catalogData.version;
                initialData.hashId = catalogData.hashId;
            }
            storing = false;
        }

        private void BuildTree()
        {
            EditorUtility.DisplayProgressBar($"Building Tree", $"Starting tree build...", 0f);
            int progressMax = loadedJsons.Count;
            int currentProgress = 0;
            foreach (CatalogData catalogData in loadedJsons.Values)
            {
                currentProgress++;
                EditorUtility.DisplayProgressBar($"Building Tree", $"Organize JSONs first pass {currentProgress}/{progressMax}", (float)currentProgress / progressMax);

                void AddToBranch(TreeBranch branch)
                {
                    if (branch.branchPath == catalogData.GetCatalogPath())
                    {
                        TreeBranch.onNewBranch -= AddToBranch;
                        branch.dataGUIDs.Add(catalogData.editorGuid);
                    }
                }

                // static event gets cleaned when the method finishes
#pragma warning disable UDR0005 // Domain Reload Analyzer
                TreeBranch.onNewBranch += AddToBranch;
#pragma warning restore UDR0005 // Domain Reload Analyzer
            }
            rootBranch = new TreeBranch("", loadedJsons);
            Dictionary<string, TreeBranch> treeBranchDictionary = new();
            List<TreeBranch> unparentedBranches = new();
            IOrderedEnumerable<Category> categories = ((Category[])Enum.GetValues(typeof(Category))).OrderBy(x => x.ToString());
            currentProgress = 0;
            foreach (Category category in categories)
            {
                CatalogCategory catalogCategory = Catalog.data[(int)category];
                if (catalogCategory == null) continue;
                foreach (CatalogData catalogData in catalogCategory.catalogDatas.OrderBy(i => i.SortKey()))
                {
                    currentProgress++;
                    EditorUtility.DisplayProgressBar($"Building Tree", $"Organize JSONs second pass {currentProgress}/{progressMax}", (float)currentProgress / progressMax);
                    catalogData.CatalogEditorRefresh();
                    string catalogDataPath = catalogData.GetCatalogPath();
                    if (treeBranchDictionary.ContainsKey(catalogDataPath)) continue;
                    TreeBranch branch = new TreeBranch(catalogDataPath, loadedJsons);
                    treeBranchDictionary[catalogDataPath] = branch;
                    if (catalogDataPath.Contains('/'))
                    {
                        unparentedBranches.Add(branch);
                    }
                    else
                    {
                        rootBranch.childBranches.Add(branch);
                    }
                }
            }
            progressMax = unparentedBranches.Count;
            currentProgress = 0;
            for (int u = unparentedBranches.Count - 1; u >= 0; u--)
            {
                currentProgress++;
                EditorUtility.DisplayProgressBar($"Building Tree", $"Organizing tree first pass {currentProgress}/{progressMax}", (float)currentProgress / progressMax);
                TreeBranch branch = unparentedBranches[u];
                string[] pathParts = branch.branchPath.Split('/');
                TreeBranch lastParent = rootBranch;
                string rebuiltPath = "";
                for (int p = 0; p < pathParts.Length; p++)
                {
                    rebuiltPath += (p == 0 ? "" : "/") + pathParts[p];
                    if (rebuiltPath == branch.branchPath)
                    {
                        if (!lastParent.childBranches.Contains(branch)) lastParent.childBranches.Add(branch);
                        break;
                    }
                    if (!treeBranchDictionary.TryGetValue(rebuiltPath, out TreeBranch branchParent))
                    {
                        branchParent = new TreeBranch(rebuiltPath, loadedJsons);
                        treeBranchDictionary[rebuiltPath] = branchParent;
                    }
                    if (!lastParent.childBranches.Contains(branchParent)) lastParent.childBranches.Add(branchParent);
                    lastParent = branchParent;
                }
                unparentedBranches.RemoveAt(u);
            }
            rootBranch.childBranches.Sort((a, b) => string.Compare(a.branchPath, b.branchPath));
            progressMax = treeBranchDictionary.Values.Count;
            currentProgress = 0;
            foreach (TreeBranch branch in treeBranchDictionary.Values)
            {
                currentProgress++;
                EditorUtility.DisplayProgressBar($"Building Tree", $"Organizing tree second pass {currentProgress}/{progressMax}", (float)currentProgress / progressMax);
                branch.childBranches.Sort((a, b) => string.Compare(a.branchPath, b.branchPath));
            }
            EditorUtility.ClearProgressBar();
            TreeBranch.CleanStaticEvent();
        }
    }
}
