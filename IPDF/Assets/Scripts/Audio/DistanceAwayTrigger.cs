using UnityEngine;

[CreateAssetMenu (fileName = "New Music Trigger", menuName = "Music Triggers/Distance Away")]
public class DistanceAwayTrigger : MusicTrigger {
    public float distanceThreshold;

    public override bool CanBeUsed (StructuresManager structures, StructureBehaviours player) {
        float minDist = float.MaxValue;
        foreach (StructureBehaviours structure in structures.structures) {
            if (structure != player && structure.transform.parent == player.transform.parent) {
                float dist = (structure.transform.position - player.transform.position).magnitude;
                minDist = Mathf.Min (minDist, dist);
            }
        }
        return minDist >= distanceThreshold;
    }
}
