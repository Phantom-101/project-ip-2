using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour {
    [Header ("Source")]
    public StructureBehaviours source;
    [Header ("Prefabs")]
    public GameObject equipmentButton;
    [Header ("Gradients")]
    public Gradient hullGradient;
    public Gradient shieldGradient;
    public Gradient energyGradient;

    GameObject canvas;
    Image hullUI;
    Image[] shieldUI = new Image[6];
    GameObject targetInformationPanel;
    Image targetHullUI;
    Image[] targetShieldUI = new Image[6];
    TextMeshProUGUI targetName;
    TextMeshProUGUI targetFaction;
    TextMeshProUGUI targetDistance;
    Transform equipmentButtonsParent;
    List<GameObject> equipmentButtons = new List<GameObject> ();

    void Awake () {
        canvas = GameObject.Find ("Canvas");
        hullUI = canvas.transform.Find ("Health Indicators/Hull").GetComponent<Image> ();
        for (int i = 0; i < 6; i++) shieldUI[i] = canvas.transform.Find ("Health Indicators/Hull/Shield " + i).GetComponent<Image> ();
        targetInformationPanel = canvas.transform.Find ("Target Information Panel").gameObject;
        targetHullUI = targetInformationPanel.transform.Find ("Health Indicators/Hull").GetComponent<Image> ();
        for (int i = 0; i < 6; i++) targetShieldUI[i] = targetInformationPanel.transform.Find ("Health Indicators/Hull/Shield " + i).GetComponent<Image> ();
        targetName = targetInformationPanel.transform.Find ("Name").GetComponent<TextMeshProUGUI> ();
        targetFaction = targetInformationPanel.transform.Find ("Faction").GetComponent<TextMeshProUGUI> ();
        targetDistance = targetInformationPanel.transform.Find ("Distance").GetComponent<TextMeshProUGUI> ();
        equipmentButtonsParent = canvas.transform.Find ("Equipment Buttons Parent");
    }

    void Update () {
        if (source == null) return;
        hullUI.color = hullGradient.Evaluate (source.hull / source.profile.hull);
        if (source.shield.shield != null) {
            if (source.shield.online)
                for (int i = 0; i < 6; i++)
                    shieldUI[i].color = shieldGradient.Evaluate (source.shield.strengths[i] / source.shield.shield.strength);
            else
                for (int i = 0; i < 6; i++) shieldUI[i].color = Color.grey;
        }
        StructureBehaviours targetStructureBehaviour = source.targetted;
        if (targetStructureBehaviour == null) targetInformationPanel.SetActive (false);
        else {
            targetInformationPanel.SetActive (true);
            targetHullUI.color = hullGradient.Evaluate (targetStructureBehaviour.hull / targetStructureBehaviour.profile.hull);
            if (targetStructureBehaviour.shield.shield != null) {
                if (targetStructureBehaviour.shield.online) for (int i = 0; i < 6; i++)
                        targetShieldUI[i].color = shieldGradient.Evaluate (targetStructureBehaviour.shield.strengths[i] / targetStructureBehaviour.shield.shield.strength);
                else for (int i = 0; i < 6; i++) targetShieldUI[i].color = Color.grey;
            }
            targetName.text = targetStructureBehaviour.gameObject.name;
            targetFaction.text = "";
            targetDistance.text = System.Math.Round (Vector3.Distance (source.transform.position, targetStructureBehaviour.transform.position), 2) + "m";
        }
        if (equipmentButtons.Count != source.turrets.Count + 2) {
            equipmentButtons = new List<GameObject> ();
            for (int i = 0; i < source.turrets.Count + 2; i++)
                equipmentButtons.Add (null);
        }
        int turretButtonsShift = 0;
        // TODO do the same for the electronics and tractor beam
        for (int i = 0; i < source.turrets.Count; i++) {
            Turret referencedTurret = null;
            GameObject button = equipmentButtons[i];
            if (button != null) {
                ContainerComponent containerComponent = button.GetComponent<ContainerComponent> ();
                if (containerComponent == null) Destroy (button);
                else referencedTurret = containerComponent.containers[0].value as Turret;
                if (referencedTurret != source.turrets[i].turret) Destroy (button);
            }
            if (button == null) {
                button = Instantiate (equipmentButton);
                button.transform.SetParent (equipmentButtonsParent, false);
                ContainerComponent equipmentContainer = button.GetComponent<ContainerComponent> ();
                if (equipmentContainer == null) equipmentContainer = button.AddComponent<ContainerComponent> ();
                equipmentContainer.containers.Add (new Container<Object> (source.turrets[i].turret));
            }
            button.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, (turretButtonsShift + i) * 40.0f);
            button.transform.GetChild (0).GetComponent<Image> ().sprite = referencedTurret == null ? null : referencedTurret.icon;
            button.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedTurret == null ? 0.0f : source.turrets[i].storedEnergy / referencedTurret.maxStoredEnergy * 30.0f, 3.0f);
            button.transform.GetChild (1).GetChild (0).GetComponent<Image> ().color = energyGradient.Evaluate (referencedTurret == null ? 0.0f : source.turrets[i].storedEnergy / referencedTurret.maxStoredEnergy);
            SelectableButtonFunction (() => source.turrets[button.transform.GetSiblingIndex ()].Activate (source.gameObject, source.targetted.gameObject), button.GetComponent<Button> ());
            equipmentButtons[i] = button;
        }
    }

    void SelectableButtonFunction(UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
