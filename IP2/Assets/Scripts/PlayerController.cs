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
    StructureModulesManager structureModulesManager;
    StructuresManager structuresManager;
    bool inventoryActive;

    void Awake() {
        inventoryPanel = GameObject.Find("/Canvas/Inventory Panel");
        inventoryPanel.SetActive(false);
        inventoryActive = false;
        structureStatsManager = GetComponent<StructureStatsManager>();
        structureMovementManager = GetComponent<StructureMovementManager>();
        structureModulesManager = GetComponent<StructureModulesManager>();
        structuresManager = GameObject.FindObjectOfType<StructuresManager>();
    }

    void Start() {
        InitializeTurretsUI();
        UpdateSelectablesUI();
        StartCoroutine(UpdateUI());
    }

    void Update() {
        Inputs();
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
            structureMovementManager.OverrideOrder();
            if (Input.GetKeyDown(KeyCode.W)) structureMovementManager.ChangeAxisTranslation(Axis.Z, 0.25f);
            if (Input.GetKeyDown(KeyCode.S)) structureMovementManager.ChangeAxisTranslation(Axis.Z, -0.25f);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            structureMovementManager.OverrideOrder();
            if (Input.GetKey(KeyCode.A)) structureMovementManager.SetPlaneRotation(Plane.XZ, -1.0f);
            if (Input.GetKey(KeyCode.D)) structureMovementManager.SetPlaneRotation(Plane.XZ, 1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.XZ, 0.0f);
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) {
            structureMovementManager.OverrideOrder();
            if (Input.GetKey(KeyCode.Q)) structureMovementManager.SetPlaneRotation(Plane.YZ, -1.0f);
            if (Input.GetKey(KeyCode.E)) structureMovementManager.SetPlaneRotation(Plane.YZ, 1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.YZ, 0.0f);
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.C)) {
            structureMovementManager.OverrideOrder();
            if (Input.GetKey(KeyCode.Z)) structureMovementManager.SetPlaneRotation(Plane.XY, 1.0f);
            if (Input.GetKey(KeyCode.C)) structureMovementManager.SetPlaneRotation(Plane.XY, -1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.XY, 0.0f);
        if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftControl)) structureMovementManager.WarpTo(selected);
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000000.0f) && hit.transform.gameObject != gameObject && hit.transform.gameObject.GetComponent<StructureStatsManager>())
                selected = hit.transform.gameObject;
        }
    }

    IEnumerator UpdateUI() {
        UpdateTurretsUI();
        UpdateSelectablesUI();
        UpdateSlidersUI();
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(UpdateUI());
    }

    public void RefreshInventory() {
        int x = 0;
        int y = 0;
        GameObject content = inventoryPanel.transform.GetChild(0).GetChild(0).gameObject;
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Item item in structureStatsManager.cargoHold.Keys)
        {
            if(structureStatsManager.cargoHold[item] > 0)
            {
                GameObject listItem = Instantiate(inventoryItemUI) as GameObject;
                listItem.GetComponent<RectTransform>().SetParent(content.transform, false);
                listItem.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
                listItem.transform.GetChild(1).GetComponent<Text>().text = item.name;
                listItem.transform.GetChild(2).GetComponent<Text>().text = structureStatsManager.cargoHold[item].ToString();
                listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                x += 100;
                if(x > 400)
                {
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

    void InitializeTurretsUI() {
        GameObject tp = GameObject.Find("Turrets Panel");
        foreach(Transform child in tp.transform) Destroy(child.gameObject);
        int x = 0;
        for(int i = 0; i < structureModulesManager.turrets.Count; i++) {
            Turret turret = structureModulesManager.turrets[i];
            GameObject element = Instantiate(weaponModulesListItemUI) as GameObject;
            element.transform.SetParent(tp.transform);
            element.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0.0f);
            SelectableButtonFunction(() => structureModulesManager.ToggleWeapon(element.transform.GetSiblingIndex(), selected, !structureModulesManager.turretGOs[element.transform.GetSiblingIndex()].GetComponent<TurretAttachmentPoint>().moduleActive), element.GetComponent<Button>());
            x += 30;
        }
    }

    void UpdateSelectablesUI() {
        GameObject sp = GameObject.Find("Selectables Panel");
        List<StructureStatsManager> structures = structuresManager.GetStructures();
        if(structures == null) return;
        foreach(Transform child in sp.transform) Destroy(child.gameObject);
        int y = 0;
        for(int i = 0; i < structures.Count; i++) {
            GameObject element = Instantiate(selectablesListItemUI) as GameObject;
            element.transform.SetParent(sp.transform);
            element.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, y);
            element.transform.GetChild(0).GetComponent<Text>().text = structures[i].gameObject.name;
            element.transform.GetChild(1).GetComponent<Text>().text = structures[i].profile.name;
            element.transform.GetChild(2).GetComponent<Text>().text = structures[i].faction;
            SelectableButtonFunction(() => SetSelected(structures[element.transform.GetSiblingIndex()].gameObject), element.GetComponent<Button>());
            y -= 15;
        }
    }

    void SetSelected(GameObject go) {
        selected = go;
    }

    void UpdateTurretsUI() {
        GameObject tp = GameObject.Find("Turrets Panel");
        for(int i = 0; i < structureModulesManager.turretGOs.Count; i++) {
            GameObject turretGO = structureModulesManager.turretGOs[i];
            TurretAttachmentPoint tap = turretGO.GetComponent<TurretAttachmentPoint>();
            GameObject button = tp.transform.GetChild(i).gameObject;
            Image img = button.GetComponent<Image>();
            Image icon = button.transform.GetChild(0).GetComponent<Image>();
            if(tap.moduleActive) img.color = Color.green;
            else {
                if(tap.cycleElapsed == 0.0f) img.color = Color.white;
                else img.color = Color.red;
            }
            if(tap.turret.showLoadedAmmoIcon) {
                if(tap.loaded == null) icon.sprite = tap.turret.icon;
                else icon.sprite = tap.loaded.icon;
            } else icon.sprite = tap.turret.icon;
        }
    }

    void SelectableButtonFunction(UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
