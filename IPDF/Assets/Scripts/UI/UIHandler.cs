﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    public StructureBehaviours source;
    public Gradient hullGradient;
    public Gradient shieldGradient;

    GameObject canvas;
    Image hullUI;
    Image[] shieldUI = new Image[6];
    GameObject targetInformationPanel;
    Image targetHullUI;
    Image[] targetShieldUI = new Image[6];

    void Awake () {
        canvas = GameObject.Find ("Canvas");
        hullUI = canvas.transform.Find ("Health Indicators/Hull").GetComponent<Image> ();
        for (int i = 0; i < 6; i++) shieldUI[i] = canvas.transform.Find ("Health Indicators/Hull/Shield " + i).GetComponent<Image> ();
        targetInformationPanel = canvas.transform.Find ("Target Information Panel").gameObject;
        targetHullUI = targetInformationPanel.transform.Find ("Health Indicators/Hull").GetComponent<Image> ();
        for (int i = 0; i < 6; i++) targetShieldUI[i] = targetInformationPanel.transform.Find ("Health Indicators/Hull/Shield " + i).GetComponent<Image> ();
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
                if (targetStructureBehaviour.shield.online)
                    for (int i = 0; i < 6; i++)
                        targetShieldUI[i].color = shieldGradient.Evaluate (targetStructureBehaviour.shield.strengths[i] / targetStructureBehaviour.shield.shield.strength);
                else
                    for (int i = 0; i < 6; i++) targetShieldUI[i].color = Color.grey;
            }
            
        }
    }
}
