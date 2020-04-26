using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputEssentials {
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