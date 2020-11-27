using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour {
    StructuresManager structuresManager;
    static MarketManager instance;

    Dictionary<Item, List<StructureBehaviours>> itemToStructure = new Dictionary<Item, List<StructureBehaviours>> ();

    void Awake () {
        instance = this;
    }

    void Start () {
        structuresManager = StructuresManager.GetInstance ();
    }

    public static MarketManager GetInstance () {
        return instance;
    }

    void Update () {
        itemToStructure = new Dictionary<Item, List<StructureBehaviours>> ();
        foreach (StructureBehaviours structure in structuresManager.structures) {
            if (structure.profile.market != null) {
                List<Item> tradables = structure.profile.market.GetTradableItems (structure);
                foreach (Item tradable in tradables) {
                    if (!itemToStructure.ContainsKey (tradable)) itemToStructure[tradable] = null;
                    if (itemToStructure[tradable] == null) itemToStructure[tradable] = new List<StructureBehaviours> ();
                    itemToStructure[tradable].Add (structure);
                }
            }
        }
    }

    public List<StructureBehaviours> GetStructuresTradeItem (Item item) {
        if (item == null) return new List<StructureBehaviours> ();
        if (!itemToStructure.ContainsKey (item)) itemToStructure[item] = null;
        return (itemToStructure[item] == null ? new List<StructureBehaviours> () : itemToStructure[item]);
    }
}
