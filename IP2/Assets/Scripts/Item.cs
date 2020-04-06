using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default,
    Blueprint,
    Module,
    Vessel
}

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
	public float basePrice;
	public float size;
    public Sprite icon;
}
