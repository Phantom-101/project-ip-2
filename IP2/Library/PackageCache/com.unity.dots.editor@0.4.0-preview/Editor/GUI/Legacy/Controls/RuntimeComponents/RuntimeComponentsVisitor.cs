using System;
using System.Collections.Generic;
using System.Linq;
using Unity.DOTS.Editor;
using Unity.Editor.Controls;
using Unity.Entities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer
    {
        sealed class Visitor : PropertyVisitor
        {
            const int k_BufferPageLength = 5;

            readonly Dictionary<int, PaginationField> m_Pagination = new Dictionary<int, PaginationField>();
            readonly RuntimeComponentsDrawer m_Drawer;

            Context Context => m_Drawer.m_Context;
            
            public Visitor(RuntimeComponentsDrawer drawer)
            {
                m_Drawer = drawer;
                AddAdapter(new ImguiPrimitivesAdapter(Context));
                AddAdapter(new ImguiMathematicsAdapter(Context));
                AddAdapter(new ImguiEntityAdapter(Context));
            }

            public void OnGUI<TContainer>(Rect rect, TContainer container)
            {
                m_Drawer.m_Context.SetCurrent(rect);
                PropertyContainer.Visit(ref container, this);
            }

            public float CalculateHeight<TContainer>(TContainer container)
            {
                var operation = Context.Operation;
                var position = Context.GetCurrent();
                Context.ResetHeight();

                try
                {
                    Context.Operation = VisitOperation.Layout;
                    PropertyContainer.Visit(ref container, this);
                }
                finally
                {
                    Context.Operation = operation;
                    Context.SetCurrent(position);
                }

                return Context.HeightBuffer;
            }

            public override bool IsExcluded<TProperty, TContainer, TValue>(TProperty property, ref TContainer container)
            {
                // For this particular visitor, we don't want to show the source entity.
                if (typeof(EntityContainer).IsAssignableFrom(typeof(TContainer)) && typeof(Entity).IsAssignableFrom(typeof(TValue)))
                {
                    return true;
                }

                return base.IsExcluded<TProperty, TContainer, TValue>(property, ref container);
            }

            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property,
                ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                if (typeof(TValue).IsEnum)
                {
                    Context.DoField(property, ref container, ref value, ref changeTracker, (rect, content, val) =>
                    {
                        var options = Enum.GetNames(typeof(TValue)).ToArray();
                        var local = val;
                        var index = EditorGUI.Popup(
                            rect,
                            typeof(TValue).Name,
                            Array.FindIndex(options, name => name == local.ToString()),
                            options);

                        return (TValue)Enum.Parse(typeof(TValue), options[index]);
                    });
                }
                else
                {
                    Context.LabelField(property.GetName());
                }

                return VisitStatus.Handled;
            }

            protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property,
                ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                // We only show components that are in the type filter.
                if (IsComponentType(ref value, true))
                {
                    var typeIndex = TypeManager.GetTypeIndex(GetComponentType(ref value));
                    if (!m_Drawer.m_ComponentTypes.Contains(typeIndex))
                    {
                        return VisitStatus.Override;
                    }
                }
                
                if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue)))
                {
                    Context.DoField(property, ref container, ref value, ref changeTracker, (rect, label, v) =>
                    {
                        EditorGUI.ObjectField(rect, label, v as UnityEngine.Object, typeof(TValue), false);
                        return v;
                    });
                    return VisitStatus.Override;
                }
                
                var propertyName = property.GetName();

                using (Context.MakePathScope(propertyName, (property as ICollectionElementProperty)?.Index ?? -1))
                {
                    var isComponentRoot = IsComponentType(ref value); 
                    if (isComponentRoot)
                    {
                        ContainerHeader(propertyName, ref value);
                    }
                    else
                    {
                        Context.LabelField(propertyName);
                    }
                
                    EditorGUI.indentLevel++;

                    if (null != value)
                    {
                        PropertyContainer.Visit(ref value, this);
                    }
                    else
                    {
                        Context.LabelField(propertyName, "Value is null");
                    }

                    EditorGUI.indentLevel--;

                    if (isComponentRoot)
                    {
                        Context.Space(2.0f);
                    }
                }
                
                return VisitStatus.Override;
            }

            protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property,
                ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Context.Push(property.GetName());
                if (container is IDynamicBufferContainer buffer)
                {
                    if (typeof(IBufferElementData).IsAssignableFrom(buffer.ElementType))
                    {
                        var typeIndex = TypeManager.GetTypeIndex(buffer.ElementType);
                        if (!m_Drawer.m_ComponentTypes.Contains(typeIndex))
                        {
                            return VisitStatus.Override;
                        }
                    }
                }
                var count = property.GetCount(ref container);

                if (count == 0)
                {
                    Context.LabelField("Buffer is empty");
                }

                // If the count is greater then some arbitrary value
                if (container is IDynamicBufferContainer bufferContainer && count > k_BufferPageLength)
                {
                    var hash = bufferContainer.GetHashCode();
                    if (!m_Pagination.ContainsKey(hash))
                    {
                        m_Pagination.Add(hash, new PaginationField {ItemsPerPage = k_BufferPageLength});
                    }

                    var scrollData = m_Pagination[hash];
                    scrollData.Count = count;

                    for (var i = scrollData.Page * k_BufferPageLength;
                        i < (scrollData.Page + 1) * k_BufferPageLength && i < count;
                        i++)
                    {
                        var callback = new VisitCollectionElementCallback<TContainer>(this);
                        property.GetPropertyAtIndex(ref container, i, ref changeTracker, ref callback);
                    }

                    Context.CustomGUI(scrollData.OnGUI, 20);
                    return VisitStatus.Override;
                }

                // Otherwise business as usual.
                return VisitStatus.Handled;
            }
            
            protected override void EndCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Context.Pop();
                base.EndCollection(property, ref container, ref value, ref changeTracker);
            }

            static bool IsComponentType<TValue>(ref TValue value, bool includeBufferElements = false)
            {
                return typeof(IComponentData).IsAssignableFrom(typeof(TValue))
                       || typeof(ISharedComponentData).IsAssignableFrom(typeof(TValue))
                       || value is IDynamicBufferContainer
                       || (includeBufferElements && typeof(IBufferElementData).IsAssignableFrom(typeof(TValue)));
            }

            static Type GetComponentType<TValue>(ref TValue value)
            {
                if (typeof(IComponentData).IsAssignableFrom(typeof(TValue))
                    || typeof(ISharedComponentData).IsAssignableFrom(typeof(TValue))
                    || typeof(IBufferElementData).IsAssignableFrom(typeof(TValue)))
                {
                    return typeof(TValue);
                }

                if (value is IDynamicBufferContainer buffer)
                {
                    return buffer.ElementType;
                }

                return null;
            }

            static string GetComponentCategory<TValue>(ref TValue value)
            {
                if (typeof(IComponentData).IsAssignableFrom(typeof(TValue)))
                {
                    if (typeof(TValue).IsClass)
                    {
                        return " (Managed)";
                    }
                    return string.Empty;
                }

                if (typeof(ISharedComponentData).IsAssignableFrom(typeof(TValue)))
                {
                    return " (Shared)";
                }

                if (value is IDynamicBufferContainer)
                {
                    return " (Buffer)";
                }

                return string.Empty;
            }

            void ContainerHeader<TValue>(string displayName, ref TValue value)
            {
                Context.LabelField(new GUIContent {text = " " + displayName + GetComponentCategory(ref value), image = Icons.RuntimeComponent}, IsComponentType(ref value) ? EditorStyles.boldLabel : EditorStyles.label);

                Type toRemove;

                if (value is IDynamicBufferContainer container)
                {
                    toRemove = container.ElementType;
                }
                else
                {
                    toRemove = typeof(TValue);
                }
                
                Context.CustomGUIWithLastRect(rect =>
                {
                    rect.x = rect.x + rect.width - 16.0f;
                    rect.width = 12.0f;
                    if (GUI.Button(rect, Icons.Remove, EditorStyles.label))
                    {
                        var typeIndex = TypeManager.GetTypeIndex(toRemove);
                        m_Drawer.onRemoveComponent?.Invoke(typeIndex);
                        GUIUtility.ExitGUI();
                    }    
                });
            }
        }
        
        // @TODO Remove once this is properly exposed in `com.unity.properties` 
        struct VisitCollectionElementCallback<TContainer> : ICollectionElementPropertyGetter<TContainer>
        {
            readonly IPropertyVisitor m_Visitor;
        
            public VisitCollectionElementCallback(IPropertyVisitor visitor)
            {
                m_Visitor = visitor;
            }

            public void VisitProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TElementProperty : ICollectionElementProperty<TContainer, TElement>
            {
                m_Visitor.VisitProperty<TElementProperty, TContainer, TElement>(property, ref container, ref changeTracker);
            }

            public void VisitCollectionProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TElementProperty : ICollectionProperty<TContainer, TElement>, ICollectionElementProperty<TContainer, TElement>
            {
                m_Visitor.VisitCollectionProperty<TElementProperty, TContainer, TElement>(property, ref container, ref changeTracker);
            }
        }
    }
}
