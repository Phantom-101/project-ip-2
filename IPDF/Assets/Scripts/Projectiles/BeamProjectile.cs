using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamProjectile : Projectile {
    [Header ("Components")]
    public LineRenderer lineRenderer;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        lineRenderer = gameObject.AddComponent<LineRenderer> ();
        lineRenderer.material = (turret as BeamTurret).beamMaterial;
        lineRenderer.colorGradient = (turret as BeamTurret).beamGradient;
        lineRenderer.widthCurve = (turret as BeamTurret).beamWidth;
        transform.parent = from.transform.parent;
    }

    protected override void Process () {
        if (turret != handler.turret) Disable ();
        if (!handler.activated) Disable ();
        if (from == null || to == null) Disable ();
        transform.localPosition = from.transform.localPosition;
        Vector3 beamFrom = from.transform.position + from.transform.rotation * handler.position;
        lineRenderer.SetPosition (0, beamFrom);
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.position - beamFrom, out hit, turret.range)) {
            lineRenderer.SetPosition (1, hit.point);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null) hitStructure.TakeDamage (turret.damage * Time.deltaTime, beamFrom);
        }
    }

    protected override void Disable () {
        Destroy (gameObject);
        base.Disable ();
    }
}