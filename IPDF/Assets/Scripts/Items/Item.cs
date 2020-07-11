using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            id = System.Guid.NewGuid ().ToString ();
            ForceSerialization ();
        }
    }

    void ForceSerialization () {
        #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty (this);
        #endif
    }
}
