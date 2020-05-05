using UnityEngine;

namespace Essentials {
    public static class MathUtils {
        public static float Clamp (float current, float min, float max) {
            return Mathf.Min(Mathf.Max(current, min), max);
        }
    }

    public static class InputDetector {
        public static bool GetKeyDown(KeyCode keyCode, bool control = false, bool shift = false, bool alt = false) {
            return (Input.GetKeyDown(keyCode) &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) == control &&
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) == shift &&
                (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) == alt);
        }

        public static bool GetKeyUp(KeyCode keyCode, bool control = false, bool shift = false, bool alt = false) {
            return (Input.GetKeyUp(keyCode) &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) == control &&
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) == shift &&
                (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) == alt);
        }

        public static bool GetKey(KeyCode keyCode, bool control = false, bool shift = false, bool alt = false) {
            return (Input.GetKey(keyCode) &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) == control &&
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) == shift &&
                (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) == alt);
        }
    }
}