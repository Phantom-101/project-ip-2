using Unity.Entities;
using Unity.Properties;
using UnityEditor;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer
    {
        sealed class ImguiEntityAdapter : ImguiAdapter
            , IVisitAdapter<Entity>
        {
            public ImguiEntityAdapter(Context context) : base(context)
            {
            }

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property,
                ref TContainer container, ref Entity value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, Entity>
            {
                var richText = EditorStyles.label.richText;
                EditorStyles.label.richText = true;
                Context.LabelField(property.GetName(), $"Index: <b>{value.Index}</b>, Version: <b>{value.Version}</b>");
                EditorStyles.label.richText = richText;
                return VisitStatus.Handled;
            }
        }
    }
}
