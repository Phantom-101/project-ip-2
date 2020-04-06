using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAttachmentPoint : ActiveModuleAttachmentPoint
{
    public Turret turret;
    public GameObject target;
    public Ammunition loaded;
    public int amount;
    public bool activatedThisCycle;
    public int activatedCount;
    public float currentCycleTime;

    public void Initialize() {
        if(turret.LRO != null) {
            for(int i = 0; i < turret.activationBursts; i++) {
                GameObject lro = Instantiate(turret.LRO) as GameObject;
                lro.transform.parent = transform;
                lro.GetComponent<LineRenderObject>().firedFrom = turret;
            }
        }
    }

    public override void SetModuleActive(bool a) {
        base.SetModuleActive(a);
        if(!a) {
            if(turret.cycleInterruptable) {
                if(!activatedThisCycle) OnCycleInterrupt();
            }
        }
    }

    protected override void OnCycleStart() {
        base.OnCycleStart();
        currentCycleTime = turret.cycleTime + Random.Range(-turret.cycleTimeVariation, turret.cycleTimeVariation);
    }

    protected override void ElapseCycle() {
        base.ElapseCycle();
        if(turret.activationAnchor == ActivationAnchor.Start) {
            if(cycleElapsed >= turret.activationOffset && !activatedThisCycle) {
                if(target == null) SetModuleActive(false);
                else StartCoroutine(OnActivate());
            }
        } else {
            if(turret.cycleTime - cycleElapsed <= turret.activationOffset && !activatedThisCycle) StartCoroutine(OnActivate());
        }
        if(cycleElapsed >= currentCycleTime) OnCycleEnd();
    }

    protected override void OnEachActivate() {
        base.OnEachActivate();
        if(target != null) {
            float damageMult = transform.parent.parent.GetComponent<StructureStatsManager>().GetStat("structure turret damage multiplier");
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float maxAccu;
            if (distance > turret.falloffRange) maxAccu = 0.0f;
            else if (distance > turret.optimalRange) maxAccu = (distance - turret.optimalRange) / (turret.falloffRange - turret.optimalRange);
            else maxAccu = 1.0f;
            target.GetComponent<StructureStatsManager>().TakeDamage(loaded.damage * turret.damageMultiplier * damageMult, transform.parent.parent.gameObject);
        }
    }

    protected override IEnumerator OnActivate() {
        base.OnActivate();
        activatedThisCycle = true;
        for(int i = 0; i < turret.activationBursts; i++) {
            OnEachActivate();
            if(turret.LRO != null) HandleLRO(activatedCount);
            activatedCount++;
            yield return new WaitForSeconds(turret.burstInterval);
        }
    }

    void HandleLRO(int n) {
        transform.GetChild(n).GetComponent<LineRenderObject>().from = transform.position;
        if(target == null) return;
        transform.GetChild(n).GetComponent<LineRenderObject>().to = target.transform.position;
        transform.GetChild(n).GetComponent<LineRenderObject>().Activate();
    }

    protected override void OnCycleEnd() {
        base.OnCycleEnd();
        activatedCount = 0;
        if(!turret.repeating) SetModuleActive(false);
        activatedThisCycle = false;
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
