using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsHandler : MonoBehaviour {
    public static ItemsHandler current;

    public Item[] items;

    void Awake () {
        current = this;
    }

    public static ItemsHandler GetInstance () {
        return current;
    }

    public string GetItemId (Item item) {
        if (item == null) return "";
        return item.id;
    }

    public Item GetItemById (string id) {
        if (id == "") return null;
        foreach (Item item in items) if (item != null) if (item.id == id) return item;
        return null;
    }
}
