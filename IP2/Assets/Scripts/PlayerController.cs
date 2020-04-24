using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject selectableListItemUI;
    public GameObject weaponModulesListItemUI;
    public GameObject selectablesListItemUI;
    public GameObject inventoryItemUI;
    public GameObject selected;

    GameObject selectedObject;
    GameObject inventoryPanel;
    StructureStatsManager structureStatsManager;
    StructureMovementManager structureMovementManager;
    StructureEquipmentManager structureEquipmentManager;
    StructuresManager structuresManager;
    PositionsManager positionsManager;
    List<StructureStatsManager> structures;
    bool inventoryActive;

    void Awake() {
        inventoryPanel = GameObject.Find("/Canvas/Inventory Panel");
        inventoryPanel.SetActive(false);
        inventoryActive = false;
        structureStatsManager = GetComponent<StructureStatsManager>();
        structureMovementManager = GetComponent<StructureMovementManager>();
        structureEquipmentManager = GetComponent<StructureEquipmentManager>();
        structuresManager = FindObjectOfType<StructuresManager>();
        positionsManager = FindObjectOfType<PositionsManager>();
    }

    void Start() {
        InitializeEquipmentUI();
        UpdateSelectablesUI();
        StartCoroutine(UpdateUI());
    }

    void Update() {
        Inputs();
        if(transform.position.sqrMagnitude > 10000) positionsManager.ShiftOrigin(transform.position);
    }

    void Inputs() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(inventoryActive) {
                inventoryPanel.SetActive(false);
                inventoryActive = false;
            }
        }
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I)) {
            if(inventoryActive) {
                inventoryPanel.SetActive(false);
                inventoryActive = false;
            } else {
                inventoryPanel.SetActive(true);
                inventoryActive = true;
            }
        }
        if ((Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.LeftControl)) || Input.GetKeyDown(KeyCode.S)) {
            structureMovementManager.ClearOrders();
            if (Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.LeftControl)) structureMovementManager.ChangeAxisTranslation(Axis.Z, 0.25f);
            if (Input.GetKeyDown(KeyCode.S)) structureMovementManager.ChangeAxisTranslation(Axis.Z, -0.25f);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            structureMovementManager.ClearOrders();
            if (Input.GetKey(KeyCode.A)) structureMovementManager.SetPlaneRotation(Plane.XZ, -1.0f);
            if (Input.GetKey(KeyCode.D)) structureMovementManager.SetPlaneRotation(Plane.XZ, 1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.XZ, 0.0f);
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) {
            structureMovementManager.ClearOrders();
            if (Input.GetKey(KeyCode.Q)) structureMovementManager.SetPlaneRotation(Plane.YZ, -1.0f);
            if (Input.GetKey(KeyCode.E)) structureMovementManager.SetPlaneRotation(Plane.YZ, 1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.YZ, 0.0f);
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.C)) {
            structureMovementManager.ClearOrders();
            if (Input.GetKey(KeyCode.Z)) structureMovementManager.SetPlaneRotation(Plane.XY, 1.0f);
            if (Input.GetKey(KeyCode.C)) structureMovementManager.SetPlaneRotation(Plane.XY, -1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.XY, 0.0f);
        if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftControl)) {
            structureMovementManager.SetTarget(selected);
            structureMovementManager.ClearOrders();
            structureMovementManager.AddOrder("Align");
            structureMovementManager.AddOrder("Warp");
        }
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000000.0f) && hit.transform.gameObject != gameObject && hit.transform.gameObject.GetComponent<StructureStatsManager>())
                selected = hit.transform.gameObject;
        }
    }

    IEnumerator UpdateUI() {
        UpdateEquipmentUI();
        UpdateSelectablesUI();
        UpdateSlidersUI();
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(UpdateUI());
    }

    public void RefreshInventory() {
        int x = 0;
        int y = 0;
        GameObject content = inventoryPanel.transform.GetChild(0).GetChild(0).gameObject;
        foreach (Transform child in content.transform) Destroy(child.gameObject);
        foreach (Item item in structureStatsManager.cargoHold.Keys) {
            if(structureStatsManager.cargoHold[item] > 0) {
                GameObject listItem = Instantiate(inventoryItemUI) as GameObject;
                listItem.GetComponent<RectTransform>().SetParent(content.transform, false);
                listItem.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
                listItem.transform.GetChild(1).GetComponent<Text>().text = item.name;
                listItem.transform.GetChild(2).GetComponent<Text>().text = structureStatsManager.cargoHold[item].ToString();
                listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                x += 100;
                if(x > 400) {
                    x = 0;
                    y += 100;
                }
            }
        }
    }

    void UpdateSlidersUI() {
        GameObject.Find("/Canvas/Hull Slider").GetComponent<Slider>().maxValue = structureStatsManager.GetStat("Hull Max");
        GameObject.Find("/Canvas/Hull Slider").GetComponent<Slider>().value = structureStatsManager.GetStat("Hull");
        GameObject.Find("/Canvas/Armor Slider").GetComponent<Slider>().maxValue = structureStatsManager.GetStat("Armor Max");
        GameObject.Find("/Canvas/Armor Slider").GetComponent<Slider>().value = structureStatsManager.GetStat("Armor");
        GameObject.Find("/Canvas/Shield Slider").GetComponent<Slider>().maxValue = structureStatsManager.GetStat("Shield Max");
        GameObject.Find("/Canvas/Shield Slider").GetComponent<Slider>().value = structureStatsManager.GetStat("Shield");
    }

    void InitializeEquipmentUI() {
        GameObject panel = GameObject.Find("Equipment Panel");
        foreach(Transform child in panel.transform) Destroy(child.gameObject);
        int x = 0;
        for(int i = 0; i < structureEquipmentManager.equipment.Count; i++) {
            Equipment turret = structureEquipmentManager.equipment[i];
            GameObject element = Instantiate(weaponModulesListItemUI) as GameObject;
            element.transform.SetParent(panel.transform);
            element.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0.0f);
            SelectableButtonFunction(() => structureEquipmentManager.ToggleEquipment(element.transform.GetSiblingIndex(), selected, !structureEquipmentManager.equipmentGOs[element.transform.GetSiblingIndex()].GetComponent<EquipmentAttachmentPoint>().moduleActive), element.GetComponent<Button>());
            x += 30;
        }
    }

    void UpdateSelectablesUI() {
        GameObject sp = GameObject.Find("Selectables Panel");
        int destroyed = 0;
        List<GameObject> haveButtonTo = new List<GameObject>();
        foreach(Transform button in sp.transform) {
            GameObject pointsTo = button.GetComponent<GameObjectContainer>().value;
            if(pointsTo == null) {
                destroyed ++;
                Destroy(button.gameObject);
            } else {
                haveButtonTo.Add(pointsTo);
                if(destroyed > 0) {
                    RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
                    buttonRectTransform.anchoredPosition = new Vector2(0.0f, buttonRectTransform.anchoredPosition.y + destroyed * 15);
                }
            }
        }
        structures = structuresManager.GetStructures();
        if(structures == null) return;
        int y = 0;
        foreach(StructureStatsManager structure in structures) {
            GameObject structureGameObject = structure.gameObject;
            if(!haveButtonTo.Contains(structureGameObject)) {
                GameObject element = Instantiate(selectableListItemUI) as GameObject;
                Transform eTransform = element.transform;
                eTransform.SetParent(sp.transform);
                element.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, y);
                eTransform.GetChild(0).GetComponent<Text>().text = structureGameObject.name;
                eTransform.GetChild(1).GetComponent<Text>().text = structure.profile.name;
                eTransform.GetChild(2).GetComponent<Text>().text = structure.faction;
                if(structure == structureStatsManager) element.GetComponent<Button>().interactable = false;
                Container container = element.AddComponent<Container>();
                container.items.Add(new GameObjectContainerItem(structure.gameObject));
                SelectableButtonFunction(() => SetSelected(container.GetGameObjectContainerItem(0).value), element.GetComponent<Button>());
                y -= 15;
            }
        }
    }

    void SetSelected(GameObject go) {
        selected = go;
    }

    void UpdateEquipmentUI() {
        GameObject panel = GameObject.Find("Equipment Panel");
        for(int i = 0; i < structureEquipmentManager.equipmentGOs.Count; i++) {
            GameObject equipmentGO = structureEquipmentManager.equipmentGOs[i];
            EquipmentAttachmentPoint attachmentPoint = equipmentGO.GetComponent<EquipmentAttachmentPoint>();
            GameObject button = panel.transform.GetChild(i).gameObject;
            Image img = button.GetComponent<Image>();
            Image icon = button.transform.GetChild(0).GetComponent<Image>();
            if(attachmentPoint.moduleActive) {
                Color c = Color.green;
                c.a = 0.25f;
                img.color = c;
            } else {
                if(attachmentPoint.cycleElapsed == 0.0f) {
                    Color c = Color.white;
                    c.a = 0.25f;
                    img.color = c;
                } else {
                    Color c = Color.red;
                    c.a = 0.25f;
                    img.color = c;
                }
            }
            if(attachmentPoint.equipment.showLoadedChargeIcon) {
                if(attachmentPoint.loaded == null) icon.sprite = attachmentPoint.equipment.icon;
                else icon.sprite = attachmentPoint.loaded.icon;
            } else icon.sprite = attachmentPoint.equipment.icon;
        }
    }

    void SelectableButtonFunction(UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
