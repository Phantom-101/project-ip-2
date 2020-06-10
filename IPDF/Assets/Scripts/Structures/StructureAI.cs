using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class StructureAI {
    public virtual void Process (StructureBehaviours structureBehaviours) {
        StructureBehaviours closest = null;
        float leastWeight = float.MaxValue;
        foreach (StructureBehaviours structure in structureBehaviours.structuresManager.structures) {
            if (structure != null && structure.profile != null && structure.profile.canFireAt) {
                float sizeDif = Mathf.Abs (structureBehaviours.profile.apparentSize - structure.profile.apparentSize);
                sizeDif = MathUtils.Clamp (sizeDif - 2, 1, 100);
                float distance = Vector3.Distance (structureBehaviours.transform.position, structure.transform.position);
                float weight = distance;
                if (structure != structureBehaviours && structure.factionID != structureBehaviours.factionID &&
                    structureBehaviours.factionsManager.GetRelations (structureBehaviours.factionID, structure.factionID) <= -0.5f && !structure.cloaked && weight < leastWeight &&
                    structure.transform.parent == structureBehaviours.transform.parent) {
                    leastWeight = weight;
                    closest = structure;
                }
            }
        }
        if (closest != null) {
            structureBehaviours.targeted = closest;
            float totalRange = 0.0f;
            int effectiveTurrets = 0;
            foreach (TurretHandler turretHandler in structureBehaviours.turrets) {
                turretHandler.Activate (structureBehaviours.targeted.gameObject);
                Turret turret = turretHandler.turret;
                if (turret != null) {
                    totalRange += turret.range;
                    effectiveTurrets ++;
                }
            }
            if (structureBehaviours.route == null) {
                float optimalRange = effectiveTurrets == 0 ? 1000.0f : totalRange / effectiveTurrets * structureBehaviours.profile.engagementRangeMultiplier;
                structureBehaviours.engine.forwardSetting = 1.0f;
                Vector3 heading = structureBehaviours.targeted.transform.position - structureBehaviours.transform.position;
                Vector3 perp = Vector3.Cross (structureBehaviours.transform.forward, heading);
                float leftRight = Vector3.Dot (perp, structureBehaviours.transform.up);
                float angle = structureBehaviours.targeted.transform.position - structureBehaviours.transform.position == Vector3.zero ?
                        0.0f :
                        Quaternion.Angle (structureBehaviours.transform.rotation, Quaternion.LookRotation (structureBehaviours.targeted.transform.position
                    - structureBehaviours.transform.position)
                );
                float lrMult = leftRight >= 0.0f ? 1.0f : -1.0f;
                angle *= lrMult;
                float approachAngle = 90.0f * lrMult;
                float sqrDis = (structureBehaviours.targeted.transform.position - structureBehaviours.transform.position).sqrMagnitude;
                approachAngle -= sqrDis > optimalRange * optimalRange ? structureBehaviours.profile.rangeChangeAngle * lrMult : 0.0f;
                approachAngle += sqrDis < optimalRange * optimalRange * 0.75f ? structureBehaviours.profile.rangeChangeAngle * lrMult : 0.0f;
                if (angle > approachAngle) structureBehaviours.engine.turnSetting = 1.0f;
                else if (angle > 0.0f && angle < approachAngle * 0.9) structureBehaviours.engine.turnSetting = -1.0f;
                else if (angle < -approachAngle) structureBehaviours.engine.turnSetting = -1.0f;
                else if (angle < 0.0f && angle > -approachAngle * 0.9) structureBehaviours.engine.turnSetting = 1.0f;
                else structureBehaviours.engine.turnSetting = 0.0f;
            }
        } else {
            if (structureBehaviours.route == null) {
                structureBehaviours.engine.forwardSetting = 0.0f;
                structureBehaviours.engine.turnSetting = 0.0f;
            }
        }
        if (structureBehaviours.route != null)
            if (structureBehaviours.route.waypoints.Count == 0)
                structureBehaviours.route = null;
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
            else if (angle > 0 && angle < approachAngle * 0.9) structureBehaviours.engine.turnSetting = -1;
            else if (angle < -approachAngle) structureBehaviours.engine.turnSetting = -1;
            else if (angle < 0 && angle > -approachAngle * 0.9) structureBehaviours.engine.turnSetting = 1;
            else structureBehaviours.engine.turnSetting = 0;
            structureBehaviours.engine.forwardSetting = 1;
            if ((structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position).sqrMagnitude <= 650) structureBehaviours.route.ReachedWaypoint ();
        }
    }
}
