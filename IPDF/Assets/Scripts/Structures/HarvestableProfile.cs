using UnityEngine;

[CreateAssetMenu(fileName = "New Harvestable Profile", menuName = "Structures/Harvestable/Profile")]
public class HarvestableProfile : StructureProfile {
    [Header ("Stats")]
    public Item harvestItem;
}