using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Container<T> {
    public T value;

    public Container (T value) {
        this.value = value;
    }
}
