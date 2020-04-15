using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Editor;
using Unity.Entities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.LowLevel;
using System;
using UnityEngine.Profiling;
using Unity.Editor.Bridge;
using Resources = Unity.Editor.Resources;

namespace Unity.DOTS.Editor
{
    class SystemScheduleWindow : EditorWindow
    {
        const string k_SystemScheduleWindow = "internal:Window/DOTS/Systems Schedule";
        static readonly string k_WindowName = L10n.Tr("Systems");
        const string k_ShowFullPlayerLoopString = "Show Full Player Loop";
        const string k_ShowInactiveSystemsString = "Show Inactive Systems";
        const string k_NoWorldString = "No World";

        VisualElement m_Root;
        ToolbarMenu m_WorldMenu;
        Toggle m_ShowFullPlayerLoopToggle;
        Toggle m_ShowInactiveSystemsToggle;
        VisualElement m_SystemTreeViewHeader;
        VisualElement m_SystemTreeViewContainer;
        SystemScheduleTreeView m_SystemTreeView;
        ToolbarSearchField m_SearchField;

        /// <summary>
        /// Helper class to detect changes to system list.
        /// </summary>
        SystemListChangeTracker m_ChangeTracker;

        /// <summary>
        /// Hashmap to keep track of the parent-children relationship for all the systems.
        /// </summary>
        static NativeMultiHashMap<int, int> s_ParentChildrenMap;
        static NativeHashMap<int, int> s_ChildrenParentMap;
        static Dictionary<int, ComponentSystemBase> s_SystemsById;
        static Dictionary<int, string> s_PlayerLoopSystemById;
        static Dictionary<ComponentSystemBase, AverageRecorder> s_RecordersBySystem;

        class AverageRecorder
        {
            readonly Recorder m_Recorder;
            int m_FrameCount;
            int m_TotalNanoseconds;
            float m_LastReading;

            public AverageRecorder(Recorder recorder)
            {
                this.m_Recorder = recorder;
            }

            public void Update()
            {
                ++m_FrameCount;
                m_TotalNanoseconds += (int)m_Recorder.elapsedNanoseconds;
            }

            public float ReadMilliseconds()
            {
                if (m_FrameCount > 0)
                {
                    m_LastReading = (m_TotalNanoseconds / 1e6f) / m_FrameCount;
                    m_FrameCount = m_TotalNanoseconds = 0;
                }

                return m_LastReading;
            }
        }

        // To get information after domain reload.
        const string k_SharedStateKey = nameof(SystemScheduleWindow) + "." + nameof(SharedState);
        const string k_StateKey = nameof(SystemScheduleWindow) + "." + nameof(State);

        /// <summary>
        /// Helper container to store session state data.
        /// </summary>
        class SharedState
        {
            /// <summary>
            /// The currently selected <see cref="World"/> in the drop-down.
            /// </summary>
            public string SelectedWorldName;
        }

        /// <summary>
        /// Helper container to store session state data.
        /// </summary>
        class State
        {
            /// <summary>
            /// This field controls the showing of full player loop state.
            /// </summary>
            public bool ShowFullPlayerLoopBool;

            /// <summary>
            /// This field controls the showing of inactive system state.
            /// </summary>
            public bool ShowInactiveSystemsBool;

            /// <summary>
            /// This field controls the search field search string.
            /// </summary>
            public string SearchFilterString;
        }

        /// <summary>
        /// State World data for <see cref="SystemScheduleWindow"/>. This data is persisted between domain reloads.
        /// </summary>
        SharedState m_SharedState;

        /// <summary>
        /// State data for <see cref="SystemScheduleWindow"/>. This data is persisted between domain reloads.
        /// </summary>
        State m_State;

        /// <summary>
        /// Get currently selected world, also after domain reload, get last selected world if exists.
        /// </summary>
        /// <returns></returns>
        World GetCurrentlySelectedWorld()
        {
            if (World.All.Count == 0)
            {
                return null;
            }

            World selectedWorld = null;
            foreach (var world in World.All)
            {
                if (world.Name == m_SharedState.SelectedWorldName)
                {
                    selectedWorld = world;
                    break;
                }
            }

            if (null == selectedWorld)
            {
                selectedWorld = World.All[0];
            }

            m_SharedState.SelectedWorldName = selectedWorld.Name;
            return selectedWorld;
        }

