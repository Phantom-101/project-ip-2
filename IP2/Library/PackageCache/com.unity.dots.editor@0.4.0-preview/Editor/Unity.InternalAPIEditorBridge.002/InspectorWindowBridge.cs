using UnityEditor;

namespace Unity.Editor.Bridge
{
    static class InspectorWindowBridge
    {
        public static void RepaintAllInspectors() => InspectorWindow.RepaintAllInspectors();
    }
}
