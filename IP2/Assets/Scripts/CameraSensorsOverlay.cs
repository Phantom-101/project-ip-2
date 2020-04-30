using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraSensorsOverlay : MonoBehaviour
{
    public Texture neutralSignature;
    public Texture alliedSignature;
    public Texture hostileSignature;
    public Camera attachedCamera;
    public bool signatureOverlayEnabled = true;

    PlayerController playerController;
    CloseUpCameraController closeUpCameraController;
    FleetControlCameraController fleetControlCameraController;
    StructuresManager structuresManager;

    void Start() {
        attachedCamera = GetComponent<Camera>();
        closeUpCameraController = GetComponent<CloseUpCameraController>();
        fleetControlCameraController = GetComponent<FleetControlCameraController>();
        playerController = closeUpCameraController.target.GetComponent<PlayerController>();
        structuresManager = GameObject.FindObjectOfType<StructuresManager>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.O)) signatureOverlayEnabled = !signatureOverlayEnabled;
    }

    void OnGUI() {
        if(playerController != null) {
            foreach(StructureStatsManager structureStatsManager in structuresManager.GetStructures()) {
                if(structureStatsManager.gameObject != playerController.gameObject) {
                    Vector3 screenPos = attachedCamera.WorldToScreenPoint(structureStatsManager.transform.position);
                    if (screenPos.z > 0) {
                        if(signatureOverlayEnabled) {
                            float distance = Vector3.Distance(playerController.gameObject.transform.position, structureStatsManager.transform.position);
                            if(distance <= 6000.0f) {
                                float size = 30 - distance / 200.0f;
                                if(size < 1.0f) size = 1.0f;
                                GUI.DrawTexture(new Rect(screenPos.x - size / 2.0f, Screen.height - screenPos.y - size / 2.0f, size, size), structureStatsManager.profile.signatureTex);
                            }
                        }
                        Ray ray = attachedCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast (ray, out hit, 1000000.0f) && hit.transform.gameObject == structureStatsManager.gameObject) {
                            GUI.Label(new Rect(screenPos.x + 30, Screen.height - screenPos.y - 20, 250, 20), structureStatsManager.gameObject.name);
                            GUI.Label(new Rect(screenPos.x + 30, Screen.height - screenPos.y, 250, 20), structureStatsManager.hitpoints[0] + " / " + structureStatsManager.hitpoints[1] + " / " + structureStatsManager.hitpoints[2]);
                            //GUIDrawRect(new Rect(screenPos.x + 30, Screen.height - screenPos.y, 100 * (structureStatsManager.GetStat("Hull") / structureStatsManager.GetStat("Hull Max")), 10), Color.green);
                            //GUIDrawRect(new Rect(screenPos.x + 30, Screen.height - screenPos.y, 100 * (structureStatsManager.GetStat("Armor") / structureStatsManager.GetStat("Armor Max")), 10), Color.grey);
                            //GUIDrawRect(new Rect(screenPos.x + 30, Screen.height - screenPos.y, 100 * (structureStatsManager.GetStat("Shield") / structureStatsManager.GetStat("Shield Max")), 10), Color.blue);
                            GUI.Label(new Rect(screenPos.x + 30, Screen.height - screenPos.y + 20, 250, 20), Math.Round(Vector3.Distance(playerController.gameObject.transform.position, hit.transform.position), 2).ToString());
                        }
                        if(playerController != null && playerController.selected != null && playerController.selected == structureStatsManager.gameObject) {
                            Vector3 selectedScreenPos = attachedCamera.WorldToScreenPoint(playerController.selected.transform.position);
                            if(selectedScreenPos.z > 0) {
                                GUI.Label(new Rect(selectedScreenPos.x + 30, Screen.height - selectedScreenPos.y - 20, 250, 20), structureStatsManager.gameObject.name);
                                GUI.Label(new Rect(selectedScreenPos.x + 30, Screen.height - selectedScreenPos.y, 250, 20), structureStatsManager.hitpoints[0] + " / " + structureStatsManager.hitpoints[1] + " / " + structureStatsManager.hitpoints[2]);
                                //GUIDrawRect(new Rect(selectedScreenPos.x + 30, Screen.height - selectedScreenPos.y, 100 * (structureStatsManager.GetStat("Hull") / structureStatsManager.GetStat("Hull Max")), 10), Color.green);
                                //GUIDrawRect(new Rect(selectedScreenPos.x + 30, Screen.height - selectedScreenPos.y, 100 * (structureStatsManager.GetStat("Armor") / structureStatsManager.GetStat("Armor Max")), 10), Color.grey);
                                //GUIDrawRect(new Rect(selectedScreenPos.x + 30, Screen.height - selectedScreenPos.y, 100 * (structureStatsManager.GetStat("Shield") / structureStatsManager.GetStat("Shield Max")), 10), Color.blue);
                                GUI.Label(new Rect(selectedScreenPos.x + 30, Screen.height - selectedScreenPos.y + 20, 250, 20), Math.Round(Vector3.Distance(playerController.gameObject.transform.position, structureStatsManager.transform.position), 2).ToString());
                            }
                        }
                    } else {
                    }
                }
            }
        }
    }

    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;
 
    public static void GUIDrawRect( Rect position, Color color )
    {
        if( _staticRectTexture == null )
        {
            _staticRectTexture = new Texture2D( 1, 1 );
        }
 
        if( _staticRectStyle == null )
        {
            _staticRectStyle = new GUIStyle();
        }
 
        _staticRectTexture.SetPixel( 0, 0, color );
        _staticRectTexture.Apply();
 
        _staticRectStyle.normal.background = _staticRectTexture;
 
        GUI.Box( position, GUIContent.none, _staticRectStyle );
 
 
    }
}
