using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuresManager : MonoBehaviour {
    public GameObject explosion;

    List<StructureStatsManager> sbs = new List<StructureStatsManager>();
    List<StructureBehaviours> structureBehaviours = new List<StructureBehaviours>();

    public void AddStructure(StructureStatsManager ssm) {
        sbs.Add(ssm);
    }

    public void ProcessTick() {
        foreach(StructureBehaviours structure in structureBehaviours) {
            structure.ApplyMovementVectors();
            structure.ApplyHealthChanges();
            if (structure.GetLayerHealth(0) == 0.0f) Destroyed(structure);
        }
    }

    public List<StructureStatsManager> GetStructures() {
        return sbs;
    }

    public void Destroyed(StructureStatsManager sb) {
        sbs.Remove(sb);
        Instantiate(explosion, sb.transform.position, Quaternion.identity);
        Destroy(sb.gameObject);
    }
    
    public void Destroyed(StructureBehaviours structure) {
        structureBehaviours.Remove(structure);
        Instantiate(explosion, structure.transform.position, Quaternion.identity);
        Destroy(structure.gameObject);
    }
}
