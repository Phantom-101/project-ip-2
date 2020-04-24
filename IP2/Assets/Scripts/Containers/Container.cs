using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {
    public List<ContainerItem> items;

    public IntContainerItem GetIntContainerItem (int index) {
        return items[index].GetAsIntContainerItem();
    }

    public LongContainerItem GetLongContainerItem (int index) {
        return items[index].GetAsLongContainerItem();
    }

    public FloatContainerItem GetFloatContainerItem (int index) {
        return items[index].GetAsFloatContainerItem();
    }

    public DoubleContainerItem GetDoubleContainerItem (int index) {
        return items[index].GetAsDoubleContainerItem();
    }

    public StringContainerItem GetStringContainerItem (int index) {
        return items[index].GetAsStringContainerItem();
    }

    public BoolContainerItem GetBoolContainerItem (int index) {
        return items[index].GetAsBoolContainerItem();
    }

    public GameObjectContainerItem GetGameObjectContainerItem (int index) {
        return items[index].GetAsGameObjectContainerItem();
    }
}
