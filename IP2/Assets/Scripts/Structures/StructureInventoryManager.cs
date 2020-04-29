using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInventoryManager : MonoBehaviour {
    [Header("Inventory")]
    public Dictionary<Item, int> cargoHold = new Dictionary<Item, int>();

    StructureInitializer structureInitializer;
    StructureStatsManager structureStatsManager;

    public void Initialize(StructureInitializer initializer) {
        structureInitializer = initializer;
        structureStatsManager = initializer.structureStatsManager;
    }
}
