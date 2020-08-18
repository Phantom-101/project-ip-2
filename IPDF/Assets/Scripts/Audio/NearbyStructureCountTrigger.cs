using UnityEngine;

[CreateAssetMenu (fileName = "New Music Trigger", menuName = "Music Triggers/Nearby Structure Count")]
public class NearbyStructureCountTrigger : MusicTrigger {
    public int countThreshold;
    public float distanceThreshold;

    public override bool CanBeUsed (StructuresManager structures, StructureBehaviours player) {
        int withinRange = 0;
        foreach (StructureBehaviours structure in structures.structures) {
            if (structure != player && structure.transform.parent == player.transform.parent) {
                if ((structure.transform.position - player.transform.position).sqrMagnitude <= distanceThreshold * distanceThreshold) {
                    withinRange++;
                }
            }
        }
        return withinRange >= countThreshold;
    }
}
