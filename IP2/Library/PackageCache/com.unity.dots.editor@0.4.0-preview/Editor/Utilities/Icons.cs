using UnityEditor;
using UnityEngine;
using Unity.Editor.Bridge;

namespace Unity.Editor
{
    static class Icons
    {
        const string k_IconsDirectory = Constants.EditorDefaultResourcesPath + "/icons";

        public static Texture2D RuntimeComponent { get; private set; }
        public static Texture2D Remove { get; private set; }
        public static Texture2D RoundedCorners { get; private set; }
        public static Texture2D Entity { get; private set;}
        public static Texture2D Filter { get; private set;}
        public static Texture2D Convert { get; private set;}

        static Icons()
        {
            LoadIcons();
        }

        static void LoadIcons()
        {
            RuntimeComponent = LoadIcon(nameof(RuntimeComponent));
            Remove = LoadIcon(nameof(Remove));
            RoundedCorners = LoadIcon(nameof(RoundedCorners));
            Entity = LoadIcon(nameof(Entity));
            Filter = LoadIcon(nameof(Filter));
            Convert = LoadIcon(nameof(Convert));
        }

        /// <summary>
        /// Workaround for `EditorGUIUtility.LoadIcon` not working with packages. This can be removed once it does
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static Texture2D LoadIcon(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (EditorGUIUtility.isProSkin)
            {
                name = "d_" + name;
            }

            // Try to use high DPI if possible
            if (Bridge.GUIUtility.pixelsPerPoint > 1.0)
            {
                var texture = LoadIconTexture($"{k_IconsDirectory}/{name}@2x.png");

                if (null != texture)
                {
                    return texture;
                }
            }

            // Fallback to low DPI if we couldn't find the high res or we are on a low res screen
            return LoadIconTexture($"{k_IconsDirectory}/{name}.png");
        }

        static Texture2D LoadIconTexture(string path)
        {
            var texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

            if (texture != null &&
                !Mathf.Approximately(texture.GetPixelsPerPoint(), (float)Bridge.GUIUtility.pixelsPerPoint) &&
                !Mathf.Approximately((float)Bridge.GUIUtility.pixelsPerPoint % 1f, 0.0f))
            {
                texture.filterMode = FilterMode.Bilinear;
            }

            return texture;
        }
    }
}
