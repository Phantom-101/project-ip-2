using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Essentials;

public class PlayerController : MonoBehaviour {
    [Header ("Basics")]
    public StructureBehaviours structureBehaviours;
    [Header ("UI Elements")]
    public Slider forwardPowerSlider;
    public Button turnLeftButton;
    public Button turnRightButton;
    public float turnValue;
    [Header ("Misc")]
    public bool initialized;

    public void Initialize (StructureBehaviours initializer) {
        structureBehaviours = initializer;
        initialized = true;
    }

    void Update () {
        if (!initialized) return;
        if (structureBehaviours.engine.engine != null) {
            structureBehaviours.engine.forwardSetting = forwardPowerSlider.value;
            structureBehaviours.engine.turnSetting = turnValue;
        }
    }

    public void TurnLeftButtonDown () {
        turnValue -= 1.0f;
    }

    public void TurnLeftButtonUp () {
        turnValue += 1.0f;
    }

    public void TurnRightButtonDown () {
        turnValue += 1.0f;
    }

    public void TurnRightButtonUp () {
        turnValue -= 1.0f;
    }
}
