using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject {
    public string id;
    [Header ("Icon")]
    public Sprite icon;
    [Header ("Item Stats")]
    public float basePrice;
    public float size;

    void OnEnable () {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    void Awake () {
        if (id == null || id == "") {
            Undo.RecordObject (this, "ID Initialization");
            id = System.Guid.NewGuid ().ToString ();
        }
    }
}
