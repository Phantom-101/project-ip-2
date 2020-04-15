using Unity.Mathematics;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Legacy
{
    internal sealed partial class RuntimeComponentsDrawer
    {
        sealed class ImguiMathematicsAdapter : ImguiAdapter
            , IVisitAdapter<quaternion>
            , IVisitAdapter<float2>
            , IVisitAdapter<float3>
            , IVisitAdapter<float4>
            , IVisitAdapter<float2x2>
            , IVisitAdapter<float3x3>
            , IVisitAdapter<float4x4>
        {
            public ImguiMathematicsAdapter(Context context) : base(context)
            {
            }

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref quaternion value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, quaternion>
                => Context.DoField(property, ref container, ref value, ref changeTracker,
                    (rect, label, val) =>
                    {
                        using (Context.MakePathScope(nameof(quaternion.value)))
                        {
                            Context.Vector4Label(rect, label, val.value);
                        }
                        return val;
                    },
                    EditorGUIUtility.singleLineHeight * (Context.IsWide ? 1.0f : 2.0f));

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float2 value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float2>
                => Context.DoField(property, ref container, ref value, ref changeTracker,
                    (rect, label, val) =>
                    {
                        Context.Vector2Label(rect, label, val);
                        return val;
                    },
                    EditorGUIUtility.singleLineHeight * (Context.IsWide ? 1.0f : 2.0f));

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float3 value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float3>
                => Context.DoField(property, ref container, ref value, ref changeTracker,
                    (rect, label, val) =>
                    {
                        Context.Vector3Label(rect, label, val);
                        return val;
                    },
                    EditorGUIUtility.singleLineHeight * (Context.IsWide ? 1.0f : 2.0f));

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float4 value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float4>
                => Context.DoField(property, ref container, ref value, ref changeTracker,
                    (rect, label, val) =>
                    {
                        Context.Vector4Label(rect, label, val);
                        return val;
                    },
                    EditorGUIUtility.singleLineHeight * (Context.IsWide ? 1.0f : 2.0f));

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float2x2 value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float2x2>
                => Context.DoField(property, ref container, ref value, ref changeTracker, (rect, label, val) =>
                {
                    if (Context.IsWide)
                    {
                        using (Context.MakePathScope(nameof(float2x2.c0))) 
                            Context.Vector2Label(rect, Context.TempContent(property.GetName()), val.c0);
                        using (Context.MakePathScope(nameof(float2x2.c1))) 
                            Context.Vector2Label(Context.Advance(ref rect), null, val.c1);
                    }
                    else
                    {
                        rect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.LabelField(rect, property.GetName());
                        ++EditorGUI.indentLevel;
                        using (Context.MakePathScope(nameof(float2x2.c0))) 
                            Context.Vector2Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c0);
                        using (Context.MakePathScope(nameof(float2x2.c1))) 
                            Context.Vector2Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c1);
                        --EditorGUI.indentLevel;
                    }

                    return val;
                }, Context.DistanceBetweenFields * (Context.IsWide ? 2 : 3));

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float3x3 value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float3x3>
                => Context.DoField(property, ref container, ref value, ref changeTracker, (rect, label, val) =>
                {
                    if (Context.IsWide)
                    {
                        using (Context.MakePathScope(nameof(float3x3.c0))) 
                            Context.Vector3Label(rect, Context.TempContent(property.GetName()), val.c0);
                        using (Context.MakePathScope(nameof(float3x3.c1))) 
                            Context.Vector3Label(Context.Advance(ref rect), null, val.c1);
                        using (Context.MakePathScope(nameof(float3x3.c2))) 
                            Context.Vector3Label(Context.Advance(ref rect), null, val.c2);
                    }
                    else
                    {
                        rect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.LabelField(rect, property.GetName());
                        ++EditorGUI.indentLevel;
                        using (Context.MakePathScope(nameof(float3x3.c0))) 
                            Context.Vector3Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c0);
                        using (Context.MakePathScope(nameof(float3x3.c1))) 
                            Context.Vector3Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c1);
                        using (Context.MakePathScope(nameof(float3x3.c2))) 
                            Context.Vector3Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c2);
                        --EditorGUI.indentLevel;
                    }

                    return val;
                }, Context.DistanceBetweenFields * (Context.IsWide ? 3 : 4));

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float4x4 value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float4x4>
                => Context.DoField(property, ref container, ref value, ref changeTracker, (rect, label, val) =>
                {
                    if (Context.IsWide)
                    {
                        using (Context.MakePathScope(nameof(float4x4.c0))) 
                            Context.Vector4Label(rect, Context.TempContent(property.GetName()), val.c0);
                        using (Context.MakePathScope(nameof(float4x4.c1)))
                            Context.Vector4Label(Context.Advance(ref rect), null, val.c1); 
                        using (Context.MakePathScope(nameof(float4x4.c2))) 
                            Context.Vector4Label(Context.Advance(ref rect), null, val.c2);
                        using (Context.MakePathScope(nameof(float4x4.c3))) 
                            Context.Vector4Label(Context.Advance(ref rect), null, val.c3);
                    }
                    else
                    {
                        rect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.LabelField(rect, property.GetName());
                        ++EditorGUI.indentLevel;
                        using (Context.MakePathScope(nameof(float4x4.c0))) 
                            Context.Vector4Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c0);
                        using (Context.MakePathScope(nameof(float4x4.c1)))
                            Context.Vector4Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c1);
                        using (Context.MakePathScope(nameof(float4x4.c2)))
                            Context.Vector4Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c2);
                        using (Context.MakePathScope(nameof(float4x4.c2)))
                            Context.Vector4Label(Context.Advance(ref rect), Context.TempContent(string.Empty), val.c3);
                        --EditorGUI.indentLevel;
                    }

                    return val;
                }, Context.DistanceBetweenFields * (Context.IsWide ? 4 : 5));
            
            

        }
    }
}
