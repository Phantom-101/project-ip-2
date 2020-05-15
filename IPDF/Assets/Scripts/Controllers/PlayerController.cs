using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Essentials;

public class PlayerController : MonoBehaviour {
    [Header ("Basics")]
    public StructureBehaviours structureBehaviours;
    [Header ("UI Elements")]
    public UIHandler uIHandler;
    public Slider forwardPowerSlider;
    public Button turnLeftButton;
    public Button turnRightButton;
    public bool leftPressed;
    public bool rightPressed;

    void Awake () {
        uIHandler = FindObjectOfType<UIHandler> ();
    }

    void Update () {
        if (!structureBehaviours) return;
        uIHandler.source = structureBehaviours;
        if (structureBehaviours.AIActivated) {
            // Do not act according to player inputs
        } else {
            if (structureBehaviours.engine.engine != null) {
                structureBehaviours.engine.forwardSetting = forwardPowerSlider.value;
                structureBehaviours.engine.turnSetting = (leftPressed ? -1.0f : 0.0f) + (rightPressed ? 1.0f : 0.0f);
            }
            if (Input.GetMouseButton (0)) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, 1000.0f)) {
                    GameObject hitGameObject = hit.transform.gameObject;
                    if (hitGameObject != structureBehaviours.gameObject) {
                        StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
                        if (hitStructureBehaviours != null) {
                            structureBehaviours.targetted = hitStructureBehaviours;
                        }
                    }
                }
            }
        }
    }

    public void TurnLeftButtonDown () {
        leftPressed = true;
    }

    public void TurnLeftButtonUp () {
        leftPressed = false;
    }

    public void TurnRightButtonDown () {
        rightPressed = true;
    }

    public void TurnRightButtonUp () {
        rightPressed = false;
    }
}
