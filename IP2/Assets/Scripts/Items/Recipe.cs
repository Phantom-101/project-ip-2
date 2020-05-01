using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObjects/Recipe")]
public class Recipe : ScriptableObject {
    public Item[] requiredItems;
    public int[] requiredPieces;
    public Item[] productItems;
    public int[] productPieces;
    public float time;
}
