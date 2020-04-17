using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAttachmentPoint : ActiveModuleAttachmentPoint
{
    public Turret turret;
    public GameObject target;
    public Ammunition loaded;
    public int amount;
    public int activatedCount;

    public void Initialize() {
        if(turret.LRO != null) {
            for(int i = 0; i < turret.activations.Length; i++) {
                GameObject lro = Instantiate(turret.LRO) as GameObject;
                lro.transform.parent = transform;
                lro.GetComponent<LineRenderObject>().firedFrom = turret;
            }
        }
    }

    public override void SetModuleActive(bool a) {
        base.SetModuleActive(a);
        if(!a) {
            if(turret.cycleInterruptable) OnCycleInterrupt();
        }
    }

    protected override void OnCycleStart() {
        base.OnCycleStart();
    }

    protected override void ElapseCycle() {
        base.ElapseCycle();
        if(target == null) SetModuleActive(false);
        for(int i = activatedCount; i < turret.activations.Length; i++) {
            if(cycleElapsed >= turret.activations[i]) {
                OnActivate(i);
                activatedCount++;
            }
        }
        if(cycleElapsed >= turret.cycleTime) OnCycleEnd();
    }

    protected override void OnActivate(int n) {
        if(!(loaded != null || turret.fireableWithoutAmmo)) return;
        base.OnActivate(n);
        if(target != null) {
            StructureStatsManager targetSSM = target.GetComponent<StructureStatsManager>();
            float damageMult = transform.parent.parent.GetComponent<StructureStatsManager>().GetStat("Turret Modules Damage Multiplier");
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float disDamageMult;
            if (distance > turret.falloffRange) disDamageMult = 0.0f;
            else if (distance > turret.optimalRange) disDamageMult = (distance - turret.optimalRange) / (turret.falloffRange - turret.optimalRange);
            else disDamageMult = 1.0f;
            disDamageMult *= turret.tracking / targetSSM.GetStat("Speed");
            if(disDamageMult > 1.0f) disDamageMult = 1.0f;
            else if (disDamageMult < 0.0f) disDamageMult = 0.0f;
            float damage = ((loaded == null) ? turret.baseDamage : loaded.damage) * turret.damageMultiplier * damageMult * disDamageMult;
            targetSSM.TakeDamage(damage, transform.parent.parent.gameObject);
        }
        if(turret.LRO != null) HandleLRO(n);
    }

    void HandleLRO(int n) {
        LineRenderObject lro = transform.GetChild(n).GetComponent<LineRenderObject>();
        lro.from = transform.position;
        if(target == null) return;
        lro.to = target.transform.position;
        lro.firedAs = loaded;
        lro.Activate();
    }

    protected override void OnCycleInterrupt() {
        base.OnCycleInterrupt();
        activatedCount = 0;
    }

    protected override void OnCycleEnd() {
        base.OnCycleEnd();
        activatedCount = 0;
        if(!turret.repeating) SetModuleActive(false);
    }

    public bool LoadAmmo(Ammunition a, int aa) {
        bool found = false;
        for(int i = 0; i < turret.accepted.Length; i++) {
            if(turret.accepted[i] == a) {
                found = true;
                break;
            }
        }
        if(found) {
            loaded = a;
            amount = aa;
            return true;
        } else return false;
    }
}
