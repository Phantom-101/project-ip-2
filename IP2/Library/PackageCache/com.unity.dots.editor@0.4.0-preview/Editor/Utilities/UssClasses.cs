namespace Unity.Editor
{
    static class UssClasses
    {
        public static class Resources
        {
            public const string SystemSchedule = "system-schedule__resources";
        }

        public static class SystemScheduleWindow
        {
            public const string SystemSchedule = "system-schedule";
            public const string ToolbarContainer = SystemSchedule + "__toolbar-container";
            public const string SearchField = SystemSchedule + "__search-field";

            public static class TreeView
            {
                public const string Header = SystemSchedule + "__tree-view__header";
                public const string System = SystemSchedule + "__tree-view__system-label";
                public const string Matches = SystemSchedule + "__tree-view__matches-label";
                public const string Time = SystemSchedule + "__tree-view__time-label";
            }

            public static class Items
            {
                const string Base = SystemSchedule + "-item";
                public const string Icon = Base + "__icon";
                public const string Enabled = Base + "__enabled-toggle";
                public const string SystemName = Base + "__name-label";
                public const string Matches = Base + "__matches-label";
                public const string Time = Base + "__time-label";

                public const string SystemIcon = Icon + "--system";
                public const string SystemGroupIcon = Icon + "--system-group";
                public const string CommandBufferIcon = Icon + "--command-buffer";
                public const string PlayerLoopIcon = Icon + "--player-loop";
            }
        }
    }
}
