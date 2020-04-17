using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RigAttachmentPoint : ActiveModuleAttachmentPoint
{
    public Rig rig;
    public GameObject effect;
    public int activatedCount;
    public float currentCycleTime;

    StructureStatModifiersPackage modifiersPackage;
    VisualEffect visualEffect;
    StructureStatsManager fitterStatsManager;

    void Awake() {
        fitterStatsManager = transform.parent.parent.GetComponent<StructureStatsManager>();
    }

    public void Initialize() {
        if(rig.effect != null) {
            effect = Instantiate(rig.effect) as GameObject;
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector3.zero;
            visualEffect = effect.GetComponent<VisualEffect>();
        }
        List<StructureStatModifier> modifiers = new List<StructureStatModifier>();
        for(int i = 0; i < rig.targetStats.Length; i++) modifiers.Add(new StructureStatModifier(rig.targetStats[i], rig.modifierTypes[i], rig.values[i]));
        modifiersPackage = new StructureStatModifiersPackage(modifiers, rig.duration);
    }

    protected override void ElapseCycle() {
        base.ElapseCycle();
        for(int i = activatedCount; i < rig.activations.Length; i++) {
            if(cycleElapsed >= rig.activations[i]) {
                OnActivate(i);
                activatedCount++;
            }
        }
        if(cycleElapsed >= rig.cycleTime) OnCycleEnd();
    }

    protected override void OnActivate(int n) {
        base.OnActivate(n);
        fitterStatsManager.AddModifiersPackage(modifiersPackage);
        if(visualEffect != null) visualEffect.SendEvent("Activate");
    }

    protected override void OnCycleInterrupt() {
        base.OnCycleInterrupt();
        activatedCount = 0;
    }

    protected override void OnCycleEnd() {
        base.OnCycleEnd();
        activatedCount = 0;
        if(!rig.repeating) SetModuleActive(false);
    }
}
