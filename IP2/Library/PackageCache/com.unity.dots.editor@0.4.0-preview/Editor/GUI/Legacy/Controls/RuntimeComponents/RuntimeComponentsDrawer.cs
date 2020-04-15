using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer
    {
        readonly List<EntityContainer> m_Targets = new List<EntityContainer>();
        readonly HashSet<int> m_ComponentTypes = new HashSet<int>();

        readonly Context m_Context;
        readonly Visitor m_Visitor;

        public event Action<int> onRemoveComponent;

        public RuntimeComponentsDrawer()
        {
            m_Context = new Context(this);
            m_Visitor = new Visitor(this);
        }
        
        public void SetTargets(IEnumerable<EntityContainer> targets)
        {
            m_Targets.Clear();
            
            foreach (var target in targets)
            {
                m_Targets.Add(target);
            }
        }
        
        public void SetComponentTypes(IEnumerable<int> components)
        {
            m_ComponentTypes.Clear();
            
            foreach (var component in components)
            {
                m_ComponentTypes.Add(component);
            }
        }

        public float GetTotalHeight()
        {
            Validate();

            if (m_Targets.Count == 0)
                return 0.0f;

            return m_Visitor.CalculateHeight(GetCurrentContainer()) + 8.0f;
        }

        public void OnGUI(Rect rect)
        {
            Validate();

            if (m_Targets.Count == 0)
                return;

            // Draw background with rounded corners.
            rect.x += 4;
            rect.width -= 8.0f;

            var style = new GUIStyle("HelpBox")
            {
                normal =
                {
                    background = Icons.RoundedCorners
                }
            };

            GUI.color = EditorGUIUtility.isProSkin
                ? new Color32(0x22, 0x22, 0x22, 0xFF)
                : new Color32(0xE4, 0xE4, 0xE4, 0xFF);

            GUI.Box(rect, "", style);
            GUI.color = Color.white;

            // Draw actual runtime components
            rect.x += 4;
            rect.width -= 8.0f;
            rect.y += 4.0f;

            m_Visitor.OnGUI(rect, GetCurrentContainer());
        }

        void Validate()
        {
            m_Targets.RemoveAll(data => !data.EntityManager.Exists(data.Entity));
        }

        EntityContainer GetCurrentContainer()
        {
            return m_Targets.First();
        }
    }
}
