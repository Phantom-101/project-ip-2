using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsHandler : MonoBehaviour {
    public Item[] items;

    public string GetItemId (Item item) {
        return item.name;
    }

    public Item GetItemById (string id) {
        foreach (Item item in items) if (item.name == id) return item;
        return null;
    }
}
