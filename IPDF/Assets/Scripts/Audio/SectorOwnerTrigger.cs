using UnityEngine;

[CreateAssetMenu (fileName = "New Music Trigger", menuName = "Music Triggers/Sector Owner")]
public class SectorOwnerTrigger : MusicTrigger {
    public SectorOwnerState targetOwnerState;

    public override bool CanBeUsed (StructuresManager structures, StructureBehaviours player) {
        FactionsManager factionsManager = FactionsManager.GetInstance ();
        switch (targetOwnerState) {
            case SectorOwnerState.Ally:
                if (player.faction.id != player.sector.controllerID && factionsManager.Ally (player.faction, factionsManager.GetFaction (player.sector.controllerID))) return true;
                return false;
            case SectorOwnerState.Hostile:
                if (factionsManager.Hostile (player.faction, factionsManager.GetFaction (player.sector.controllerID))) return true;
                return false;
            case SectorOwnerState.Player:
                if (player.faction.id == player.sector.controllerID) return true;
                return false;
            default:
                return false;
        }
    }
}

public enum SectorOwnerState {
    Ally,
    Hostile,
    Player
}