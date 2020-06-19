using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamProjectile : Projectile {
    [Header ("Stats")]
    public float timeExistedFor;
    [Header ("Components")]
    public LineRenderer lineRenderer;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        lineRenderer = gameObject.AddComponent<LineRenderer> ();
        lineRenderer.materials[0] = (turret as BeamTurret).beamMaterial;
        lineRenderer.colorGradient = (turret as BeamTurret).beamGradient;
        lineRenderer.widthCurve = (turret as BeamTurret).beamWidth;
    }

    protected override void Process () {
        if (turret != handler.turret) Disable ();
        if (!handler.activated) Disable ();
        if (from == null || to == null) Disable ();
        lineRenderer.SetPosition (0, from.transform.position);
        lineRenderer.SetPosition (1, to.transform.position);
        to.TakeDamage (turret.damage * Time.deltaTime, from.transform.position);
    }

    protected override void Disable () {
        Destroy (gameObject);
        base.Disable ();
    }
}