﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIHandler : MonoBehaviour {
    public static GameUIHandler current;

    [Header ("Source Info")]
    public StructureBehaviours source;
    public StructureBehaviours stationStructureBehaviours;
    [Header ("Prefabs")]
    public GameObject equipmentButton;
    public GameObject itemSmallInfoPanel;
    public GameObject saveItem;
    public GameObject bayItem;
    public GameObject equipmentItem;
    [Header ("Gradients")]
    public Gradient hullGradient;
    public Gradient shieldGradient;
    public Gradient energyGradient;
    public Color playerFactionColor;
    public Gradient positiveFactionRelationsGradient;
    public Color alliedFactionColor;
    public Gradient negativeFactionRelationsGradient;
    public Color hostileFactionColor;
    [Header ("Settings")]
    public float flashTime = 0.3f;
    public float flashOffset = -0.1f;
    public int flashes = 5;
    [Header ("Managers")]
    public PlayerController playerController;
    public FactionsManager factionsManager;
    public StructuresManager structuresManager;
    public SavesHandler savesHandler;
    public new Camera camera;
    public SettingsHandler settingsHandler;
    public ScenesManager scenesManager;
    [Header ("UI Elements")]
    public Stack<GameUIState> activeUI = new Stack<GameUIState> ();
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public GameObject billboards;
    public Image hullUI;
    public Image[] shieldUI = new Image[6];
    public RectTransform sourceTo;
    public TextMeshProUGUI speedCounter;
    public RectTransform targetInformationPanel;
    public Image targetHullUI;
    public Image[] targetShieldUI = new Image[6];
    public Text targetName;
    public RectTransform targetNameRectTransform;
    public Text targetFaction;
    public RectTransform targetFactionRectTransform;
    public Text targetDistance;
    public RectTransform targetDistanceRectTransform;
    public RectTransform toSource;
    public Button lockCameraButton;
    public Transform equipmentButtonsParent;
    public List<GameObject> equipmentButtons = new List<GameObject> ();
    public GameObject capacitorBackground;
    public RectTransform capacitorTransform;
    public GameObject AIInfo;
    public Button dockButton;
    public Button undockButton;
    // Station
    public GameObject stationPanel;
    public Button stationMarket;
    public Button stationRepair;
    public Button stationEquipment;
    public GameObject marketPanel;
    public Button marketExit;
    public GameObject itemsPanel;
    public GameObject itemsPanelContent;
    public Item selectedMarketItem;
    public GameObject itemInfoPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemQuantity;
    public TextMeshProUGUI stationBuyPrice;
    public TextMeshProUGUI stationSellPrice;
    public Button buy1;
    public Button sell1;
    public Button buy10;
    public Button sell10;
    public Button buy100;
    public Button sell100;
    public GameObject equipmentPanel;
    public int selectedBay;
    public Equipment selectedEquipment;
    public List<GameObject> bayItems = new List<GameObject> ();
    public List<GameObject> equipmentItems = new List<GameObject> ();
    public Image equipmentIcon;
    public GameObject equipmentInformationPanel;
    public TextMeshProUGUI equipmentName;
    public TextMeshProUGUI replacePrice;
    public Button replaceButton;
    public Button unequipButton;
    public Button equipmentExitButton;
    public GameObject baysPanel;
    public GameObject baysPanelContent;
    public GameObject availableEquipmentPanel;
    public GameObject availableEquipmentPanelContent;
    public GameObject noEquipmentAvailable;
    public List<GameObject> selectableBillboards = new List<GameObject> ();
    public GameObject savesSelection;
    public GameObject savesPanelContent;
    public Button saveExitButton;
    public GameObject repairPanel;
    public Text hullRepairCostText;
    public Button hullRepairConfirmButton;
    public Button hullRepairCancelButton;
    public Text shieldRechargeCostText;
    public Button shieldRechargeConfirmButton;
    public Button shieldRechargeCancelButton;
    List<GameObject> saveItems = new List<GameObject> ();
    public GameObject death;
    public Button pauseButton;
    [Header ("Initialization")]
    public bool initialized;

    void Awake () {
        current = this;
    }

    public static GameUIHandler GetInstance () {
        return current;
    }

    public void Initialize () {
        playerController = PlayerController.GetInstance ();
        factionsManager = FactionsManager.GetInstance ();
        structuresManager = StructuresManager.GetInstance ();
        savesHandler = SavesHandler.GetInstance ();
        camera = Camera.main;
        settingsHandler = SettingsHandler.GetInstance ();
        scenesManager = ScenesManager.GetInstance ();
        if (settingsHandler != null) canvasScaler.scaleFactor = settingsHandler.settings.UIScale;
        ButtonFunction (() => { ChangeScreen (GameUIState.Station); playerController.Dock (); }, dockButton);
        ButtonFunction (() => { ChangeScreen (GameUIState.InSpace); playerController.Undock (); }, undockButton);
        ButtonFunction (() => RemoveOverlay (), marketExit);
        ButtonFunction (() => BuySelectedMarketItem (1), buy1);
        ButtonFunction (() => SellSelectedMarketItem (1), sell1);
        ButtonFunction (() => BuySelectedMarketItem (10), buy10);
        ButtonFunction (() => SellSelectedMarketItem (10), sell10);
        ButtonFunction (() => BuySelectedMarketItem (100), buy100);
        ButtonFunction (() => SellSelectedMarketItem (100), sell100);
        ButtonFunction (() => ReplaceSelectedEquipment (), replaceButton);
        ButtonFunction (() => UnequipSelectedBay (), unequipButton);
        ButtonFunction (() => { RemoveOverlay (); foreach (GameObject go in bayItems.ToArray ()) { bayItems.Remove (go); Destroy (go); } }, equipmentExitButton);
        ButtonFunction (() => { RemoveOverlay (); foreach (GameObject go in saveItems.ToArray ()) { saveItems.Remove (go); Destroy (go); } }, saveExitButton);
        ButtonFunction (() => { RemoveOverlay (); RepairShipHull (); }, hullRepairConfirmButton);
        ButtonFunction (() => { RemoveOverlay (); RechargeShipShield (); }, shieldRechargeConfirmButton);
        ButtonFunction (() => RemoveOverlay (), hullRepairCancelButton);
        ButtonFunction (() => Time.timeScale = (Time.timeScale == 1 ? 0 : 1), pauseButton);

        source = playerController.structureBehaviours;
        if (activeUI.Count == 0) activeUI.Push (GameUIState.InSpace);
        UpdateCanvas ();
        initialized = true;
    }

    public void FastestTickCanvas () {
        if (!initialized) return;
        if (activeUI.Peek () == GameUIState.InSpace) {
            // Resize target information
            StructureBehaviours targetStructureBehaviour = source.targeted;
            if (targetStructureBehaviour != null) {
                targetName.text = targetStructureBehaviour.gameObject.name;
                targetFaction.text = targetStructureBehaviour.faction.abbreviated;
                targetInformationPanel.sizeDelta = new Vector2 (
                    Mathf.Max (
                        350,
                        Mathf.Max (
                            targetNameRectTransform.sizeDelta.x,
                            Mathf.Max (
                                targetFactionRectTransform.sizeDelta.x,
                                targetDistanceRectTransform.sizeDelta.x
                            )
                        ) + 160
                    ),
                    135
                );
            }
            // Selectable UI
            List<StructureBehaviours> referenced = new List<StructureBehaviours> ();
            foreach (GameObject selectableBillboard in selectableBillboards.ToArray ()) {
                ContainerComponent billboardTarget = selectableBillboard.GetComponent<ContainerComponent> ();
                StructureBehaviours reference = billboardTarget.containers[0].value as StructureBehaviours;
                if (reference == null || source == null || reference.transform.parent != source.transform.parent || !reference.CanBeTargeted ()) {
                    selectableBillboards.Remove (selectableBillboard);
                    Destroy (selectableBillboard);
                } else {
                    if (reference == source) Destroy (selectableBillboard);
                    else {
                        Vector3 screenPosition = camera.WorldToScreenPoint (reference.transform.position);
                        if (screenPosition.z > 0) {
                            selectableBillboard.SetActive (true);
                            float canvasScaler = canvas.GetComponent<CanvasScaler> ().scaleFactor;
                            RectTransform selectableRectTransform = selectableBillboard.GetComponent<RectTransform> ();
                            selectableRectTransform.anchoredPosition = new Vector2 (screenPosition.x / canvasScaler, screenPosition.y / canvasScaler);
                            float scaler = Mathf.Sqrt (Vector3.Distance (reference.transform.position, source.transform.position)) * 5;
                            float size = Mathf.Clamp (250 - scaler, 75, 250) * 0.75f;
                            selectableRectTransform.sizeDelta = new Vector2 (size, size) * (reference == source.targeted ? 1.5f : 1);
                            Image billboardImage = selectableBillboard.GetComponent<Image> ();
                            if (reference.faction == source.faction) billboardImage.color = playerFactionColor;
                            else {
                                float relations = factionsManager.GetRelations (reference.faction, source.faction);
                                if (relations > 0) {
                                    if (factionsManager.Ally (reference.faction, source.faction)) billboardImage.color = alliedFactionColor;
                                    else billboardImage.color = positiveFactionRelationsGradient.Evaluate (relations / factionsManager.GetAllyThreshold (reference.faction));
                                } else {
                                    if (factionsManager.Hostile (reference.faction, source.faction)) billboardImage.color = hostileFactionColor;
                                    else billboardImage.color = negativeFactionRelationsGradient.Evaluate (relations / factionsManager.GetHostileThreshold (reference.faction));
                                }
                            }
                        } else selectableBillboard.SetActive (false);
                        referenced.Add (reference);
                    }
                }
            }
            List<StructureBehaviours> structures = structuresManager.structures;
            foreach (StructureBehaviours structure in structures) {
                if (!referenced.Contains (structure) && structure != source && structure.transform.parent == source.transform.parent) {
                    GameObject billboard = new GameObject ("Billboard UI Element (" + structure.gameObject.name + ")");
                    RectTransform billboardRectTransform = billboard.AddComponent<RectTransform> ();
                    billboardRectTransform.SetParent (billboards.transform);
                    billboardRectTransform.anchorMin = new Vector2 (0, 0);
                    billboardRectTransform.anchorMax = new Vector2 (0, 0);
                    billboard.transform.localPosition = new Vector3 (0, 0, 0.1f);
                    Image billboardImage = billboard.AddComponent<Image> ();
                    billboardImage.sprite = structure.profile.selectableBillboard;
                    ContainerComponent billboardContainerComponent = billboard.AddComponent<ContainerComponent> ();
                    billboardContainerComponent.containers.Add (new Container<Object> (structure as Object));
                    selectableBillboards.Add (billboard);
                    Button billboardButton = billboard.AddComponent<Button> ();
                    ButtonFunction (() => source.targeted = source.targeted == structure ? null : structure, billboardButton);
                }
            }
        }
    }

    public void ClampedTickCanvas () {
        if (!initialized) return;

        if (source == null && activeUI.Peek () != GameUIState.Death && activeUI.Peek () != GameUIState.SelectSave) ChangeScreen (GameUIState.Death);

        if (source == null) return;

        stationStructureBehaviours = source.transform.parent.GetComponent<StructureBehaviours> ();
        if (stationStructureBehaviours != null && activeUI.Peek () == GameUIState.InSpace) AddOverlay (GameUIState.Station);
        if (activeUI.Peek () == GameUIState.StationEquipment) {
            if (stationStructureBehaviours == null) {
                equipmentPanel.SetActive (false);
            } else {
                equipmentPanel.SetActive (true);
                if (bayItems.Count == 0) {
                    for (int i = 0; i < source.profile.turretSlots + 6; i++) {
                        GameObject bay = Instantiate (bayItem, baysPanelContent.transform);
                        RectTransform bayRect = bay.GetComponent<RectTransform> ();
                        bayRect.anchoredPosition = new Vector2 (0, -i * 45);
                        string bayName = source.GetSlot (i).GetSlotName ();
                        string equippedName = source.GetSlot (i).GetEquippedName ();
                        bay.transform.GetChild (1).GetComponent<Text> ().text = bayName;
                        bay.transform.GetChild (2).GetComponent<Text> ().text = equippedName;
                        ButtonFunction (() => { SetSelectedBay (bay.transform.GetSiblingIndex ()); selectedEquipment = null; }, bay.GetComponent<Button> ());
                        bayItems.Add (bay);
                    }
                    baysPanelContent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, (source.profile.turretSlots + 6) * 45 + 5);
                }
                System.Type equipmentType = source.GetSlot (selectedBay).GetEquipmentType ();
                if (equipmentItems.Count == 0) {
                    int offset = 0;
                    foreach (Equipment offered in stationStructureBehaviours.profile.offeredEquipment) {
                        if (offered.GetType ().IsSubclassOf (equipmentType)) {
                            GameObject equipmentElement = Instantiate (equipmentItem, availableEquipmentPanelContent.transform);
                            RectTransform rect = equipmentElement.GetComponent<RectTransform> ();
                            rect.anchoredPosition = new Vector2 (0, -offset * 45);
                            equipmentElement.transform.GetChild (1).GetComponent<Text> ().text = offered.name;
                            ButtonFunction (() => selectedEquipment = offered, equipmentElement.GetComponent<Button> ());
                            offset ++;
                            equipmentItems.Add (equipmentElement);
                        }
                    }
                    availableEquipmentPanelContent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, offset * 45 + 5);
                    if (offset == 0) noEquipmentAvailable.SetActive (true);
                    else noEquipmentAvailable.SetActive (false);
                }
                if (selectedEquipment != null) {
                    equipmentInformationPanel.SetActive (true);
                    equipmentIcon.sprite = selectedEquipment.icon;
                    equipmentName.text = selectedEquipment.name;
                    replacePrice.text = "Not Implemented";
                } else equipmentInformationPanel.SetActive (false);
            }
        } else equipmentPanel.SetActive (false);
        if (activeUI.Peek () == GameUIState.InSpace || activeUI.Peek () == GameUIState.Station || activeUI.Peek () == GameUIState.StationRepair) {
            // Hull
            hullUI.gameObject.SetActive (true);
            hullUI.sprite = source.profile.hullUI;
            hullUI.color = Mathf.Floor ((source.hullTimeSinceLastDamaged + flashOffset) / flashTime) % 2 == 1 && source.hullTimeSinceLastDamaged < flashOffset + flashTime * 2 * flashes - flashTime ?
                Color.white :
                hullGradient.Evaluate (source.hull / source.profile.hull);
            // Shields
            for (int i = 0; i < 6; i++) shieldUI[i].gameObject.SetActive (true);
            if (source.shield.shield != null) {
                if (source.shield.online)
                    for (int i = 0; i < 6; i++)
                        shieldUI[i].color = Mathf.Floor ((source.shield.shieldTimesSinceLastDamaged[i] + flashOffset) / flashTime) % 2 == 1 && source.shield.shieldTimesSinceLastDamaged[i] < flashOffset + flashTime * 2 * flashes - flashTime ?
                            Color.white :
                            shieldGradient.Evaluate (source.shield.strengths[i] / source.shield.shield.strength);
                else for (int i = 0; i < 6; i++) shieldUI[i].color = shieldGradient.Evaluate (0);
            } else for (int i = 0; i < 6; i++) shieldUI[i].color = shieldGradient.Evaluate (0);
            // Capacitor
            capacitorBackground.SetActive (true);
            capacitorTransform.sizeDelta = new Vector2 (source.capacitor.capacitor == null ? 0.0f : source.capacitor.storedEnergy / source.capacitor.capacitor.capacitance * 150.0f, 20.0f);
        } else {
            hullUI.gameObject.SetActive (false);
            for (int i = 0; i < 6; i++) shieldUI[i].gameObject.SetActive (false);
            capacitorBackground.SetActive (false);
        }
        // AI indicators
        if (source.AI == null) AIInfo.SetActive (false);
        else AIInfo.SetActive (true);
        // Docking
        if (source.targeted == null || !(source.targeted.DockerCanDock (source)) || stationStructureBehaviours != null) {
            dockButton.gameObject.SetActive (false);
        } else dockButton.gameObject.SetActive (true);
        // Undocking
        if (stationStructureBehaviours == null) undockButton.gameObject.SetActive (false);
        else undockButton.gameObject.SetActive (true);
        if (activeUI.Peek () == GameUIState.InSpace) {
            // Angle arrow
            if (source.targeted == null) sourceTo.gameObject.SetActive (false);
            else {
                sourceTo.gameObject.SetActive (true);
                float rot = source.GetSector (source.targeted.transform.position) * 60;
                sourceTo.anchoredPosition = new Vector2 (Mathf.Sin (rot * Mathf.Deg2Rad) * 75, Mathf.Cos (rot * Mathf.Deg2Rad) * 75);
                sourceTo.eulerAngles = new Vector3 (0, 0, -rot);
            }
            // Speed counter
            speedCounter.gameObject.SetActive (true);
            speedCounter.text = Mathf.Round (source.GetComponent<Rigidbody> ().velocity.magnitude).ToString ();
            // Target information
            StructureBehaviours targetStructureBehaviour = source.targeted;
            if (targetStructureBehaviour == null) targetInformationPanel.gameObject.SetActive (false);
            else {
                targetInformationPanel.gameObject.SetActive (true);
                targetHullUI.sprite = targetStructureBehaviour.profile.hullUI;
                targetHullUI.color = Mathf.Floor ((targetStructureBehaviour.hullTimeSinceLastDamaged + flashOffset) / flashTime) % 2 == 1 && targetStructureBehaviour.hullTimeSinceLastDamaged < flashOffset + flashTime * 2 * flashes - flashTime ?
                    Color.white :
                    hullGradient.Evaluate (targetStructureBehaviour.hull / targetStructureBehaviour.profile.hull);
                if (targetStructureBehaviour.shield.shield != null) {
                    if (targetStructureBehaviour.shield.online)
                        for (int i = 0; i < 6; i++)
                            targetShieldUI[i].color = Mathf.Floor ((targetStructureBehaviour.shield.shieldTimesSinceLastDamaged[i] + flashOffset) / flashTime) % 2 == 1 && targetStructureBehaviour.shield.shieldTimesSinceLastDamaged[i] < flashOffset + flashTime * 2 * flashes - flashTime ?
                                Color.white :
                                shieldGradient.Evaluate (targetStructureBehaviour.shield.strengths[i] / targetStructureBehaviour.shield.shield.strength);
                    else for (int i = 0; i < 6; i++) targetShieldUI[i].color = shieldGradient.Evaluate (0);
                } else for (int i = 0; i < 6; i++) targetShieldUI[i].color = shieldGradient.Evaluate (0);
                float relations = factionsManager.GetRelations (targetStructureBehaviour.faction, source.faction);
                if (relations > 0) {
                    if (factionsManager.Ally (targetStructureBehaviour.faction, source.faction)) targetFaction.color = alliedFactionColor;
                    else targetFaction.color = positiveFactionRelationsGradient.Evaluate (relations / factionsManager.GetAllyThreshold (targetStructureBehaviour.faction));
                } else {
                    if (factionsManager.Hostile (targetStructureBehaviour.faction, source.faction)) targetFaction.color = hostileFactionColor;
                    else targetFaction.color = negativeFactionRelationsGradient.Evaluate (relations / factionsManager.GetHostileThreshold (targetStructureBehaviour.faction));
                }
                targetDistance.text = Mathf.Round (Vector3.Distance (source.transform.position, targetStructureBehaviour.transform.position)) + "m";
                float rot = targetStructureBehaviour.GetSector (source.transform.position) * 60;
                toSource.anchoredPosition = new Vector2 (Mathf.Sin (rot * Mathf.Deg2Rad) * 55, Mathf.Cos (rot * Mathf.Deg2Rad) * 55);
                toSource.eulerAngles = new Vector3 (0, 0, -rot);
            }
            // Equipment
            if (equipmentButtons.Count != source.turrets.Count + 2) {
                equipmentButtons = new List<GameObject> ();
                for (int i = 0; i < source.turrets.Count + 2; i++)
                    equipmentButtons.Add (null);
            }
            int turretButtonsShift = 0;
            // Tractor beam
            TractorBeam referencedTractorBeam = null;
            GameObject tractorBeamButton = equipmentButtons[0];
            if (tractorBeamButton != null) {
                ContainerComponent containerComponent = tractorBeamButton.GetComponent<ContainerComponent> ();
                if (containerComponent == null) Destroy (tractorBeamButton);
                else referencedTractorBeam = containerComponent.containers[0].value as TractorBeam;
                if (referencedTractorBeam != source.tractorBeam.tractorBeam) Destroy (tractorBeamButton);
            }
            if (tractorBeamButton == null) {
                tractorBeamButton = Instantiate (equipmentButton) as GameObject;
                tractorBeamButton.transform.SetParent (equipmentButtonsParent, false);
                ContainerComponent equipmentContainer = tractorBeamButton.GetComponent<ContainerComponent> ();
                if (equipmentContainer == null) equipmentContainer = tractorBeamButton.AddComponent<ContainerComponent> ();
                equipmentContainer.containers.Add (new Container<Object> (source.tractorBeam.tractorBeam));
                referencedTractorBeam = equipmentContainer.containers[0].value as TractorBeam;
            }
            tractorBeamButton.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, turretButtonsShift * 50);
            tractorBeamButton.transform.GetChild (0).GetComponent<Image> ().sprite = referencedTractorBeam == null ? null : referencedTractorBeam.icon;
            tractorBeamButton.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedTractorBeam == null ? 0 : source.tractorBeam.storedEnergy / referencedTractorBeam.maxStoredEnergy * 40, 5);
            tractorBeamButton.transform.GetChild (1).GetChild (0).GetComponent<Image> ().color = energyGradient.Evaluate (referencedTractorBeam == null ? 0 : source.tractorBeam.storedEnergy / referencedTractorBeam.maxStoredEnergy);
            //tractorBeamButton.GetComponent<Button> ().interactable = source.tractorBeam.CanActivate (source.targeted == null ? null : source.targeted.gameObject);
            ButtonFunction (() => source.tractorBeam.Interacted (source.gameObject, source.targeted == null ? null : source.targeted.gameObject), tractorBeamButton.GetComponent<Button> ());
            equipmentButtons[0] = tractorBeamButton;
            if (referencedTractorBeam != null) {
                equipmentButtons[0].gameObject.SetActive (true);
                turretButtonsShift ++;
            }
            else equipmentButtons[0].gameObject.SetActive (false);
            equipmentButtons[0].transform.SetSiblingIndex(0);
            // Electronics
            Electronics referencedElectronics = null;
            GameObject electronicsButton = equipmentButtons[1];
            if (electronicsButton != null) {
                ContainerComponent containerComponent = electronicsButton.GetComponent<ContainerComponent> ();
                if (containerComponent == null) Destroy (electronicsButton);
                else referencedElectronics = containerComponent.containers[0].value as Electronics;
                if (referencedElectronics != source.electronics.electronics) Destroy (electronicsButton);
            }
            if (electronicsButton == null) {
                electronicsButton = Instantiate (equipmentButton) as GameObject;
                electronicsButton.transform.SetParent (equipmentButtonsParent, false);
                ContainerComponent equipmentContainer = electronicsButton.GetComponent<ContainerComponent> ();
                if (equipmentContainer == null) equipmentContainer = electronicsButton.AddComponent<ContainerComponent> ();
                equipmentContainer.containers.Add (new Container<Object> (source.electronics.electronics));
                referencedElectronics = equipmentContainer.containers[0].value as Electronics;
            }
            electronicsButton.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, turretButtonsShift * 50);
            electronicsButton.transform.GetChild (0).GetComponent<Image> ().sprite = referencedElectronics == null ? null : referencedElectronics.icon;
            electronicsButton.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedElectronics == null ? 0 : source.electronics.storedEnergy / referencedElectronics.maxStoredEnergy * 40, 5);
            electronicsButton.transform.GetChild (1).GetChild (0).GetComponent<Image> ().color = energyGradient.Evaluate (referencedElectronics == null ? 0 : source.electronics.storedEnergy / referencedElectronics.maxStoredEnergy);
            //tractorBeamButton.GetComponent<Button> ().interactable = source.tractorBeam.CanActivate (source.targeted == null ? null : source.targeted.gameObject);
            ButtonFunction (() => source.electronics.Activate (), electronicsButton.GetComponent<Button> ());
            equipmentButtons[1] = electronicsButton;
            if (referencedElectronics != null) {
                equipmentButtons[1].gameObject.SetActive (true);
                turretButtonsShift ++;
            }
            else equipmentButtons[1].gameObject.SetActive (false);
            equipmentButtons[1].transform.SetSiblingIndex(1);
            // Turrets
            for (int i = 0; i < source.turrets.Count; i++) {
                Turret referencedTurret = null;
                GameObject button = equipmentButtons[i + 2];
                if (button != null) {
                    ContainerComponent containerComponent = button.GetComponent<ContainerComponent> ();
                    if (containerComponent == null) Destroy (button);
                    else referencedTurret = containerComponent.containers[0].value as Turret;
                    if (referencedTurret != source.turrets[i].turret) Destroy (button);
                }
                if (button == null) {
                    button = Instantiate (equipmentButton) as GameObject;
                    button.transform.SetParent (equipmentButtonsParent, false);
                    ContainerComponent equipmentContainer = button.GetComponent<ContainerComponent> ();
                    if (equipmentContainer == null) equipmentContainer = button.AddComponent<ContainerComponent> ();
                    equipmentContainer.containers.Add (new Container<Object> (source.turrets[i].turret));
                    referencedTurret = equipmentContainer.containers[0].value as Turret;
                }
                button.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, (i + turretButtonsShift) * 50);
                button.transform.GetChild (0).GetComponent<Image> ().sprite = referencedTurret == null ? null : referencedTurret.icon;
                button.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedTurret == null ? 0.0f : source.turrets[i].storedEnergy / referencedTurret.maxStoredEnergy * 40, 5);
                button.transform.GetChild (1).GetChild (0).GetComponent<Image> ().color = energyGradient.Evaluate (referencedTurret == null ? 0.0f : source.turrets[i].storedEnergy / referencedTurret.maxStoredEnergy);
                button.transform.GetChild (2).GetComponent<Text> ().text = source.turrets[i].turret.variant;
                button.GetComponent<Button> ().interactable = source.turrets[i].CanPress ();
                ButtonFunction (() => source.turrets[button.transform.GetSiblingIndex () - 2].Interacted (source.targeted == null ? null : source.targeted.gameObject), button.GetComponent<Button> ());
                equipmentButtons[i + 2] = button;
                if (referencedTurret != null) equipmentButtons[i + 2].gameObject.SetActive (true);
                else equipmentButtons[i + 2].gameObject.SetActive (false);
                equipmentButtons[i + 2].transform.SetSiblingIndex(i + 2);
            }
        }
        if (activeUI.Peek () == GameUIState.StationMarket) {
            // Market UI
            if (stationStructureBehaviours == null) {
                marketPanel.SetActive (false);
                selectedMarketItem = null;
            } else {
                marketPanel.SetActive (true);
                float height = 0;
                List<Item> tradables = stationStructureBehaviours.profile.market.GetTradableItems (stationStructureBehaviours);
                List<Item> present = new List<Item> ();
                foreach (RectTransform itemTransform in itemsPanelContent.transform) {
                    ContainerComponent itemContainer = itemTransform.GetComponent<ContainerComponent> ();
                    if (itemContainer == null) Destroy (itemTransform);
                    else {
                        if (!tradables.Contains (itemContainer.containers[0].value as Item)) Destroy (itemTransform);
                        else {
                            itemTransform.anchoredPosition = new Vector2 (0, height);
                            present.Add (itemContainer.containers[0].value as Item);
                            height -= 50;
                        }
                    }
                }
                foreach (Item tradable in tradables) {
                    if (!present.Contains (tradable)) {
                        GameObject instantiated = Instantiate (itemSmallInfoPanel) as GameObject;
                        RectTransform instantiatedRectTransform = instantiated.GetComponent<RectTransform> ();
                        instantiatedRectTransform.SetParent (itemsPanelContent.transform);
                        instantiatedRectTransform.anchoredPosition = new Vector2 (0, height);
                        ContainerComponent instantiatedContainer = instantiated.AddComponent<ContainerComponent> ();
                        instantiatedContainer.containers.Add (new Container<Object> (tradable));
                        instantiatedRectTransform.GetChild (0).GetComponent<Image> ().sprite = tradable.icon;
                        instantiatedRectTransform.GetChild (0).GetChild (0).GetComponent<TextMeshProUGUI> ().text = tradable.name;
                        ButtonFunction (() => selectedMarketItem = instantiated.GetComponent<ContainerComponent> ().containers[0].value as Item, instantiated.GetComponent<Button> ());
                        height -= 50;
                    }
                }
                itemsPanelContent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, -height);
            }
            if (stationStructureBehaviours == null || selectedMarketItem == null) {
                itemInfoPanel.SetActive (false);
                buy1.gameObject.SetActive (false);
                sell1.gameObject.SetActive (false);
                buy10.gameObject.SetActive (false);
                sell10.gameObject.SetActive (false);
                buy100.gameObject.SetActive (false);
                sell100.gameObject.SetActive (false);
            }
            else {
                itemInfoPanel.SetActive (true);
                buy1.gameObject.SetActive (true);
                sell1.gameObject.SetActive (true);
                buy10.gameObject.SetActive (true);
                sell10.gameObject.SetActive (true);
                buy100.gameObject.SetActive (true);
                sell100.gameObject.SetActive (true);
                buy1.interactable = CanBuySelectedMarketItem (1);
                sell1.interactable = CanSellSelectedMarketItem (1);
                buy10.interactable = CanBuySelectedMarketItem (10);
                sell10.interactable = CanSellSelectedMarketItem (10);
                buy100.interactable = CanBuySelectedMarketItem (100);
                sell100.interactable = CanSellSelectedMarketItem (100);
                itemIcon.sprite = selectedMarketItem.icon;
                itemName.text = selectedMarketItem.name;
                itemQuantity.text = stationStructureBehaviours.inventory.GetItemCount (selectedMarketItem).ToString ();
                float sellPrice = stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem);
                float buyPrice = stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem);
                stationBuyPrice.text = sellPrice == -1 ? "Unavailable" : sellPrice.ToString ();
                stationSellPrice.text = buyPrice == -1 ? "Unavailable" : buyPrice.ToString ();
            }
        } else marketPanel.SetActive (false);
        if (activeUI.Peek () == GameUIState.StationRepair) {
            if (stationStructureBehaviours != null) {
                hullRepairCostText.text = GetHullRepairPrice ().ToString () + " Credits";
                shieldRechargeCostText.text = GetShieldRechargePrice ().ToString () + " Credits";
                hullRepairConfirmButton.enabled = factionsManager.GetWealth (source.faction) >= GetHullRepairPrice ();
                shieldRechargeConfirmButton.enabled = factionsManager.GetWealth (source.faction) >= GetShieldRechargePrice ();
            }
        }
    }

    void UpdateCanvas () {
        if (!initialized) return;
        if (activeUI.Peek () == GameUIState.InSpace) {
            // Control
            playerController.forwardPowerSlider.gameObject.SetActive (true);
            playerController.turnLeftButton.gameObject.SetActive (true);
            playerController.turnRightButton.gameObject.SetActive (true);
            playerController.dampenerSlider.gameObject.SetActive (true);
            lockCameraButton.gameObject.SetActive (true);
        } else {
            playerController.forwardPowerSlider.gameObject.SetActive (false);
            playerController.turnLeftButton.gameObject.SetActive (false);
            playerController.turnRightButton.gameObject.SetActive (false);
            playerController.dampenerSlider.gameObject.SetActive (false);
            lockCameraButton.gameObject.SetActive (false);
            sourceTo.gameObject.SetActive (false);
            speedCounter.gameObject.SetActive (false);
            targetInformationPanel.gameObject.SetActive (false);
            for (int i = 0; i < equipmentButtons.Count; i++) equipmentButtons[i].SetActive (false);
            foreach (GameObject selectableBillboard in selectableBillboards) selectableBillboard.SetActive (false);
        }
        if (activeUI.Peek () == GameUIState.Station) stationPanel.SetActive (true);
        else stationPanel.SetActive (false);
        if (activeUI.Peek () == GameUIState.StationRepair) {
            if (stationStructureBehaviours == null) repairPanel.SetActive (false);
            else repairPanel.SetActive (true);
        } else repairPanel.SetActive (false);
        if (activeUI.Peek () == GameUIState.SelectSave) {
            savesSelection.SetActive (true);
            if (!Directory.Exists (GetSavePath ())) Directory.CreateDirectory (GetSavePath ());
            if (saveItems.Count == 0) {
                FileInfo[] saves = new DirectoryInfo (Application.persistentDataPath + "/saves/").GetFiles ("*.txt").OrderBy (f => f.LastWriteTime).Reverse ().ToArray ();
                for (int i = 0; i < saves.Length; i++) {
                    FileInfo save = saves[i];
                    GameObject instantiated = Instantiate (saveItem, savesPanelContent.transform) as GameObject;
                    RectTransform rectTransform = instantiated.GetComponent<RectTransform> ();
                    rectTransform.anchoredPosition = new Vector2 (0, -i * 100);
                    UniverseSaveData universe = JsonUtility.FromJson<UniverseSaveData> (File.ReadAllText (GetSavePath () + save.Name));
                    instantiated.transform.GetChild (0).GetComponent<Text> ().text = universe.saveName;
                    instantiated.transform.GetChild (1).GetComponent<Text> ().text = save.LastWriteTime.ToString ();
                    int playerFactionID = 0;
                    foreach (StructureSaveData structure in universe.structures)
                        if (structure.isPlayer)
                            playerFactionID = structure.factionID;
                    foreach (Faction faction in universe.factions)
                        if (faction.id == playerFactionID) {
                            instantiated.transform.GetChild (2).GetComponent<Text> ().text = faction.name;
                            instantiated.transform.GetChild (3).GetComponent<Text> ().text = faction.wealth.ToString () + " Credits";
                        }
                    ButtonFunction (() => { SaveSelected (save.Name); Base (GameUIState.InSpace); }, instantiated.GetComponent<Button> ());
                    saveItems.Add (instantiated);
                }
                savesPanelContent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, saves.Length * 100);
            }
        } else savesSelection.SetActive (false);
        if (activeUI.Peek () == GameUIState.Death) death.SetActive (true);
        else death.SetActive (false);
    }

    void ButtonFunction (UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    void RepairShipHull () {
        source.hull = source.profile.hull;
        factionsManager.ChangeWealth (source.faction, -GetHullRepairPrice ());
    }

    void RechargeShipShield () {
        for (int i = 0; i < 6; i++) source.shield.strengths[i] = source.shield.shield.strength;
        factionsManager.ChangeWealth (source.faction, -GetShieldRechargePrice ());
    }

    long GetHullRepairPrice () {
        float percentage = source.hull / source.profile.hull;
        return (long) ((1 - percentage) * source.profile.sellPrice);
    }

    long GetShieldRechargePrice () {
        float totalDeficit = 0;
        for (int i = 0; i < 6; i++) totalDeficit += source.shield.shield.strength - source.shield.strengths[i];
        return (long) (totalDeficit / source.shield.shield.shieldRechargeEfficiency);
    }

    public void SetSelectedBay (int i) {
        selectedBay = i;
        foreach (GameObject go in equipmentItems.ToArray ()) {
            equipmentItems.Remove (go);
            Destroy (go);
        }
    }

    void ReplaceSelectedEquipment () {
        if (stationStructureBehaviours == null) return;
        if (selectedEquipment == null) return;
        if (source.GetSlot (selectedBay).TrySetEquipment (selectedEquipment)) {
            // Pay
        }
        foreach (GameObject go in bayItems.ToArray ()) {
            bayItems.Remove (go);
            Destroy (go);
        }
    }

    void UnequipSelectedBay () {
        if (stationStructureBehaviours == null) return;
        source.GetSlot (selectedBay).TrySetEquipment (null);
        foreach (GameObject go in bayItems.ToArray ()) {
            bayItems.Remove (go);
            Destroy (go);
        }
    }

    void BuySelectedMarketItem (int amount) {
        if (!CanBuySelectedMarketItem (amount)) return;
        long price = (long) stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem) * amount;
        factionsManager.ChangeWealth (stationStructureBehaviours.faction, price);
        factionsManager.ChangeWealth (source.faction, -price);
        source.inventory.AddItem (selectedMarketItem, amount);
        stationStructureBehaviours.inventory.RemoveItem (selectedMarketItem, amount);
    }

    bool CanBuySelectedMarketItem (int amount) {
        if (selectedMarketItem == null) return false;
        if (stationStructureBehaviours == null) return false;
        if (stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem) == -1) return false;
        if (stationStructureBehaviours.inventory.GetItemCount (selectedMarketItem) < amount) return false;
        if (selectedMarketItem.size * amount > source.inventory.GetAvailableSize ()) return false;
        long price = (long) stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem) * amount;
        if (price > factionsManager.GetWealth (source.faction)) return false;
        return true;
    }

    void SellSelectedMarketItem (int amount) {
        if (!CanSellSelectedMarketItem (amount)) return;
        long price = (long) stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem) * amount;
        factionsManager.ChangeWealth (source.faction, price);
        factionsManager.ChangeWealth (stationStructureBehaviours.faction, -price);
        stationStructureBehaviours.inventory.AddItem (selectedMarketItem, amount);
        source.inventory.RemoveItem (selectedMarketItem, amount);
    }

    bool CanSellSelectedMarketItem (int amount) {
        if (selectedMarketItem == null) return false;
        if (stationStructureBehaviours == null) return false;
        if (stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem) == -1) return false;
        if (source.inventory.GetItemCount (selectedMarketItem) < amount) return false;
        if (selectedMarketItem.size * amount > stationStructureBehaviours.inventory.GetAvailableSize ()) return false;
        long price = (long) stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem) * amount;
        if (price > factionsManager.GetWealth (stationStructureBehaviours.faction)) return false;
        return true;
    }

    public void SaveSelected (string saveName) {
        initialized = false;
        savesHandler.Load (GetSavePath () + saveName);
    }

    public string GetSavePath () {
        return Application.persistentDataPath + "/saves/";
    }

    public string GetSavePath (string saveName) {
        return Application.persistentDataPath + "/saves/" + saveName + ".txt";
    }

    public void AddOverlay (GameUIState target) {
        activeUI.Push (target);
        UpdateCanvas ();
    }

    public void AddOverlay (int target) {
        activeUI.Push ((GameUIState) target);
        UpdateCanvas ();
    }

    public void RemoveOverlay () {
        activeUI.Pop ();
        UpdateCanvas ();
    }

    public void ChangeScreen (GameUIState target) {
        activeUI.Pop ();
        activeUI.Push (target);
        UpdateCanvas ();
    }

    public void ChangeScreen (int target) {
        activeUI.Pop ();
        activeUI.Push ((GameUIState) target);
        UpdateCanvas ();
    }

    public void Base (GameUIState target) {
        activeUI = new Stack<GameUIState> ();
        activeUI.Push (target);
        UpdateCanvas ();
    }

    public void Base (int target) {
        activeUI = new Stack<GameUIState> ();
        activeUI.Push ((GameUIState) target);
        UpdateCanvas ();
    }

    public void Exit () {
        scenesManager.SetLoadedScene ("Main Menu");
    }
}

public enum GameUIState {
    InSpace = 0,
    Station = 1,
    StationRepair = 2,
    StationEquipment = 3,
    StationMarket = 4,
    SelectSave = 5,
    Death = 6
}