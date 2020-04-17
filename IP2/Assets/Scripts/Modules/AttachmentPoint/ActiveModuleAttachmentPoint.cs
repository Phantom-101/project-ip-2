using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveModuleAttachmentPoint : ModuleAttachmentPoint
{
    public bool moduleActive;
    public float cycleElapsed = 0.0f;

    public virtual void SetModuleActive(bool a) {
        moduleActive = a;
    }

    void Update() {
        OnTick();
    }

    public void OnTick() {
        CheckState();
    }

    protected void CheckState() {
        if(moduleActive){
            if(cycleElapsed == 0.0f) OnCycleStart();
            ElapseCycle();
        } else {
            if(cycleElapsed > 0.0f) ElapseCycle();
        }
    }

    protected virtual void ElapseCycle() {
        cycleElapsed += Time.deltaTime;
    }

    protected virtual void OnActivate(int n) {}

    protected virtual void OnCycleStart() {}

    protected virtual void OnCycleInterrupt() {
        cycleElapsed = 0.0f;
    }

    protected virtual void OnCycleEnd() {
        cycleElapsed = 0.0f;
    }
}
