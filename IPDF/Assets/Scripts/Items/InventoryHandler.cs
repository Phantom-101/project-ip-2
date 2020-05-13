using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[Serializable]
public class InventoryHandler {
    public StructureBehaviours storage;
    public Dictionary<Item, float> inventory;
    public float inventorySize;
    
    public InventoryHandler (StructureBehaviours storage, Dictionary<Item, float> inventory = null, float inventorySize = 0.0f) {
        this.storage = storage;
        if (inventory == null) this.inventory = new Dictionary<Item, float> ();
        else this.inventory = inventory;
        if (inventorySize >= 0.0f) this.inventorySize = inventorySize;
        else this.inventorySize = -inventorySize;
    }

    public InventoryHandler (InventoryHandler inventoryHandler, StructureBehaviours storage) {
        this.storage = storage;
        this.inventory = inventoryHandler.inventory;
    }

    public float GetStoredSize () {
        float res = 0.0f;
        foreach (Item item in inventory.Keys) res += item.size * inventory[item];
        return res;
    }

    public float GetAvailableSize () {
        return inventorySize - GetStoredSize ();
    }

    public float GetItemCount (Item item) {
        return inventory.ContainsKey (item) ? inventory[item] : 0.0f;
    }

    public bool AddItem (Item item, float amount) {
        amount -= amount % item.partialSize;
        float sizeAvailableFor = GetAvailableSize () / item.size;
        sizeAvailableFor -= sizeAvailableFor % item.partialSize;
        if (sizeAvailableFor < amount) return false;
        inventory.Add (item, GetItemCount (item) + amount);
        return true;
    }

    public bool HasItemCount (Item item, float amount) {
        float trueAmount = GetItemCount (item);
        if (trueAmount >= amount) return true;
        return false;
    }

    public bool RemoveItem (Item item, float amount) {
        amount -= amount % item.partialSize;
        if (HasItemCount (item, amount)) {
            inventory.Add (item, GetItemCount (item) - amount);
            return true;
        }
        return false;
    }
}