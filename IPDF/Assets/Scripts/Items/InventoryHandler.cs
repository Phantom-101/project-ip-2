using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;
using System.Linq;

[Serializable]
public class InventoryHandler {
    public StructureBehaviours storage;
    [SerializeReference] public Dictionary<Item, int> inventory;
    public float inventorySize;

    public InventoryHandler (Dictionary<Item, int> inventory = null, float inventorySize = 0.0f) {
        if (inventory == null) this.inventory = new Dictionary<Item, int> ();
        else this.inventory = inventory;
        if (inventorySize >= 0.0f) this.inventorySize = inventorySize;
        else this.inventorySize = -inventorySize;
    }

    public InventoryHandler (InventoryHandler inventoryHandler) {
        this.inventory = inventoryHandler.inventory;
        if (this.inventory == null) this.inventory = new Dictionary<Item, int> ();
        if (this.inventorySize != storage.profile.inventorySize) this.inventorySize = storage.profile.inventorySize;
    }

    public float GetStoredSize () {
        float res = 0.0f;
        if (inventory == null) inventory = new Dictionary<Item, int> ();
        foreach (Item item in inventory.Keys.ToArray ()) res += item.size * inventory[item];
        return res;
    }

    public float GetAvailableSize () {
        return inventorySize - GetStoredSize ();
    }

    public int GetItemCount (Item item) {
        return inventory.ContainsKey (item) ? inventory[item] : 0;
    }

    public bool AddItem (Item item, int amount) {
        float sizeRequired = item.size * amount;
        float availableSize = GetAvailableSize ();
        if (sizeRequired <= availableSize) {
            SetValue (item, GetItemCount (item) + amount);
            return true;
        }
        return true;
    }

    public bool HasItemCount (Item item, int amount) {
        float trueAmount = GetItemCount (item);
        if (trueAmount >= amount) return true;
        return false;
    }

    public bool RemoveItem (Item item, int amount) {
        if (HasItemCount (item, amount)) {
            SetValue (item, GetItemCount (item) - amount);
            return true;
        }
        return false;
    }

    public void SetValue (Item item, int target) {
        inventory[item] = target;
    }

    public int RoomFor (Item item) {
        return (int) Mathf.Floor (GetAvailableSize () / item.size);
    }
}