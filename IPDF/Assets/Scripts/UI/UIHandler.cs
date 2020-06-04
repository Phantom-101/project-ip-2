using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour {
    [Header ("Source")]
    public StructureBehaviours source;
    [Header ("Prefabs")]
    public GameObject equipmentButton;
    public GameObject itemSmallInfoPanel;
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
    public GameObject marketPanel;
    public GameObject itemsPanel;
    public GameObject itemsPanelContent;
    public Item selectedMarketItem;
    public GameObject itemInfoPanel;
    public GameObject itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemQuantity;
    public TextMeshProUGUI stationBuyPrice;
    public TextMeshProUGUI stationSellPrice;
    public GameObject buy1;
    public GameObject sell1;
    public GameObject buy10;
    public GameObject sell10;
    public GameObject buy100;
    public GameObject sell100;

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
        marketPanel = canvas.transform.Find ("Market Panel").gameObject;
        itemsPanel = marketPanel.transform.Find ("Items Panel").gameObject;
        itemsPanelContent = itemsPanel.transform.Find ("Viewport/Content").gameObject;
        itemInfoPanel = marketPanel.transform.Find ("Item Info Panel").gameObject;
        itemIcon = itemInfoPanel.transform.Find ("Item Icon").gameObject;
        itemName = itemIcon.transform.Find ("Item Name").GetComponent<TextMeshProUGUI> ();
        itemQuantity = itemIcon.transform.Find ("Item Quantity").GetComponent<TextMeshProUGUI> ();
        stationBuyPrice = itemInfoPanel.transform.Find ("Station Buy Price").GetComponent<TextMeshProUGUI> ();
        stationSellPrice = itemInfoPanel.transform.Find ("Station Sell Price").GetComponent<TextMeshProUGUI> ();
        buy1 = marketPanel.transform.Find ("Buy 1 Button").gameObject;
        sell1 = marketPanel.transform.Find ("Sell 1 Button").gameObject;
        buy10 = marketPanel.transform.Find ("Buy 10 Button").gameObject;
        sell10 = marketPanel.transform.Find ("Sell 10 Button").gameObject;
        buy100 = marketPanel.transform.Find ("Buy 100 Button").gameObject;
        sell100 = marketPanel.transform.Find ("Sell 100 Button").gameObject;
    }

    void Update () {
        if (source == null) return;
        // Hull
        hullUI.sprite = source.profile.hullUI;
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
        if (source.targeted == null) sourceTo.gameObject.SetActive (false);
        else {
            sourceTo.gameObject.SetActive (true);
            float rot = source.GetSector (source.targeted.transform.position) * 60.0f;
            sourceTo.anchoredPosition = new Vector2 (Mathf.Sin (rot * Mathf.Deg2Rad) * 75.0f, Mathf.Cos (rot * Mathf.Deg2Rad) * 75.0f);
            sourceTo.eulerAngles = new Vector3 (0.0f, 0.0f, -rot);
        }
        // Speed counter
        speedCounter.text = Mathf.Round (source.GetComponent<Rigidbody> ().velocity.magnitude).ToString ();
        // Capacitor
        capacitorTransform.sizeDelta = new Vector2 (source.capacitor.capacitor == null ? 0.0f : source.capacitor.storedEnergy / source.capacitor.capacitor.capacitance * 150.0f, 20.0f);
        // AI indicators
        if (source.AI == null) AIInfo.SetActive (false);
        else AIInfo.SetActive (true);
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
            button.GetComponent<Button> ().interactable = source.turrets[i].CanActivate (source.targeted == null ? null : source.targeted.gameObject);
            ButtonFunction (() => source.turrets[button.transform.GetSiblingIndex ()].Activate (source.targeted == null ? null : source.targeted.gameObject), button.GetComponent<Button> ());
            equipmentButtons[i] = button;
        }
        StructureBehaviours stationStructureBehaviours = source.transform.parent.GetComponent<StructureBehaviours> ();
        // Docking
        if (source.targeted == null || source.targeted.profile.dockingPoints == 0 ||
            (source.transform.position - source.targeted.transform.position).sqrMagnitude > source.targeted.profile.dockingRange * source.targeted.profile.dockingRange ||
            stationStructureBehaviours != null) {
            dockButton.SetActive (false);
        } else dockButton.SetActive (true);
        // Undocking
        if (stationStructureBehaviours == null) undockButton.SetActive (false);
        else undockButton.SetActive (true);
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
        if (stationStructureBehaviours == null || selectedMarketItem == null) itemInfoPanel.SetActive (false);
        else {
            itemInfoPanel.SetActive (true);
            itemIcon.GetComponent<Image> ().sprite = selectedMarketItem.icon;
            itemName.text = selectedMarketItem.name;
            itemQuantity.text = stationStructureBehaviours.inventory.GetItemCount (selectedMarketItem).ToString ();
            stationBuyPrice.text = stationStructureBehaviours.profile.market.GetSellPrice (stationStructureBehaviours, selectedMarketItem).ToString ();
            stationSellPrice.text = stationStructureBehaviours.profile.market.GetBuyPrice (stationStructureBehaviours, selectedMarketItem).ToString ();
        }
    }

    void ButtonFunction (UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
