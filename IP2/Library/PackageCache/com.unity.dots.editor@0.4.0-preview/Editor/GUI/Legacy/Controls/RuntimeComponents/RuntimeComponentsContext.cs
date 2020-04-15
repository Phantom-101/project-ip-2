using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Editor.Bridge;
using Unity.Entities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using GUIUtility = UnityEngine.GUIUtility;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer
    {
        enum VisitOperation
        {
            Layout,
            Draw
        }

        struct PathScope : IDisposable
        {
            readonly Context m_Context;
            public PathScope(Context context) => m_Context = context;
            public void Dispose() => m_Context.Pop();
        }

        class Context
        {
            static readonly PropertyPath s_PathRemap = new PropertyPath();

            static readonly int s_FoldoutHash = "Foldout".GetHashCode();
            
            static readonly float[] s_VectorComponents = new float[4];
            
            static readonly GUIContent[] s_VectorComponentsLabels = new GUIContent[4]
            {
                new GUIContent("X"),
                new GUIContent("Y"),
                new GUIContent("Z"),
                new GUIContent("W")
            };

            static readonly GUIContent s_TempContent1 = new GUIContent();
            static readonly GUIContent s_TempContent2 = new GUIContent();

            Rect m_Rect;
            Rect m_LastRect;
            
            readonly PropertyPath m_PropertyPath = new PropertyPath();
            readonly RuntimeComponentsDrawer m_Drawer;

            public float HeightBuffer { get; private set; }
            public VisitOperation Operation { get; set; } = VisitOperation.Draw;
            public float DistanceBetweenFields => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            public Context(RuntimeComponentsDrawer drawer)
            {
                m_Drawer = drawer;
                m_Rect = m_LastRect = new Rect(0, 0, 0, EditorGUIUtility.singleLineHeight);
            }
            
            public void ResetHeight()
            {
                HeightBuffer = 0;
            }

            public Rect GetCurrent()
            {
                return m_Rect;
            }

            public void SetCurrent(Rect rect)
            {
                m_Rect = rect;
                m_PropertyPath.Clear();
            }

            public IDisposable MakePathScope(string name, int index = -1)
            {
                Push(name, index);
                return new PathScope(this);
            }

            public void Push(string name, int index = -1)
            {
                if (index != -1)
                {
                    name = m_PropertyPath[m_PropertyPath.PartsCount - 1].Name;
                    m_PropertyPath.Pop();
                    m_PropertyPath.Push(name, index);
                }
                else
                {
                    m_PropertyPath.Push(name);
                }
            }

            public void Pop()
            {
                if (m_PropertyPath[m_PropertyPath.PartsCount - 1].Index != -1)
                {
                    var name = m_PropertyPath[m_PropertyPath.PartsCount - 1].Name;
                    m_PropertyPath.Pop();
                    m_PropertyPath.Push(name, -1);
                }
                else
                {
                    m_PropertyPath.Pop();
                }
            }

            /// <summary>
            /// Returns true if the value at the given path is the equal for all targets.
            ///
            /// @TODO Optimization!
            /// </summary>
            static bool IsMixed<TValue>(IReadOnlyList<EntityContainer> targets, PropertyPath path, TValue value)
            {
                if (targets.Count <= 0)
                    return true;
                
                var target = targets[0];

                if (path.PartsCount <= 1 && path[0].Index == -1)
                    return true;
                
                using (var types = target.EntityManager.GetComponentTypes(target.Entity))
                {
                    // Get the component we care about.
                    var componentType = types[path[0].Index].GetManagedType();

                    for (var index = 1; index < targets.Count; index++)
                    {
                        var data = targets[index];

                        var componentIndex = GetComponentIndex(data, componentType);
                        
                        if (componentIndex == -1)
                            return true;
                        
                        s_PathRemap.Clear();
                        s_PathRemap.Push("Components", componentIndex);

                        for (var p = 1; p < path.PartsCount; p++)
                            s_PathRemap.Push(path[p].Name, path[p].Index);
                        
                        var container = new EntityContainer(data.EntityManager, data.Entity);
                        if (!PropertyContainer.TryGetValueAtPath<EntityContainer, TValue>(ref container, s_PathRemap, out var other) || !value.Equals(other))
                            return true;
                    }
                }

                return false;
            }

            static int GetComponentIndex(EntityContainer data, Type type)
            {
                using (var types = data.EntityManager.GetComponentTypes(data.Entity))
                {
                    for (var i = 0; i < types.Length; i++)
                    {
                        if (types[i] == type)
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }

            public bool IsWide => EditorGUIUtility.wideMode;
            public Rect LastRect => m_LastRect;

            Rect GetNext()
            {
                return GetNext(EditorGUIUtility.singleLineHeight);
            }

            Rect GetNext(float height)
            {
                var rect = m_Rect;
                rect.height = height;
                Advance(height);
                return rect;
            }

            public Rect Advance(ref Rect rect)
            {
                return Advance(ref rect, DistanceBetweenFields);
            }

            Rect Advance(ref Rect rect, float height)
            {
                rect.y += height;
                return rect;
            }

            void Advance(float height)
            {
                m_LastRect = m_Rect;
                m_LastRect.height = height;
                var heightAndSpacing = height + EditorGUIUtility.standardVerticalSpacing;
                m_Rect.y += heightAndSpacing;
                HeightBuffer += heightAndSpacing;
            }

            public VisitStatus DoFieldAsLabel<TProperty, TContainer, TValue>(TProperty property, ref TContainer container,
                ref TValue value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, TValue>
            {
                return DoField(property, ref container, ref value, ref changeTracker, (rect, label, val) =>
                {
                    var richText = EditorStyles.label.richText;
                    EditorStyles.label.richText = true;
                    EditorGUI.LabelField(rect, label, GetLabelContent(val));
                    EditorStyles.label.richText = richText;
                    return val;
                });
            }

            public GUIContent GetLabelContent<TValue>(TValue value)
            {
                if (IsMixed(m_Drawer.m_Targets, m_PropertyPath, value))
                {
                    return EditorGUIBridge.mixedValueContent;
                }
                
                string labelValue;

                if (value is IFormattable formattable)
                {
                    labelValue = formattable.ToString("0.##", NumberFormatInfo.CurrentInfo);
                }
                else
                {
                    labelValue = value.ToString();
                }

                return TempContent($"<b>{labelValue}</b>");
            }

            public VisitStatus DoField<TProperty, TContainer, TValue>(TProperty property, ref TContainer container,
                ref TValue value, ref ChangeTracker changeTracker, Func<Rect, GUIContent, TValue, TValue> drawer)
                where TProperty : IProperty<TContainer, TValue>
                => DoField(property, ref container, ref value, ref changeTracker, drawer, EditorGUIUtility.singleLineHeight);
                
            public VisitStatus DoField<TProperty, TContainer, TValue>(TProperty property, ref TContainer container,
                ref TValue value, ref ChangeTracker changeTracker, Func<Rect, GUIContent, TValue, TValue> drawer,
                float fieldHeight)
                where TProperty : IProperty<TContainer, TValue>
            {
                var rect = GetNext(fieldHeight);
                
                if (VisitOperation.Layout == Operation)
                {
                    return VisitStatus.Handled;
                }

                using (MakePathScope(property.GetName()))
                {
                    EditorGUI.BeginChangeCheck();

                    value = drawer(rect, new GUIContent(property.GetName()), value);

                    if (EditorGUI.EndChangeCheck())
                    {
                        changeTracker.MarkChanged();
                    }
                }
                
                return VisitStatus.Handled;
            }

            public void LabelField(string label)
            {
                LabelField(new GUIContent(label));
            }
            
            public void LabelField(GUIContent label)
            {
                var rect = GetNext(); 
                if (VisitOperation.Layout == Operation)
                {
                    return;
                }

                EditorGUI.LabelField(rect, label);
            }

            public void LabelField(string label1, string label2)
            {
                LabelField(new GUIContent(label1), new GUIContent(label2));
            }

            public void LabelField(GUIContent label1, GUIContent label2)
            {
                var rect = GetNext();
                if (VisitOperation.Layout == Operation)
                {
                    return;
                }

                EditorGUI.LabelField(rect, label1, label2);
            }
            
            public void LabelField(GUIContent label, GUIStyle style)
            {
                var rect = GetNext(); 
                if (VisitOperation.Layout == Operation)
                {
                    return;
                }

                EditorGUI.LabelField(rect, label, style);
            }

            public void LabelField(GUIContent label1, GUIContent label2, GUIStyle style)
            {
                var rect = GetNext();
                if (VisitOperation.Layout == Operation)
                {
                    return;
                }

                EditorGUI.LabelField(rect, label1, label2, style);
            }

            public void CustomGUI(Action<Rect> handler)
            {
                CustomGUI(handler, EditorGUIUtility.singleLineHeight);
            }
            
            public void CustomGUI(Action<Rect> handler, float height)
            {
                var rect = GetNext();
                if (VisitOperation.Layout == Operation)
                {
                    return;
                }

                handler?.Invoke(rect);
            }

            public void CustomGUIWithLastRect(Action<Rect> handler)
            {
                if (VisitOperation.Layout == Operation)
                {
                    return;
                }
                handler?.Invoke(m_LastRect);
            }

            public void Space(float height)
            {
                GetNext(height);
            }

            public GUIContent TempContent(string text) => TempContent(text, null, string.Empty);
            public GUIContent TempContent(string text, string tooltip) => TempContent(text, null, tooltip);

            public GUIContent TempContent2(string text) => TempContent2(text, null, string.Empty);
            public GUIContent TempContent2(string text, string tooltip) => TempContent2(text, null, tooltip);

            public static GUIContent TempContent(string text, Texture image, string tooltip)
            {
                return TempContent(s_TempContent1, text, image, tooltip);
            }

            static GUIContent TempContent2(string text, Texture image, string tooltip)
            {
                return TempContent(s_TempContent2, text, image, tooltip);
            }

            static GUIContent TempContent(GUIContent temp, string text, Texture image, string tooltip)
            {
                temp.text = text;
                temp.image = image;
                temp.tooltip = tooltip;
                return temp;
            }
            
            public Vector2 Vector2Label(Rect position, GUIContent label, Vector2 value)
            {
                var controlId = GUIUtility.GetControlID(s_FoldoutHash, FocusType.Keyboard, position);
                position = MultiFieldPrefixLabel(position, controlId, label, 3);
                position.height = 18f;
                return Vector2Label(position, value);
            }

            Vector2 Vector2Label(Rect position, Vector2 value)
            {
                s_VectorComponents[0] = value.x;
                s_VectorComponents[1] = value.y;
                position.height = 18f;
                MultiFloatFieldAsLabel(position, s_VectorComponentsLabels, s_VectorComponents, 2);
                return value;
            }
            
            public Vector3 Vector3Label(Rect position, GUIContent label, Vector3 value)
            {
                var controlId = GUIUtility.GetControlID(s_FoldoutHash, FocusType.Keyboard, position);
                position = MultiFieldPrefixLabel(position, controlId, label, 3);
                position.height = 18f;
                return Vector3Label(position, value);
            }

            Vector3 Vector3Label(Rect position, Vector3 value)
            {
                s_VectorComponents[0] = value.x;
                s_VectorComponents[1] = value.y;
                s_VectorComponents[2] = value.z;
                position.height = 18f;
                MultiFloatFieldAsLabel(position, s_VectorComponentsLabels, s_VectorComponents, 3);
                return value;
            }
            
            public Vector4 Vector4Label(Rect position, GUIContent label, Vector4 value)
            {
                var controlId = GUIUtility.GetControlID(s_FoldoutHash, FocusType.Keyboard, position);
                position = MultiFieldPrefixLabel(position, controlId, label, 4);
                position.height = 18f;
                return Vector4FieldNoIndent(position, value);
            }
            
            internal static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns)
            {
                if (!LabelHasContent(label))
                    return EditorGUI.IndentedRect(totalPosition);
                if (EditorGUIUtility.wideMode)
                {
                    var labelPosition = new Rect(totalPosition.x + EditorGUI.indentLevel, totalPosition.y, EditorGUIUtility.labelWidth - EditorGUI.indentLevel, 18f);
                    var rect = totalPosition;
                    rect.xMin += EditorGUIUtility.labelWidth + 2f;
                    if (columns == 2)
                    {
                        var num = (float) ((rect.width - 8.0) / 3.0);
                        rect.xMax -= num + 4f;
                    }

                    if (null != label)
                    {
                        EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, id);
                    }

                    return rect;
                }
                var labelPosition1 = new Rect(totalPosition.x + EditorGUI.indentLevel, totalPosition.y, totalPosition.width - EditorGUI.indentLevel, 18f);
                var rect1 = totalPosition;
                rect1.xMin += EditorGUI.indentLevel + 15f;
                rect1.yMin += 18f;
                EditorGUI.HandlePrefixLabel(totalPosition, labelPosition1, label, id);
                return rect1;
            }

            Vector4 Vector4FieldNoIndent(Rect position, Vector4 value)
            {
                s_VectorComponents[0] = value.x;
                s_VectorComponents[1] = value.y;
                s_VectorComponents[2] = value.z;
                s_VectorComponents[3] = value.w;
                position.height = 18f;
                MultiFloatFieldAsLabel(position, s_VectorComponentsLabels, s_VectorComponents, 4);
                return value;
            }
            
            internal void MultiFloatFieldAsLabel(Rect position, GUIContent[] subLabels, float[] values, int count)
            {
                var length = values.Length;
                var num = (position.width - (float) (length - 1) * 4f) / (float) length;
                var position1 = new Rect(position)
                {
                    width = num
                };
                var labelWidth = EditorGUIUtility.labelWidth;
                var indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                for (var index = 0; index < count; ++index)
                {
                    EditorGUIUtility.labelWidth = CalcPrefixLabelWidth(subLabels[index], (GUIStyle) null);
                    var richText = EditorStyles.label.richText;
                    EditorStyles.label.richText = true;

                    using (MakePathScope(subLabels[index].text.ToLower()))
                    {
                        var content = IsMixed(m_Drawer.m_Targets, m_PropertyPath, values[index])
                            ? EditorGUIBridge.mixedValueContent
                            : TempContent($"<b>{values[index]:0.##}</b>", null, string.Empty);
                    
                        EditorGUI.LabelField(position1, subLabels[index], content);
                        EditorStyles.label.richText = richText;
                        position1.x += num + 4f; 
                    }
                }
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel = indentLevel;
            }

            static float CalcPrefixLabelWidth(GUIContent label, GUIStyle style = null)
            {
                if (style == null)
                    style = EditorStyles.label;
                return style.CalcSize(label).x;
            }

            static bool LabelHasContent(GUIContent label)
            {
                if (label == null)
                    return true;
                
                return label.text != string.Empty || label.image != null;
            }
        }
    }
}
