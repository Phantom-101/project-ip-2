namespace Unity.Editor
{
    static class Constants
    {
        public const string PackageName = "com.unity.dots.editor";
        public const string PackagePath = "Packages/" + PackageName;

        public const string EditorDefaultResourcesPath = PackagePath + "/Editor Default Resources/";

        public static class Conversion
        {
            public const string SelectedComponentSessionKey = "Conversion.Selected.{0}.{1}";
            public const string ShowAdditionalEntitySessionKey = "Conversion.ShowAdditional.{0}";
            public const string SelectedAdditionalEntitySessionKey = "Conversion.Additional.{0}";
        }
    }
}