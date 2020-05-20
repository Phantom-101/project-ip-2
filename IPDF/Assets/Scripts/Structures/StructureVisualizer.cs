using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StructureVisualizer : MonoBehaviour {
    void OnDrawGizmos () {
        StructureBehaviours structureBehaviours = GetComponent<StructureBehaviours> ();
        if (structureBehaviours != null && !structureBehaviours.initialized && structureBehaviours.profile != null) {
            Gizmos.color = structureBehaviours.faction == "Player" ? Color.blue : Color.red;
            Quaternion rot = Quaternion.Euler (structureBehaviours.profile.rotate) * transform.rotation;
            Gizmos.DrawMesh (structureBehaviours.profile.mesh, transform.position + transform.rotation * structureBehaviours.profile.offset, rot, Vector3.one);
        }
    }
}
