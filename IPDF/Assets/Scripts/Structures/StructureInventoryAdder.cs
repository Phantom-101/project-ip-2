using UnityEngine;

public class StructureInventoryAdder : MonoBehaviour {
    public Item[] items;
    public int[] counts;

    void Start () {
        StructureBehaviours structureBehaviours = GetComponent<StructureBehaviours> ();
        if (structureBehaviours != null) {
            for (int i = 0; i < items.Length; i++) {
                structureBehaviours.inventory.AddItem (items[i], counts[i]);
            }
        }
        Destroy (this);
    }
}
