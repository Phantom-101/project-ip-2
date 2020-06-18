using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public Slider dampenerSlider;
    public bool leftPressed;
    public bool rightPressed;
    [Header ("Materials")]
    public GraphicsManager graphicsManager;

    void Awake () {
        uIHandler = FindObjectOfType<UIHandler> ();
        graphicsManager = FindObjectOfType<GraphicsManager> ();
    }

    void Update () {
        if (structureBehaviours == null) return;
        uIHandler.source = structureBehaviours;
        // Skybox stuff
        Sector inSector = structureBehaviours.transform.parent.GetComponent<Sector> ();
        if (inSector != null) RenderSettings.skybox = graphicsManager.skyboxes[inSector.sectorData.skyboxID];
        if (structureBehaviours.AI == null) {
            structureBehaviours.dampening = dampenerSlider.value;
            if (structureBehaviours.engine.engine != null) {
                structureBehaviours.engine.forwardSetting = forwardPowerSlider.value;
                structureBehaviours.engine.turnSetting = (leftPressed ? -1.0f : 0.0f) + (rightPressed ? 1.0f : 0.0f);
            }
            if (Input.GetMouseButtonDown (0) && !PointerOverUIObject ()) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, 5000.0f)) {
                    GameObject hitGameObject = hit.transform.gameObject;
                    if (hitGameObject != structureBehaviours.gameObject && hitGameObject.transform.parent == structureBehaviours.transform.parent) {
                        StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
                        if (hitStructureBehaviours != null && hitStructureBehaviours != structureBehaviours.targeted) structureBehaviours.targeted = hitStructureBehaviours;
                        else structureBehaviours.targeted = null;
                    }
                }
            }
        }
    }

    public bool PointerOverUIObject () {
        PointerEventData eventDataCurrentPosition = new PointerEventData (EventSystem.current);
        eventDataCurrentPosition.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult> ();
        EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
        return results.Count > 0;
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

    public void Reset () {
        forwardPowerSlider.value = 0.0f;
        leftPressed = false;
        rightPressed = false;
    }

    public void Dock () {
        if (structureBehaviours.targeted == null) return;
        structureBehaviours.targeted.Dock (structureBehaviours);
    }

    public void Undock () {
        StructureBehaviours stationStructureBehaviours = structureBehaviours.transform.parent.GetComponent<StructureBehaviours> ();
        if (stationStructureBehaviours == null) return;
        stationStructureBehaviours.Undock (structureBehaviours);
    }
}
