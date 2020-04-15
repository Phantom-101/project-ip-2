using System.Linq;
using Unity.Properties;
using UnityEditor;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer
    {
        sealed class ImguiPrimitivesAdapter : ImguiAdapter,
            IVisitAdapterPrimitives
        {
            public ImguiPrimitivesAdapter(Context context) : base(context)
            {
            }

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref sbyte value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, sbyte>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref short value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, short>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref int value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, int>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref long value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, long>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref byte value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, byte>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref ushort value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, ushort>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref uint value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, uint>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref ulong value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, ulong>
                => Context.DoField(property, ref container, ref value, ref changeTracker, (rect, label, val) =>
                {
                    var richText = EditorStyles.label.richText;
                    EditorStyles.label.richText = true;
                    EditorGUI.LabelField(rect, label, Context.GetLabelContent(val));
                    EditorStyles.label.richText = richText;
                    return val;
                });

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref float value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, float>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref double value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, double>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref bool value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, bool>
                => Context.DoFieldAsLabel(property, ref container, ref value, ref changeTracker);

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref char value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, char>
                => Context.DoField(property, ref container, ref value, ref changeTracker,
                    (rect, label, val) =>
                    {
                        var richText = EditorStyles.label.richText;
                        EditorStyles.label.richText = true;
                        EditorGUI.LabelField(rect, label, Context.GetLabelContent(val));
                        EditorStyles.label.richText = richText;
                        return val;
                    });
        }
    }
}
