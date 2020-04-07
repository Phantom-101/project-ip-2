using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject selectableListItemUI;
    public GameObject weaponModulesListItemUI;
    public GameObject inventoryItemUI;
    public GameObject selected;

    GameObject selectedObject;
    GameObject inventoryPanel;
    StructureStatsManager structureStatsManager;
    StructureMovementManager structureMovementManager;
    StructureModulesManager structureModulesManager;
    bool inventoryActive;

    void Awake()
    {
        inventoryPanel = GameObject.Find("/Canvas/Inventory Panel");
        inventoryPanel.SetActive(false);
        inventoryActive = false;
        structureStatsManager = GetComponent<StructureStatsManager>();
        structureMovementManager = GetComponent<StructureMovementManager>();
        structureModulesManager = GetComponent<StructureModulesManager>();
    }

    void Start()
    {
        InitializeTurretsUI();
        StartCoroutine(UpdateUI());
    }

    void Update()
    {
        Inputs();
    }

    void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(inventoryActive)
            {
                inventoryPanel.SetActive(false);
                inventoryActive = false;
            }
        }
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
        {
            if(inventoryActive)
            {
                inventoryPanel.SetActive(false);
                inventoryActive = false;
            }
            else
            {
                inventoryPanel.SetActive(true);
                inventoryActive = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
            if (Input.GetKeyDown(KeyCode.W)) structureMovementManager.ChangeAxisTranslation(Axis.Z, 0.25f);
            if (Input.GetKeyDown(KeyCode.S)) structureMovementManager.ChangeAxisTranslation(Axis.Z, -0.25f);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            if (Input.GetKey(KeyCode.A)) structureMovementManager.SetPlaneRotation(Plane.XZ, -1.0f);
            if (Input.GetKey(KeyCode.D)) structureMovementManager.SetPlaneRotation(Plane.XZ, 1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.XZ, 0.0f);
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) {
            if (Input.GetKey(KeyCode.Q)) structureMovementManager.SetPlaneRotation(Plane.YZ, -1.0f);
            if (Input.GetKey(KeyCode.E)) structureMovementManager.SetPlaneRotation(Plane.YZ, 1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.YZ, 0.0f);
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.C)) {
            if (Input.GetKey(KeyCode.Z)) structureMovementManager.SetPlaneRotation(Plane.XY, 1.0f);
            if (Input.GetKey(KeyCode.C)) structureMovementManager.SetPlaneRotation(Plane.XY, -1.0f);
        } else structureMovementManager.SetPlaneRotation(Plane.XY, 0.0f);
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000000.0f) && hit.transform.gameObject != gameObject && hit.transform.gameObject.GetComponent<StructureStatsManager>())
                selected = hit.transform.gameObject;
        }
    }

    IEnumerator UpdateUI()
    {
        UpdateTurretsUI();
        UpdateSlidersUI();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(UpdateUI());
    }

    public void RefreshInventory()
    {
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

    void UpdateSlidersUI()
    {
        GameObject.Find("/Canvas/Hull Slider").GetComponent<Slider>().maxValue = structureStatsManager.GetStat("structure hull max");
        GameObject.Find("/Canvas/Hull Slider").GetComponent<Slider>().value = structureStatsManager.GetStat("structure hull");
        GameObject.Find("/Canvas/Armor Slider").GetComponent<Slider>().maxValue = structureStatsManager.GetStat("structure armor max");
        GameObject.Find("/Canvas/Armor Slider").GetComponent<Slider>().value = structureStatsManager.GetStat("structure armor");
        GameObject.Find("/Canvas/Shield Slider").GetComponent<Slider>().maxValue = structureStatsManager.GetStat("structure shield max");
        GameObject.Find("/Canvas/Shield Slider").GetComponent<Slider>().value = structureStatsManager.GetStat("structure shield");
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

    void SelectableButtonFunction(UnityEngine.Events.UnityAction action, Button button)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
