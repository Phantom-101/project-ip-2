using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject {
    [Header ("Icon")]
    public Sprite icon;
    [Header ("Item Stats")]
    public float basePrice;
    public float size;
}
