using Unity.Properties;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer
    {
        class ImguiAdapter : IPropertyVisitorAdapter
        {
            protected Context Context { get; }

            protected ImguiAdapter(Context context)
            {
                Context = context;
            }
        }
    }
}