        [MenuItem(k_SystemScheduleWindow, false)]
        static void OpenWindow()
        {
            var window = GetWindow<SystemScheduleWindow>();
            window.Show();
        }

        /// <summary>
        /// Build the GUI for the system window.
        /// </summary>
        public void OnEnable()
        {
            titleContent = EditorGUIUtility.TrTextContent(k_WindowName);
            minSize = new Vector2(530, 300);

            m_SharedState = SessionState<SharedState>.GetOrCreateState(k_SharedStateKey);
            m_State = SessionState<State>.GetOrCreateState(k_StateKey);
            m_ChangeTracker = new SystemListChangeTracker();
            s_ParentChildrenMap = new NativeMultiHashMap<int, int>(512, Allocator.Persistent);
            s_ChildrenParentMap = new NativeHashMap<int, int>(512, Allocator.Persistent);
            s_SystemsById = new Dictionary<int, ComponentSystemBase>();
            s_PlayerLoopSystemById = new Dictionary<int, string>();
            s_RecordersBySystem = new Dictionary<ComponentSystemBase, AverageRecorder>();

            m_Root = this.rootVisualElement;
            Resources.Templates.SystemSchedule.AddStyles(m_Root);

            // Create toolbar for world drop-down, search field.
            CreateToolBar(m_Root);

            // Create a header for treeview.
            CreateTreeViewHeader(m_Root);

            // Create tree view for systems.
            m_SystemTreeView = new SystemScheduleTreeView();
            m_SystemTreeView.style.flexGrow = 1;
            m_SystemTreeView.SystemTreeView.Filter = OnFilter;

            m_Root.Add(m_SystemTreeView);

            if (World.All.Count > 0)
                BuildAll();

            // Update the tree view when necessary.
            EditorApplication.update += Update;
        }

        public void OnDisable()
        {
            if (s_ParentChildrenMap.IsCreated)
                s_ParentChildrenMap.Dispose();

            if (s_ChildrenParentMap.IsCreated)
                s_ChildrenParentMap.Dispose();
        }

        /// <summary>
        /// Clean up the window.
        /// </summary>
        public void OnDestroy()
        {
            EditorApplication.update -= Update;
        }

