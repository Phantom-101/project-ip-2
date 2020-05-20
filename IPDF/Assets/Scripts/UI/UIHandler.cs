﻿using System.Collections;
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
    [Header ("UI Elements")]
    public GameObject canvas;
    public Image hullUI;
    public Image[] shieldUI = new Image[6];
    public RectTransform sourceTo;
    public TextMeshProUGUI speedCounter;
    public GameObject targetInformationPanel;
    public Image targetHullUI;
    public Image[] targetShieldUI = new Image[6];
    public TextMeshProUGUI targetName;
    public TextMeshProUGUI targetFaction;
    public TextMeshProUGUI targetDistance;
    public RectTransform toSource;
    public Transform equipmentButtonsParent;
    public List<GameObject> equipmentButtons = new List<GameObject> ();
    public RectTransform capacitorTransform;
    public GameObject AIInfo;
    public GameObject dockButton;
    public GameObject undockButton;

    void Awake () {
        canvas = GameObject.Find ("Canvas");
        hullUI = canvas.transform.Find ("Health Indicators/Hull").GetComponent<Image> ();
        for (int i = 0; i < 6; i++) shieldUI[i] = hullUI.transform.Find ("Shield " + i).GetComponent<Image> ();
        sourceTo = hullUI.transform.Find ("Angle Arrow").GetComponent<RectTransform> ();
        speedCounter = canvas.transform.Find ("Forward Speed/Speed Counter").GetComponent<TextMeshProUGUI> ();
        targetInformationPanel = canvas.transform.Find ("Target Information Panel").gameObject;
        targetHullUI = targetInformationPanel.transform.Find ("Health Indicators/Hull").GetComponent<Image> ();
        for (int i = 0; i < 6; i++) targetShieldUI[i] = targetHullUI.transform.Find ("Shield " + i).GetComponent<Image> ();
        targetName = targetInformationPanel.transform.Find ("Name").GetComponent<TextMeshProUGUI> ();
        targetFaction = targetInformationPanel.transform.Find ("Faction").GetComponent<TextMeshProUGUI> ();
        targetDistance = targetInformationPanel.transform.Find ("Distance").GetComponent<TextMeshProUGUI> ();
        toSource = targetHullUI.transform.Find ("Angle Arrow").GetComponent<RectTransform> ();
        equipmentButtonsParent = canvas.transform.Find ("Equipment Buttons Parent");
        capacitorTransform = canvas.transform.Find ("Capacitor Background/Capacitor").GetComponent<RectTransform> ();
        AIInfo = canvas.transform.Find ("AI Indicators").gameObject;
        dockButton = canvas.transform.Find ("Dock Button").gameObject;
        undockButton = canvas.transform.Find ("Undock Button").gameObject;
    }

    void Update () {
        if (source == null) return;
        // Hull
        hullUI.color = Mathf.Floor (source.hullTimeSinceLastDamaged / 0.3f) % 2 == 1 && source.hullTimeSinceLastDamaged < 1.5f ?
            Color.white :
            hullGradient.Evaluate (source.hull / source.profile.hull);
        // Shields
        if (source.shield.shield != null) {
            if (source.shield.online)
                for (int i = 0; i < 6; i++)
                    shieldUI[i].color = Mathf.Floor (source.shield.shieldTimesSinceLastDamaged[i] / 0.3f) % 2 == 1 && source.shield.shieldTimesSinceLastDamaged[i] < 1.5f ?
                        Color.white :
                        shieldGradient.Evaluate (source.shield.strengths[i] / source.shield.shield.strength);
            else
                for (int i = 0; i < 6; i++) shieldUI[i].color = Color.grey;
        }
        // Angle arrow
        if (source.targetted == null) sourceTo.gameObject.SetActive (false);
        else {
            sourceTo.gameObject.SetActive (true);
            float rot = source.GetSector (source.targetted.transform.position) * 60.0f;
            sourceTo.anchoredPosition = new Vector2 (Mathf.Sin (rot * Mathf.Deg2Rad) * 75.0f, Mathf.Cos (rot * Mathf.Deg2Rad) * 75.0f);
            sourceTo.eulerAngles = new Vector3 (0.0f, 0.0f, -rot);
        }
        // Speed counter
        speedCounter.text = Mathf.Round (source.GetComponent<Rigidbody> ().velocity.magnitude).ToString ();
        // Capacitor
        capacitorTransform.sizeDelta = new Vector2 (source.capacitor.capacitor == null ? 0.0f : source.capacitor.storedEnergy / source.capacitor.capacitor.capacitance * 150.0f, 20.0f);
        // AI indicators
        if (source.AIActivated) AIInfo.SetActive (true);
        else AIInfo.SetActive (false);
        // Target information
        StructureBehaviours targetStructureBehaviour = source.targetted;
        if (targetStructureBehaviour == null) targetInformationPanel.SetActive (false);
        else {
            targetInformationPanel.SetActive (true);
            targetHullUI.color = Mathf.Floor (targetStructureBehaviour.hullTimeSinceLastDamaged / 0.3f) % 2 == 1 && targetStructureBehaviour.hullTimeSinceLastDamaged < 1.5f ?
                Color.white :
                hullGradient.Evaluate (targetStructureBehaviour.hull / targetStructureBehaviour.profile.hull);
            if (targetStructureBehaviour.shield.shield != null) {
                if (targetStructureBehaviour.shield.online)
                    for (int i = 0; i < 6; i++)
                        targetShieldUI[i].color = Mathf.Floor (targetStructureBehaviour.shield.shieldTimesSinceLastDamaged[i] / 0.3f) % 2 == 1 && targetStructureBehaviour.shield.shieldTimesSinceLastDamaged[i] < 1.5f ?
                            Color.white :
                            shieldGradient.Evaluate (targetStructureBehaviour.shield.strengths[i] / targetStructureBehaviour.shield.shield.strength);
                else for (int i = 0; i < 6; i++) targetShieldUI[i].color = Color.grey;
            }
            targetName.text = targetStructureBehaviour.gameObject.name;
            targetFaction.text = targetStructureBehaviour.faction;
            targetDistance.text = System.Math.Round (Vector3.Distance (source.transform.position, targetStructureBehaviour.transform.position), 2) + "m";
            float rot = targetStructureBehaviour.GetSector (source.transform.position) * 60.0f;
            toSource.anchoredPosition = new Vector2 (Mathf.Sin (rot * Mathf.Deg2Rad) * 55.0f, Mathf.Cos (rot * Mathf.Deg2Rad) * 55.0f);
            toSource.eulerAngles = new Vector3 (0.0f, 0.0f, -rot);
        }
        // Equipment
        if (equipmentButtons.Count != source.turrets.Count + 2) {
            equipmentButtons = new List<GameObject> ();
            for (int i = 0; i < source.turrets.Count + 2; i++)
                equipmentButtons.Add (null);
        }
        int turretButtonsShift = 0;
        // TODO do the same for the electronics and tractor beam
        // Turrets
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
            button.GetComponent<Button> ().interactable = source.turrets[i].CanActivate (source.targetted == null ? null : source.targetted.gameObject);
            SelectableButtonFunction (() => source.turrets[button.transform.GetSiblingIndex ()].Activate (source.targetted == null ? null : source.targetted.gameObject), button.GetComponent<Button> ());
            equipmentButtons[i] = button;
        }
        StructureBehaviours stationStructureBehaviours = source.transform.parent.GetComponent<StructureBehaviours> ();
        // Docking
        if (source.targetted == null || source.targetted.profile.dockingPoints == 0 ||
            (source.transform.position - source.targetted.transform.position).sqrMagnitude > source.targetted.profile.dockingRange * source.targetted.profile.dockingRange ||
            stationStructureBehaviours != null) {
            dockButton.SetActive (false);
        } else {
            dockButton.SetActive (true);
        }
        // Undocking
        if (stationStructureBehaviours == null) {
            undockButton.SetActive (false);
        } else {
            undockButton.SetActive (true);
        }
    }

    void SelectableButtonFunction(UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
