using UnityEngine;

[CreateAssetMenu (fileName = "New Music Trigger", menuName = "Music Triggers/Combat Size")]
public class CombatSizeTrigger : MusicTrigger {
    public int minSize;
    public int maxSize;
    public float distanceThreshold;

    public override bool CanBeUsed (StructuresManager structures, StructureBehaviours player) {
        FactionsManager factionsManager = FactionsManager.GetInstance ();
        foreach (StructureBehaviours structure in structures.structures) {
            if (factionsManager.Hostile (structure.faction, player.faction) &&
                (structure.transform.position - player.transform.position).sqrMagnitude <= distanceThreshold * distanceThreshold &&
                structure.profile.apparentSize >= minSize && structure.profile.apparentSize <= maxSize) {
                return true;
            }
        }
        return false;
    }
}
