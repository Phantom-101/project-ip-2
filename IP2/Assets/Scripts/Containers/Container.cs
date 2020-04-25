using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Container<T> {
    public T value;

    public Container (T value) {
        this.value = value;
    }
}
