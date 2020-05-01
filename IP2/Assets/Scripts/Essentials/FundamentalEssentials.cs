using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis {
    X,
    Y,
    Z
}

public enum Plane {
    XY,
    YZ,
    XZ
}

namespace Fundamentals {
    public class MathUtils {
        public static float LinearInterpolate(float current, float target, float interpolationValue) {
            if(current == target) return target;
            float dt = Time.deltaTime;
            if(Mathf.Abs(target - current) <= interpolationValue * dt) return target;
            if (current > target) {
                return current - interpolationValue * dt;
            } else {
                return current + interpolationValue * dt;
            }
        }
            
        public static float Clamp(float current, float lower, float upper) {
            if(current < lower) current = lower;
            else if (current > upper) current = upper;
            return current;
        }
    }
}