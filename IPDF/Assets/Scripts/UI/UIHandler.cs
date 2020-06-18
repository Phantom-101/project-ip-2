﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour {
    [Header ("Source Info")]
    public StructureBehaviours source;
    public StructureBehaviours stationStructureBehaviours;
    [Header ("Prefabs")]
    public GameObject equipmentButton;
    public GameObject itemSmallInfoPanel;
    [Header ("Gradients")]
    public Gradient hullGradient;
    public Gradient shieldGradient;
    public Gradient energyGradient;
    [Header ("Managers")]
    public PlayerController playerController;
    public FactionsManager factionsManager;
    public StructuresManager structuresManager;
    public new Camera camera;
    [Header ("UI Elements")]
    public string activeUI = "Self";
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
    public List<GameObject> selectableBillboards = new List<GameObject> ();

    void Awake () {
        playerController = FindObjectOfType<PlayerController> ();
        factionsManager = FindObjectOfType<FactionsManager> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        camera = FindObjectOfType<Camera> ();
        if (activeUI == "") activeUI = "Self";
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
        lockCameraButton = canvas.transform.Find ("Lock Camera").GetComponent<Button> ();
        equipmentButtonsParent = canvas.transform.Find ("Equipment Buttons Parent");
        capacitorBackground = canvas.transform.Find ("Capacitor Background").gameObject;
        capacitorTransform = capacitorBackground.transform.Find ("Capacitor").GetComponent<RectTransform> ();
        AIInfo = canvas.transform.Find ("AI Indicators").gameObject;
        dockButton = canvas.transform.Find ("Dock Button").GetComponent<Button> ();
        ButtonFunction (() => { activeUI = "Station"; playerController.Dock (); }, dockButton);
        undockButton = canvas.transform.Find ("Undock Button").GetComponent<Button> ();
        ButtonFunction (() => { activeUI = "Self"; playerController.Undock (); }, undockButton);
        stationPanel = canvas.transform.Find ("Station Panel").gameObject;
        stationMarket = stationPanel.transform.Find ("Market").GetComponent<Button> ();
        ButtonFunction (() => activeUI = "Station Market", stationMarket);
        stationRepair = stationPanel.transform.Find ("Repair").GetComponent<Button> ();
        stationEquipment = stationPanel.transform.Find ("Equipment").GetComponent<Button> ();
        marketPanel = canvas.transform.Find ("Market Panel").gameObject;
        marketExit = marketPanel.transform.Find ("Exit Button").GetComponent<Button> ();
        ButtonFunction (() => activeUI = "Station", marketExit);
        itemsPanel = marketPanel.transform.Find ("Items Panel").gameObject;
        itemsPanelContent = itemsPanel.transform.Find ("Viewport/Content").gameObject;
        itemInfoPanel = marketPanel.transform.Find ("Item Info Panel").gameObject;
        itemIcon = itemInfoPanel.transform.Find ("Item Icon").GetComponent<Image> ();
        itemName = itemIcon.transform.Find ("Item Name").GetComponent<TextMeshProUGUI> ();
        itemQuantity = itemIcon.transform.Find ("Item Quantity").GetComponent<TextMeshProUGUI> ();
        stationBuyPrice = itemInfoPanel.transform.Find ("Station Buy Price").GetComponent<TextMeshProUGUI> ();
        stationSellPrice = itemInfoPanel.transform.Find ("Station Sell Price").GetComponent<TextMeshProUGUI> ();
        buy1 = marketPanel.transform.Find ("Buy 1 Button").GetComponent<Button> ();
        ButtonFunction (() => BuySelectedMarketItem (1), buy1);
        sell1 = marketPanel.transform.Find ("Sell 1 Button").GetComponent<Button> ();
        ButtonFunction (() => SellSelectedMarketItem (1), sell1);
        buy10 = marketPanel.transform.Find ("Buy 10 Button").GetComponent<Button> ();
        ButtonFunction (() => BuySelectedMarketItem (10), buy10);
        sell10 = marketPanel.transform.Find ("Sell 10 Button").GetComponent<Button> ();
        ButtonFunction (() => SellSelectedMarketItem (10), sell10);
        buy100 = marketPanel.transform.Find ("Buy 100 Button").GetComponent<Button> ();
        ButtonFunction (() => BuySelectedMarketItem (100), buy100);
        sell100 = marketPanel.transform.Find ("Sell 100 Button").GetComponent<Button> ();
        ButtonFunction (() => SellSelectedMarketItem (100), sell100);
    }

    void Update () {
        if (source == null) return;
        stationStructureBehaviours = source.transform.parent.GetComponent<StructureBehaviours> ();
        if (activeUI == "Self" || activeUI == "Station" || activeUI == "Station Repair") {
            // Hull
            hullUI.gameObject.SetActive (true);
            hullUI.sprite = source.profile.hullUI;
            hullUI.color = Mathf.Floor (source.hullTimeSinceLastDamaged / 0.3f) % 2 == 1 && source.hullTimeSinceLastDamaged < 1.5f ?
                Color.white :
                hullGradient.Evaluate (source.hull / source.profile.hull);
            // Shields
            for (int i = 0; i < 6; i++) shieldUI[i].gameObject.SetActive (true);
            if (source.shield.shield != null) {
                if (source.shield.online)
                    for (int i = 0; i < 6; i++)
                        shieldUI[i].color = Mathf.Floor (source.shield.shieldTimesSinceLastDamaged[i] / 0.3f) % 2 == 1 && source.shield.shieldTimesSinceLastDamaged[i] < 1.5f ?
                            Color.white :
                            shieldGradient.Evaluate (source.shield.strengths[i] / source.shield.shield.strength);
                else
                    for (int i = 0; i < 6; i++) shieldUI[i].color = shieldGradient.Evaluate (0);
            }
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
        if (source.targeted == null || source.targeted.profile.dockingLocations.Length == 0 ||
            (source.transform.position - source.targeted.transform.position).sqrMagnitude > source.targeted.profile.dockingRange * source.targeted.profile.dockingRange ||
            stationStructureBehaviours != null) {
            dockButton.gameObject.SetActive (false);
        } else dockButton.gameObject.SetActive (true);
        // Undocking
        if (stationStructureBehaviours == null) undockButton.gameObject.SetActive (false);
        else undockButton.gameObject.SetActive (true);
        if (activeUI == "Self") {
            // Control
            playerController.forwardPowerSlider.gameObject.SetActive (true);
            playerController.turnLeftButton.gameObject.SetActive (true);
            playerController.turnRightButton.gameObject.SetActive (true);
            playerController.dampenerSlider.gameObject.SetActive (true);
            lockCameraButton.gameObject.SetActive (true);
            // Angle arrow
            if (source.targeted == null) sourceTo.gameObject.SetActive (false);
            else {
                sourceTo.gameObject.SetActive (true);
                float rot = source.GetSector (source.targeted.transform.position) * 60.0f;
                sourceTo.anchoredPosition = new Vector2 (Mathf.Sin (rot * Mathf.Deg2Rad) * 75.0f, Mathf.Cos (rot * Mathf.Deg2Rad) * 75.0f);
                sourceTo.eulerAngles = new Vector3 (0.0f, 0.0f, -rot);
            }
            // Speed counter
            speedCounter.gameObject.SetActive (true);
            speedCounter.text = Mathf.Round (source.GetComponent<Rigidbody> ().velocity.magnitude).ToString ();
            // Target information
            StructureBehaviours targetStructureBehaviour = source.targeted;
            if (targetStructureBehaviour == null) targetInformationPanel.SetActive (false);
            else {
                targetInformationPanel.SetActive (true);
                targetHullUI.sprite = targetStructureBehaviour.profile.hullUI;
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
                targetFaction.text = factionsManager.GetFaction(targetStructureBehaviour.factionID).abbreviated;
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
            tractorBeamButton.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, turretButtonsShift * 40);
            tractorBeamButton.transform.GetChild (0).GetComponent<Image> ().sprite = referencedTractorBeam == null ? null : referencedTractorBeam.icon;
            tractorBeamButton.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedTractorBeam == null ? 0 : source.tractorBeam.storedEnergy / referencedTractorBeam.maxStoredEnergy * 30, 3);
            tractorBeamButton.transform.GetChild (1).GetChild (0).GetComponent<Image> ().color = energyGradient.Evaluate (referencedTractorBeam == null ? 0 : source.tractorBeam.storedEnergy / referencedTractorBeam.maxStoredEnergy);
            //tractorBeamButton.GetComponent<Button> ().interactable = source.tractorBeam.CanActivate (source.targeted == null ? null : source.targeted.gameObject);
            ButtonFunction (() => source.tractorBeam.Toggle (source.gameObject, source.targeted == null ? null : source.targeted.gameObject), tractorBeamButton.GetComponent<Button> ());
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
            electronicsButton.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, turretButtonsShift * 40);
            electronicsButton.transform.GetChild (0).GetComponent<Image> ().sprite = referencedElectronics == null ? null : referencedElectronics.icon;
            electronicsButton.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedElectronics == null ? 0 : source.electronics.storedEnergy / referencedElectronics.maxStoredEnergy * 30, 3);
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
                button.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, (i + turretButtonsShift) * 40.0f);
                button.transform.GetChild (0).GetComponent<Image> ().sprite = referencedTurret == null ? null : referencedTurret.icon;
                button.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (referencedTurret == null ? 0.0f : source.turrets[i].storedEnergy / referencedTurret.maxStoredEnergy * 30.0f, 3.0f);
                button.transform.GetChild (1).GetChild (0).GetComponent<Image> ().color = energyGradient.Evaluate (referencedTurret == null ? 0.0f : source.turrets[i].storedEnergy / referencedTurret.maxStoredEnergy);
                button.GetComponent<Button> ().interactable = source.turrets[i].CanActivate (source.targeted == null ? null : source.targeted.gameObject);
                ButtonFunction (() => source.turrets[button.transform.GetSiblingIndex () - 2].Activate (source.targeted == null ? null : source.targeted.gameObject), button.GetComponent<Button> ());
                equipmentButtons[i + 2] = button;
                if (referencedTurret != null) equipmentButtons[i + 2].gameObject.SetActive (true);
                else equipmentButtons[i + 2].gameObject.SetActive (false);
                equipmentButtons[i + 2].transform.SetSiblingIndex(i + 2);
            }
            // Selectable UI
            List<StructureBehaviours> referenced = new List<StructureBehaviours> ();
            foreach (GameObject selectableBillboard in selectableBillboards.ToArray ()) {
                ContainerComponent billboardTarget = selectableBillboard.GetComponent<ContainerComponent> ();
                StructureBehaviours reference = billboardTarget.containers[0].value as StructureBehaviours;
                if (reference == null || reference.transform.parent != source.transform.parent) {
                    selectableBillboards.Remove (selectableBillboard);
                    Destroy (selectableBillboard);
                } else {
                    if (reference == source) Destroy (selectableBillboard);
                    else {
                        Vector3 screenPosition = camera.WorldToScreenPoint (reference.transform.position);
                        if (screenPosition.z > 0) {
                            selectableBillboard.SetActive (true);
                            selectableBillboard.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (screenPosition.x, screenPosition.y);
                            float scaler = Mathf.Sqrt (Vector3.Distance (reference.transform.position, source.transform.position)) * 10;
                            float size = Mathf.Clamp (250 - scaler, 50, 250);
                            selectableBillboard.GetComponent<RectTransform> ().sizeDelta = new Vector2 (size, size);
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
                    billboardRectTransform.SetParent (canvas.transform);
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
        } else {
            playerController.forwardPowerSlider.gameObject.SetActive (false);
            playerController.turnLeftButton.gameObject.SetActive (false);
            playerController.turnRightButton.gameObject.SetActive (false);
            playerController.dampenerSlider.gameObject.SetActive (false);
            lockCameraButton.gameObject.SetActive (false);
            sourceTo.gameObject.SetActive (false);
            speedCounter.gameObject.SetActive (false);
            targetInformationPanel.SetActive (false);
            for (int i = 0; i < equipmentButtons.Count; i++) equipmentButtons[i].SetActive (false);
            foreach (GameObject selectableBillboard in selectableBillboards) selectableBillboard.SetActive (false);
        }
        if (activeUI == "Station") stationPanel.SetActive (true);
        else stationPanel.SetActive (false);
        if (activeUI == "Station Market") {
            // Market UI
            if (stationStructureBehaviours == null) {
                marketPanel.SetActive (false);
                selectedMarketItem = null;
            }
            else {
                marketPanel.SetActive (true);
                float height = 0;
                Item[] inventoryItems = stationStructureBehaviours.inventory.inventory.Keys.ToArray ();
                List<Item> present = new List<Item> ();
                foreach (RectTransform itemTransform in itemsPanelContent.transform) {
                    ContainerComponent itemContainer = itemTransform.GetComponent<ContainerComponent> ();
                    if (itemContainer == null) Destroy (itemTransform);
                    else {
                        if (!inventoryItems.Contains (itemContainer.containers[0].value as Item)) Destroy (itemTransform);
                        else {
                            itemTransform.anchoredPosition = new Vector2 (0, height);
                            present.Add (itemContainer.containers[0].value as Item);
                            height -= 50;
                        }
                    }
                }
                foreach (Item inventoryItem in inventoryItems) {
                    if (!present.Contains (inventoryItem)) {
                        GameObject instantiated = Instantiate (itemSmallInfoPanel) as GameObject;
                        RectTransform instantiatedRectTransform = instantiated.GetComponent<RectTransform> ();
                        instantiatedRectTransform.SetParent (itemsPanelContent.transform);
                        instantiatedRectTransform.anchoredPosition = new Vector2 (0, height);
                        ContainerComponent instantiatedContainer = instantiated.AddComponent<ContainerComponent> ();
                        instantiatedContainer.containers.Add (new Container<Object> (inventoryItem));
                        instantiatedRectTransform.GetChild (0).GetComponent<Image> ().sprite = inventoryItem.icon;
                        instantiatedRectTransform.GetChild (0).GetChild (0).GetComponent<TextMeshProUGUI> ().text = inventoryItem.name;
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
                stationBuyPrice.text = ((long) stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem)).ToString ();
                stationSellPrice.text = ((long) stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem)).ToString ();
            }
        } else marketPanel.SetActive (false);
    }

    void ButtonFunction (UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    void BuySelectedMarketItem (int amount) {
        if (!CanBuySelectedMarketItem (amount)) return;
        long price = (long) stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem) * amount;
        factionsManager.ChangeWealth (stationStructureBehaviours.factionID, price);
        factionsManager.ChangeWealth (source.factionID, -price);
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
        if (price > factionsManager.GetWealth (source.factionID)) return false;
        return true;
    }

    void SellSelectedMarketItem (int amount) {
        if (!CanSellSelectedMarketItem (amount)) return;
        long price = (long) stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem) * amount;
        factionsManager.ChangeWealth (source.factionID, price);
        factionsManager.ChangeWealth (stationStructureBehaviours.factionID, -price);
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
        if (price > factionsManager.GetWealth (stationStructureBehaviours.factionID)) return false;
        return true;
    }
}
