using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsHandler : MonoBehaviour {
    public Item[] items;

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
