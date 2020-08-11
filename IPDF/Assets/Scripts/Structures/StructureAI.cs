using UnityEngine;

public class StructureAI {
    public float lastUpdated;
    public float delay;
    public float optimalRange;

    public StructureAI () {
        lastUpdated = 0;
        delay = Random.Range (1, 2.5f);
    }

    public virtual void Process (StructureBehaviours structureBehaviours, float deltaTime) {
        lastUpdated += deltaTime;
        if (lastUpdated < delay) return;
        lastUpdated = 0;
        delay = Random.Range (1, 2.5f);
        if (structureBehaviours.targeted == null || Vector3.Distance (structureBehaviours.transform.position, structureBehaviours.targeted.transform.position) > optimalRange) {
            float leastWeight = float.MaxValue;
            foreach (StructureBehaviours structure in structureBehaviours.sector.inSector) {
                if (structure != null && structure.CanBeTargeted () && structure.profile.canFireAt) {
                    float distance = Vector3.Distance (structureBehaviours.transform.position, structure.transform.position);
                    if (structure != structureBehaviours &&
                        structureBehaviours.factionsManager.Hostile (structureBehaviours.faction, structure.faction) &&
                        distance < leastWeight) {
                        leastWeight = distance;
                        structureBehaviours.targeted = structure;
                    }
                }
            }
        }
        if (structureBehaviours.targeted != null) {
            float totalRange = 0;
            int effectiveTurrets = 0;
            foreach (TurretHandler turretHandler in structureBehaviours.turrets) {
                //if (turretHandler.activated && turretHandler.target != structureBehaviours.targeted) turretHandler.Deactivate ();
                turretHandler.Activate (structureBehaviours.targeted.gameObject);
                Turret turret = turretHandler.turret;
                if (turret != null) {
                    totalRange += turret.range;
                    effectiveTurrets ++;
                }
            }
            if (structureBehaviours.route == null) {
                optimalRange = effectiveTurrets == 0 ? 1000 : totalRange / effectiveTurrets * structureBehaviours.profile.engagementRangeMultiplier;
                structureBehaviours.engine.forwardSetting = 1.0f;
                Vector3 heading = structureBehaviours.targeted.transform.position - structureBehaviours.transform.position;
                Vector3 perp = Vector3.Cross (structureBehaviours.transform.forward, heading);
                float leftRight = Vector3.Dot (perp, structureBehaviours.transform.up);
                float angle = structureBehaviours.targeted.transform.position - structureBehaviours.transform.position == Vector3.zero ?
                        0 :
                        Quaternion.Angle (structureBehaviours.transform.rotation, Quaternion.LookRotation (structureBehaviours.targeted.transform.position
                    - structureBehaviours.transform.position)
                );
                float lrMult = leftRight >= 0 ? 1 : -1;
                angle *= lrMult;
                float approachAngle = 90 * lrMult;
                float sqrDis = (structureBehaviours.targeted.transform.position - structureBehaviours.transform.position).sqrMagnitude;
                approachAngle -= sqrDis > optimalRange * optimalRange ? structureBehaviours.profile.rangeChangeAngle * lrMult : 0;
                approachAngle += sqrDis < optimalRange * optimalRange * 0.75f ? structureBehaviours.profile.rangeChangeAngle * lrMult : 0;
                if (angle > approachAngle) structureBehaviours.engine.turnSetting = 1;
                else if (angle > 0 && angle < approachAngle * 0.9f) structureBehaviours.engine.turnSetting = -1;
                else if (angle < -approachAngle) structureBehaviours.engine.turnSetting = -1;
                else if (angle < 0 && angle > -approachAngle * 0.9f) structureBehaviours.engine.turnSetting = 1;
                else structureBehaviours.engine.turnSetting = 0;
            }
        } else {
            if (structureBehaviours.route == null) {
                structureBehaviours.engine.forwardSetting = 0;
                structureBehaviours.engine.turnSetting = 0;
            }
        }
        if (structureBehaviours.route != null)
            if (structureBehaviours.route.waypoints != null) {
                if (structureBehaviours.route.waypoints.Count == 0)
                    structureBehaviours.route = null;
            } else structureBehaviours.route = null;
        if (structureBehaviours.route != null) {
            Vector3 heading = structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position;
            Vector3 perp = Vector3.Cross (structureBehaviours.transform.forward, heading);
            float leftRight = Vector3.Dot (perp, structureBehaviours.transform.up);
            float angle = structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position == Vector3.zero ?
                    0.0f :
                    Quaternion.Angle (structureBehaviours.transform.rotation, Quaternion.LookRotation (structureBehaviours.route.waypoints[0]
                - structureBehaviours.transform.position)
            );
            float lrMult = leftRight >= 0 ? 1 : -1;
            angle *= lrMult;
            float approachAngle = 0;
            if (angle > approachAngle) structureBehaviours.engine.turnSetting = 1;
            else if (angle > 0 && angle < approachAngle * 0.9f) structureBehaviours.engine.turnSetting = -1;
            else if (angle < -approachAngle) structureBehaviours.engine.turnSetting = -1;
            else if (angle < 0 && angle > -approachAngle * 0.9f) structureBehaviours.engine.turnSetting = 1;
            else structureBehaviours.engine.turnSetting = 0;
            structureBehaviours.engine.forwardSetting = 1;
            if ((structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position).sqrMagnitude <= 2750) structureBehaviours.route.ReachedWaypoint ();
        }
    }
}
