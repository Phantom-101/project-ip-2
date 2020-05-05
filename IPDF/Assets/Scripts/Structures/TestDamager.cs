using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestDamager : MonoBehaviour {
    public StructureBehaviours target;

    public void ApplyDamage () {
        if (target == null) return;
        target.TakeDamage (10.0f, transform.position);
    }
}

[CustomEditor (typeof (TestDamager))]
public class TestDamagerEditor : Editor {
    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        TestDamager testDamager = (TestDamager) target;
        if (GUILayout.Button ("Apply Damage")) {
            testDamager.ApplyDamage ();
        }
    }
}