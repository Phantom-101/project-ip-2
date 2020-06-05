using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New StructureMarket", menuName = "StructureMarket/StructureMarket")]
public class StructureMarket : ScriptableObject {
    public SturctureMarketType marketType;

    public float GetBuyPrice (StructureBehaviours structure, Item item) {
        if (marketType == SturctureMarketType.Station) {
            foreach (FactoryHandler factoryHandler in structure.factories)
                foreach (Item input in factoryHandler.factory.inputs)
                    if (input == item)
                        return item.basePrice * 1.05f;
            return -1;
        } else {
            return item.basePrice;
        }
    }

    public float GetSellPrice (StructureBehaviours structure, Item item) {
        if (marketType == SturctureMarketType.Station) {
            foreach (FactoryHandler factoryHandler in structure.factories)
                foreach (Item output in factoryHandler.factory.outputs)
                    if (output == item)
                        return item.basePrice * 0.95f;
            return -1;
        } else {
            return item.basePrice;
        }
    }
}

public enum SturctureMarketType {
    Station,
    Ship
}