using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject {
    public string id;
    [Header ("Icon")]
    public Sprite icon;
    [Header ("Item Stats")]
    [Tooltip ("How much others are willing to buy from you.")]
    public float buyPrice;
    [Tooltip ("How much others are willing to sell to you.")]
    public float sellPrice;
    public float size;
}