using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ContainerItem {
    public IntContainerItem GetAsIntContainerItem(ContainerItem containerItem) {
        return containerItem as IntContainerItem;
    }

    public LongContainerItem GetAsLongContainerItem(ContainerItem containerItem) {
        return containerItem as LongContainerItem;
    }

    public FloatContainerItem GetAsFloatContainerItem(ContainerItem containerItem) {
        return containerItem as FloatContainerItem;
    }

    public DoubleContainerItem GetAsDoubleContainerItem(ContainerItem containerItem) {
        return containerItem as DoubleContainerItem;
    }

    public StringContainerItem GetAsStringContainerItem(ContainerItem containerItem) {
        return containerItem as StringContainerItem;
    }

    public BoolContainerItem GetAsBoolContainerItem(ContainerItem containerItem) {
        return containerItem as BoolContainerItem;
    }

    public GameObjectContainerItem GetAsGameObjectContainerItem(ContainerItem containerItem) {
        return containerItem as GameObjectContainerItem;
    }
}

public struct IntContainerItem: ContainerItem {
    public int value;
}

public struct LongContainerItem: ContainerItem {
    public long value;
}

public struct FloatContainerItem: ContainerItem {
    public float value;
}

public struct DoubleContainerItem: ContainerItem {
    public double value;
}

public struct StringContainerItem: ContainerItem {
    public string value;
}

public struct BoolContainerItem: ContainerItem {
    public bool value;
}

public struct GameObjectContainerItem : ContainerItem {
    public GameObject value;
}