        bool OnFilter(ITreeViewItem item)
        {
            if (string.IsNullOrEmpty(m_State.SearchFilterString))
                return true;

            var itemAsData = item as SystemTreeViewItem;
            if (itemAsData == null)
                return false;

            if (itemAsData.GetSystemName().IndexOf(m_State.SearchFilterString.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }

        // Rebuild the root node with inactive systems.
        void UpdateWorldDropDownMenu()
        {
            if (m_WorldMenu == null)
                return;

            var menu = m_WorldMenu.menu;
            var menuItemsCount = menu.MenuItems().Count;

            for (var i = 0; i < menuItemsCount; i++)
            {
                menu.RemoveItemAt(0);
            }

            if (World.All.Count > 0)
            {
                foreach (var world in World.All)
                {
                    menu.AppendAction(world.Name, OnSelectWorld, DropdownMenuAction.AlwaysEnabled, world);
                }
            }
            else
            {
                menu.AppendAction(k_NoWorldString, OnSelectWorld, DropdownMenuAction.AlwaysEnabled, null);
            }
        }

        void OnSelectWorld(DropdownMenuAction action)
        {
            var menuText = "";
            World world = action.userData as World;

            if (world != null)
            {
                menuText = world.Name;
                m_SharedState.SelectedWorldName = world.Name;
            }
            else
            {
                menuText = k_NoWorldString;
            }

            m_WorldMenu.text = menuText;

            BuildAll();
        }

        // Create toolbar, including World drop-down, toggles, search field.
        void CreateToolBar(VisualElement root)
        {
            var toolbar = new Toolbar();
            toolbar.AddToClassList(UssClasses.SystemScheduleWindow.ToolbarContainer);
            root.Add(toolbar);

            var currentWorld = GetCurrentlySelectedWorld();
            m_WorldMenu = new ToolbarMenu
            {
                name = "worldMenu",
                variant = ToolbarMenu.Variant.Popup,
                text = currentWorld == null ? k_NoWorldString : currentWorld.Name
            };
            UpdateWorldDropDownMenu();

            toolbar.Add(m_WorldMenu);

            m_ShowFullPlayerLoopToggle = new Toggle(k_ShowFullPlayerLoopString);
            m_ShowFullPlayerLoopToggle.SetValueWithoutNotify(m_State.ShowFullPlayerLoopBool);
            m_ShowFullPlayerLoopToggle.RegisterCallback<ChangeEvent<bool>>(OnShowFullPlayerLoop);

            m_ShowInactiveSystemsToggle = new Toggle(k_ShowInactiveSystemsString);
            m_ShowInactiveSystemsToggle.SetValueWithoutNotify(m_State.ShowInactiveSystemsBool);
            m_ShowInactiveSystemsToggle.RegisterCallback<ChangeEvent<bool>>(OnShowInactiveSystems);

            toolbar.Add(m_ShowFullPlayerLoopToggle);
            toolbar.Add(m_ShowInactiveSystemsToggle);

            // Search field.
            m_SearchField = new ToolbarSearchField();
            m_SearchField.AddToClassList(UssClasses.SystemScheduleWindow.SearchField);
            m_SearchField.RegisterValueChangedCallback(e =>
            {
                m_State.SearchFilterString = e.newValue;
                BuildAll();
            });
            toolbar.Add(m_SearchField);
        }

        // Manually create header for the tree view.
        void CreateTreeViewHeader(VisualElement root)
        {
            m_SystemTreeViewHeader = new Toolbar();
            m_SystemTreeViewHeader.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.Header);

            var systemHeaderLabel = new Label("Systems");
            systemHeaderLabel.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.System);

            var entityHeaderLabel = new Label("Matches")
            {
                tooltip = "The number of entities that match the queries at the end of the frame."
            };
            entityHeaderLabel.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.Matches);

            var timeHeaderLabel = new Label("Time (ms)")
            {
                tooltip = "Average running time."
            };
            timeHeaderLabel.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.Time);

            m_SystemTreeViewHeader.Add(systemHeaderLabel);
            m_SystemTreeViewHeader.Add(entityHeaderLabel);
            m_SystemTreeViewHeader.Add(timeHeaderLabel);

