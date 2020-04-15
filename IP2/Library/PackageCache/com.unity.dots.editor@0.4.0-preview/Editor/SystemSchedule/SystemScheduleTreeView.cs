using System.Collections.Generic;
using Unity.Editor;
using UnityEngine.UIElements;
using Unity.Editor.Bridge;
using Unity.Entities;

namespace Unity.DOTS.Editor
{
    class SystemScheduleTreeView : VisualElement
    {
        public readonly TreeView SystemTreeView;
        public readonly IList<ITreeViewItem> TreeRootItems = new List<ITreeViewItem>();

        //@TO-DO: Once the tree view state is fixed, we can remove this auto expand logic.
        const int k_TreeViewInitialSelectionDelay = 10;

        /// <summary>
        /// Constructor of the tree view.
        /// </summary>
        public SystemScheduleTreeView()
        {
            SystemTreeView = new TreeView(TreeRootItems, 20, MakeItem, BindItem);
            SystemTreeView.style.flexGrow = 1;
            Add(SystemTreeView);
        }

        class SystemInformationVisualElement : BindableElement, IBinding
        {
            SystemScheduleWindow.SystemTreeViewItem m_Target;

            public SystemScheduleWindow.SystemTreeViewItem Target
            {
                get => m_Target;
                set
                {
                    if (m_Target == value)
                        return;
                    m_Icon.RemoveFromClassList(GetSystemClass(m_Target?.System));
                    m_Target = value;
                    m_Icon.AddToClassList(GetSystemClass(m_Target?.System));
                    Update();
                }
            }

            readonly Toggle m_SystemEnableToggle;
            readonly VisualElement m_Icon;
            readonly Label m_SystemNameLabel;
            readonly Label m_EntityMatchLabel;
            readonly Label m_RunningTimeLabel;

            public SystemInformationVisualElement()
            {
                Resources.Templates.CommonResources.AddStyles(this);
                Resources.Templates.SystemScheduleItem.Clone(this);
                binding = this;

                AddToClassList(UssClasses.Resources.SystemSchedule);

                m_SystemEnableToggle = this.Q<Toggle>(className: UssClasses.SystemScheduleWindow.Items.Enabled);
                m_SystemEnableToggle.RegisterCallback<ChangeEvent<bool>, SystemInformationVisualElement>(OnSystemTogglePress, this);

                m_Icon = this.Q(className: UssClasses.SystemScheduleWindow.Items.Icon);

                m_SystemNameLabel = this.Q<Label>(className: UssClasses.SystemScheduleWindow.Items.SystemName);
                m_EntityMatchLabel = this.Q<Label>(className: UssClasses.SystemScheduleWindow.Items.Matches);
                m_RunningTimeLabel = this.Q<Label>(className: UssClasses.SystemScheduleWindow.Items.Time);
            }

            static void SetText(Label label, string text)
            {
                if (label.text != text)
                {
                    label.text = text;
                }
            }

            public void Update()
            {
                if (null == Target)
                    return;

                if (m_Target.System != null && m_Target.System.World == null)
                    return;

                if (string.Empty == GetSystemClass(m_Target?.System))
                {
                    m_Icon.style.display = DisplayStyle.None;
                }
                else
                {
                    m_Icon.style.display = DisplayStyle.Flex;
                }

                SetText(m_SystemNameLabel, Target.GetSystemName());
                SetText(m_EntityMatchLabel, Target.GetEntityMatches());
                SetText(m_RunningTimeLabel, Target.GetRunningTime());

                if (Target.System == null && !Target.HasChildren) // player loop system without children
                {
                    this.SetEnabled(false);
                    m_SystemEnableToggle.visible = false;
                }
                else
                {
                    this.SetEnabled(true);
                    m_SystemEnableToggle.visible = true;
                    var systemState = Target.System?.Enabled ?? Target.GetPlayerLoopSystemState();
                    if (m_SystemEnableToggle.value != systemState)
                    {
                        m_SystemEnableToggle.SetValueWithoutNotify(systemState);
                    }

                    var groupState = systemState && Target.GetParentState();

                    m_SystemNameLabel.SetEnabled(groupState);
                    m_EntityMatchLabel.SetEnabled(groupState);
                    m_RunningTimeLabel.SetEnabled(groupState);
                }
            }

            static string GetSystemClass(ComponentSystemBase system)
            {
                switch (system)
                {
                    case null:
                        return "";
                    case EntityCommandBufferSystem _:
                        return UssClasses.SystemScheduleWindow.Items.CommandBufferIcon;
                    case ComponentSystemGroup _:
                        return UssClasses.SystemScheduleWindow.Items.SystemGroupIcon;
                    case ComponentSystemBase _:
                        return UssClasses.SystemScheduleWindow.Items.SystemIcon;
                }
            }

            static void OnSystemTogglePress(ChangeEvent<bool> evt,  SystemInformationVisualElement item)
            {
                if (item.Target.System != null)
                {
                    item.Target.SetSystemState(item.Target.System, evt.newValue);
                }
                else
                {
                    item.Target.SetPlayerLoopSystemState(evt.newValue);
                }
            }

            public void PreUpdate()
            {
            }

            public void Release()
            {
            }
        }

        static VisualElement MakeItem()
        {
            var systemItem = new SystemInformationVisualElement();
            return systemItem;
        }

        /// <summary>
        /// Refresh tree view to update with latest information.
        /// </summary>
        public void RefreshSystemTreeView()
        {
            SystemTreeView.Refresh();
            AutoExpandAllItems();
        }

        //@TO-DO: Once tree view state is fixed, we can remove this auto expand logic.
        void AutoExpandAllItems()
        {
            SystemTreeView.schedule.Execute(() =>
            {
                foreach (var item in SystemTreeView.items)
                    SystemTreeView.ExpandItem(item.id);
            }).StartingIn(k_TreeViewInitialSelectionDelay);
        }

        static void BindItem(VisualElement element, ITreeViewItem item)
        {
            var target = item as SystemScheduleWindow.SystemTreeViewItem;
            var systemInformationElement = element as SystemInformationVisualElement;
            if (null == systemInformationElement)
                return;

            systemInformationElement.Target = target;
        }
    }
}
