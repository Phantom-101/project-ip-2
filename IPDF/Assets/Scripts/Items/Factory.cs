using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Factory", menuName = "Factory")]
public class Factory : Item {
    public Item[] inputs;
    public int[] inputQuantities;
    public Item[] outputs;
    public int[] outputQuantities;
    public float time;
    public float wealthChange;
}

[Serializable]
public class FactoryHandler {
    public StructureBehaviours structure;
    public Factory factory;
    public string mountedID;
    public bool online;
    public float timer;

    public FactoryHandler (Factory factory = null) {
        this.factory = factory;
        if (factory == null) this.online = false;
        else this.online = true;
        this.timer = 0;
    }

    public FactoryHandler (FactoryHandler factoryHandler) {
        if (factoryHandler == null) {
            this.factory = null;
            this.online = false;
            this.timer = 0;
        } else {
            this.factory = factoryHandler.factory;
            this.online = factoryHandler.online;
            this.timer = factoryHandler.timer;
        }
    }

    public void SetOnline (bool target) {
        if (factory == null) {
            online = false;
            timer = 0.0f;
            return;
        }
        online = target;
    }

    public void Process () {
        if (!online) return;
        if (factory == null) return;
        if (timer == 0) {
            bool canStart = true;
            for (int i = 0; i < factory.inputs.Length; i++)
                if (structure.inventory.GetItemCount (factory.inputs[i]) < factory.inputQuantities[i])
                    canStart = false;
            if (canStart) {
                for (int i = 0; i < factory.inputs.Length; i++)
                    structure.inventory.RemoveItem (factory.inputs[i], factory.inputQuantities[i]);
                timer = 0.01f;
            }
        } else {
            if (timer < factory.time) timer += Time.deltaTime;
            if (timer >= factory.time) {
                float spaceRequired = 0;
                for (int i = 0; i < factory.outputs.Length; i++) spaceRequired += factory.outputs[i].size * factory.outputQuantities[i];
                float spaceAvailable = structure.inventory.GetAvailableSize ();
                if (spaceRequired <= spaceAvailable && structure.factionsManager.ChangeWealth (structure.factionID, (long) factory.wealthChange)) {
                    for (int i = 0; i < factory.outputs.Length; i++)
                        structure.inventory.AddItem (factory.outputs[i], factory.outputQuantities[i]);
                    timer = 0;
                }
            }
        }
    }
}