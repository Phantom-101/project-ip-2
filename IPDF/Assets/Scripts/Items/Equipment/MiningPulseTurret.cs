using UnityEngine;

[CreateAssetMenu (fileName = "New Mining Pulse Turret", menuName = "Equipment/Turrets/Mining Pulse Turret")]
public class MiningPulseTurret : PulseTurret {
    [Header ("Mining Turret Stats")]
    public float damageToAsteroidsMultiplier;

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        MiningPulseProjectile miningPulseProjectile = projectile.GetComponent<MiningPulseProjectile> ();
        if (miningPulseProjectile == null) miningPulseProjectile = projectile.AddComponent<MiningPulseProjectile> ();
        miningPulseProjectile.handler = caller;
        miningPulseProjectile.from = caller.equipper;
        miningPulseProjectile.to = caller.target.GetComponent<StructureBehaviours> ();
        miningPulseProjectile.Initialize ();
        miningPulseProjectile.Enable ();
    }

    public override GameObject RetrieveFromPool (TurretHandler caller) {
        return caller.pooler.Retrieve (caller.pooler.miningPulsePool);
    }
}