            root.Add(m_SystemTreeViewHeader);
        }

        // Rebuild treeview with full player loop.
        void OnShowFullPlayerLoop(ChangeEvent<bool> evt)
        {
            var t = (Toggle)evt.currentTarget;
            t.SetValueWithoutNotify(evt.newValue);

            m_State.ShowFullPlayerLoopBool = evt.newValue == true;

            if (World.All.Count > 0)
                BuildAll();
        }

        // Rebuild the treeview with inactive systems.
        void OnShowInactiveSystems(ChangeEvent<bool> evt)
        {
            var t = (Toggle)evt.currentTarget;
            t.SetValueWithoutNotify(evt.newValue);
            m_State.ShowInactiveSystemsBool = evt.newValue == true;

            if (World.All.Count > 0)
                BuildAll();
        }

        // Build the root node for the tree view.
        void BuildAll()
        {
            s_SystemsById.Clear();
            s_PlayerLoopSystemById.Clear();
            s_RecordersBySystem.Clear();
            s_ParentChildrenMap.Clear();
            s_ChildrenParentMap.Clear();

            var currentId = 1;
            ParsePlayerLoopSystem(ScriptBehaviourUpdateOrder.CurrentPlayerLoop, ref currentId, GetCurrentlySelectedWorld());

            SetTreeViewRootItems();
        }

        void SetTreeViewRootItems()
        {
            m_SystemTreeView?.TreeRootItems.Clear();

            foreach (var item in s_PlayerLoopSystemById.Where(item => s_ParentChildrenMap.ContainsKey(item.Key)))
            {
                m_SystemTreeView.TreeRootItems.Add(TreeViewItemFactory(item.Key));
            }

            m_SystemTreeView.RefreshSystemTreeView();
        }

        void CollectChilrenSystems(ref List<int> list, int system)
        {
            if (system == -1)
                return;
            if (list == null)
                list = new List<int>();

            list.Add(system);
        }

        // Parse through the player loop system to get all system list and their parent-children relationship,
        // which will be used to build the treeview.
        int ParsePlayerLoopSystem(PlayerLoopSystem system, ref int currentId, World world)
        {
            List<int> children = null;

            if (system.subSystemList != null)
            {
                foreach (var subSystem in system.subSystemList)
                {
                    CollectChilrenSystems(ref children, ParsePlayerLoopSystem(subSystem, ref currentId, world));
                }
            }
            else
            {
                var executionDelegate = system.updateDelegate;
                ScriptBehaviourUpdateOrder.DummyDelegateWrapper dummy;
                if (executionDelegate != null &&
                    (dummy = executionDelegate.Target as ScriptBehaviourUpdateOrder.DummyDelegateWrapper) != null)
                {
                    var rootSystem = dummy.System;
                    return ParseComponentSystem(rootSystem, ref currentId, world);
                }
            }

            if (m_State.ShowFullPlayerLoopBool || children != null)
            {
                var playerLoopSystemId = currentId;
                currentId++;

                if (system.type != null)
                {
                    s_PlayerLoopSystemById.Add(playerLoopSystemId, system.type?.Name);

                    if (children != null)
                    {
                        for (var i = children.Count - 1; i >= 0; i--)
                        {
                            s_ParentChildrenMap.Add(playerLoopSystemId, children[i]);
                            s_ChildrenParentMap.Add(children[i], playerLoopSystemId);
                        }
                    }
                }

                return playerLoopSystemId;
            }

            return -1;
        }

        // Add ComponentSystemGroup to m_SystemsById and build parent-children relationship.
        int ParseComponentSystem(ComponentSystemBase systemBase, ref int currentId, World world)
        {
            switch (systemBase)
            {
                case ComponentSystemGroup group:
                    List<int> children = null;
                    if (group.Systems != null)
                    {
                        foreach (var child in group.Systems)
                        {
                            CollectChilrenSystems(ref children, ParseComponentSystem(child, ref currentId, world));
                        }
                    }

                    if (m_State.ShowFullPlayerLoopBool || children != null || world == group.World)
                    {
                        var groupId = currentId;
                        ParseSystem(currentId++, group);
                        s_SystemsById.Add(groupId, group);

                        // Build parent-children relationship map.
                        if (children != null)
                        {
                            for (var i = children.Count - 1; i >= 0; i--)
                            {
                                s_ParentChildrenMap.Add(groupId, children[i]);
                                s_ChildrenParentMap.Add(children[i], groupId);
                            }
                        }

                        return groupId;
                    }
                    break;
                case ComponentSystemBase system:
                {
                    if (m_State.ShowFullPlayerLoopBool || world == system.World)
                    {
                        if (ParseSystem(currentId++, system))
                            return currentId - 1;
                    }
                }
                break;
            }

            return -1;
        }

        // Add all ComponentSystemBase type system to m_SystemById and start their recorder.
        bool ParseSystem(int id, ComponentSystemBase system)
        {
            if (!(system is ComponentSystemGroup))
            {
                s_SystemsById.Add(id, system);

                var recorder = Recorder.Get($"{system.World?.Name ?? "none"} {system.GetType().FullName}");
                if (!s_RecordersBySystem.ContainsKey(system))
                {
                    s_RecordersBySystem.Add(system, new AverageRecorder(recorder));
                }
                else
                {
                    UnityEngine.Debug.LogError("System added twice: " + system);
                }
                recorder.enabled = true;
            }

            return m_State.ShowInactiveSystemsBool || system.ShouldRunSystem();
        }

        /// <summary>
        /// Whenever there is a change in world selection or player loop, the whole tree view will be rebuilt.
        /// Constantly update the world dropdown menu to get the latest worlds.
        /// </summary>
        public void Update()
        {
            if (m_WorldMenu == null)
                return;

            UpdateWorldDropDownMenu();
            var currentWorld = GetCurrentlySelectedWorld();
            m_WorldMenu.text = currentWorld == null ? k_NoWorldString : currentWorld.Name;
            UpdateTimings();

            m_SearchField.value = string.IsNullOrEmpty(m_State.SearchFilterString) ? string.Empty : m_State.SearchFilterString;

            if (m_ChangeTracker.DidChange(GetCurrentlySelectedWorld(), ScriptBehaviourUpdateOrder.CurrentPlayerLoop))
            {
                BuildAll();
            }
        }

        int m_LastTimedFrame;

        void UpdateTimings()
        {
            if (Time.frameCount == m_LastTimedFrame)
                return;

            foreach (var recorder in s_RecordersBySystem.Values)
            {
                recorder.Update();
            }

            m_LastTimedFrame = Time.frameCount;
        }

        class SystemListChangeTracker
        {
            World m_LastWorld;
            PlayerLoopSystem m_LastPlayerLoopSystem;
            List<ComponentType> m_ComponentTypeList;

            public bool DidChange(World world, PlayerLoopSystem playerLoopSystem)
            {
                if (world != m_LastWorld)
                {
                    m_LastWorld = world;
                    return true;
                }

                if (!PlayerLoopsMatch(m_LastPlayerLoopSystem, playerLoopSystem))
                {
                    m_LastPlayerLoopSystem = playerLoopSystem;
                    return true;
                }

                foreach (var systemBase in s_SystemsById.Values)
                {
                    if (systemBase is ComponentSystemBase system)
                    {
                        if (system.World == null || !system.World.Systems.Contains(system))
                            return true;
                    }
                }

                return false;
            }

            static bool PlayerLoopsMatch(PlayerLoopSystem a, PlayerLoopSystem b)
            {
                if (a.type != b.type)
                    return false;
                if (a.subSystemList == b.subSystemList)
                    return true;
                if (a.subSystemList == null || b.subSystemList == null)
                    return false;
                if (a.subSystemList.Length != b.subSystemList.Length)
                    return false;
                for (var i = 0; i < a.subSystemList.Length; ++i)
                {
                    if (!PlayerLoopsMatch(a.subSystemList[i], b.subSystemList[i]))
                        return false;
                }

                return true;
            }
        }

        // Build each treeviewitem for the treeview.
        SystemTreeViewItem TreeViewItemFactory(int id)
        {
            var item = new SystemTreeViewItem();
            return item.Initialize(id, s_ParentChildrenMap.ContainsKey(id), TreeViewItemChildrenCollectionFactory);
        }

        // Build and return all the children treeviewitem given a parent.
        IEnumerable<SystemTreeViewItem> TreeViewItemChildrenCollectionFactory(int id)
        {
            var enumerator = s_ParentChildrenMap.GetValuesForKey(id);

            while (enumerator.MoveNext())
            {
                yield return TreeViewItemFactory(enumerator.Current);
            }
        }

        // Custom made treeviewitem to get/set all the system information to be shown in the treeview.
        internal class SystemTreeViewItem : ITreeViewItem, IDisposable
        {
            ComponentSystemBase m_System = null;
            int m_Id;
            bool m_HasChildren;
            Func<int, IEnumerable<ITreeViewItem>> m_GetChildren;
            List<ITreeViewItem> m_CachedChildren;

            public SystemTreeViewItem Initialize(int id, bool hasChildren, Func<int, IEnumerable<ITreeViewItem>> getChildren)
            {
                if (s_SystemsById.ContainsKey(id))
                    m_System = s_SystemsById[id];

                m_Id = id;
                m_HasChildren = hasChildren;
                m_GetChildren = getChildren;

                return this;
            }

            public ComponentSystemBase System => m_System;

            public bool HasChildren => m_HasChildren;

            public string GetSystemName()
            {
                if (s_PlayerLoopSystemById.ContainsKey(m_Id))
                    return s_PlayerLoopSystemById[m_Id];

                if (m_System != null)
                    return m_System.GetType().Name;

                return string.Empty;
            }

            public bool GetParentState()
            {
                return GetParentState(m_Id);
            }

            bool GetParentState(int id)
            {
                if (!s_ChildrenParentMap.ContainsKey(id))
                    return true;

                var parentId = s_ChildrenParentMap[id];
                if (!s_SystemsById.ContainsKey(parentId))
                    return true;

                return s_SystemsById[parentId].Enabled && GetParentState(parentId);
            }

            public bool GetPlayerLoopSystemState()
            {
                var state = true;
                if (s_PlayerLoopSystemById.ContainsKey(m_Id))
                {
                    if (m_HasChildren)
                    {
                        foreach (var child in children)
                        {
                            if (s_SystemsById.ContainsKey(child.id))
                                state &= s_SystemsById[child.id].Enabled;
                        }
                    }
                }

                return state;
            }

            public void SetPlayerLoopSystemState(bool state)
            {
                if (s_PlayerLoopSystemById.ContainsKey(m_Id) && m_HasChildren)
                {
                    foreach (var child in children)
                    {
                        if (s_SystemsById.ContainsKey(child.id))
                            SetSystemState(s_SystemsById[child.id], state);
                    }
                }
            }

            public void SetSystemState(ComponentSystemBase system, bool state)
            {
                system.Enabled = state;
            }

            public string GetEntityMatches()
            {
                if (m_HasChildren) // Group system do not need entity matches.
                    return string.Empty;

                if (m_System?.EntityQueries == null)
                    return string.Empty;

                var matchedEntityCount = m_System.EntityQueries.Sum(query => query.CalculateEntityCount());

                return matchedEntityCount.ToString();
            }

            static float GetAverageRunningTime(ComponentSystemBase system)
            {
                switch (system)
                {
                    case ComponentSystemGroup systemGroup:
                    {
                        if (systemGroup.Systems != null)
                        {
                            return systemGroup.Systems.Sum(GetAverageRunningTime);
                        }
                    }
                    break;
                    case ComponentSystemBase systemBase:
                    {
                        return s_RecordersBySystem.ContainsKey(systemBase) ? s_RecordersBySystem[systemBase].ReadMilliseconds() : 0.0f;
                    }
                }

                return -1;
            }

            public string GetRunningTime()
            {
                var totalTime = 0.0f;

                if (s_PlayerLoopSystemById.ContainsKey(m_Id))
                {
                    return string.Empty;
                }

                if (children.Count() != 0)
                {
                    totalTime += children.Where(child => s_SystemsById.ContainsKey(child.id)).Sum(child => GetAverageRunningTime(s_SystemsById[child.id]));
                }
                else
                {
                    if (s_SystemsById.ContainsKey(m_Id))
                    {
                        if (s_SystemsById[m_Id].ShouldRunSystem())
                        {
                            totalTime = GetAverageRunningTime(s_SystemsById[m_Id]);
                        }
                        else
                        {
                            return "-";
                        }
                    }
                }

                return totalTime.ToString("f2");
            }

            int ITreeViewItem.id => m_Id;
            public ITreeViewItem parent => null;
            public IEnumerable<ITreeViewItem> children => m_CachedChildren ?? (m_CachedChildren = m_GetChildren(m_Id).ToList());
            bool ITreeViewItem.hasChildren => m_HasChildren;

            public void AddChild(ITreeViewItem child)
            {
                throw new NotImplementedException();
            }

            public void AddChildren(IList<ITreeViewItem> children)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                m_System = null;
                m_HasChildren = false;
                m_GetChildren = null;
                m_CachedChildren = null;
            }

            public void RemoveChild(ITreeViewItem child)
            {
                throw new NotImplementedException();
            }
        }
    }
}
