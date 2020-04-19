using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EquipmentAttachmentPoint : MonoBehaviour {
    // Equipment
    public Equipment equipment;
    // GameObject effect
    public GameObject effect;
    // Target
    public GameObject target;
    // Charges
    public Charge loaded;
    public int amount;

    // VFX effect
    VisualEffect visualEffect;
    // StructureStatsManager of fitter
    StructureStatsManager fitterStatsManager;

    // Basic activation info
    public bool moduleActive;
    public float cycleElapsed = 0.0f;
    public int activatedCount;
    public float currentCycleTime;

    void Awake() {
        // Get the StructureStatsManager component of the fitter
        fitterStatsManager = transform.parent.parent.GetComponent<StructureStatsManager>();
    }

    public void Initialize() {
        // Instantiate and store VFX
        if(equipment.vfx != null) {
            effect = Instantiate(equipment.vfx) as GameObject;
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector3.zero;
            visualEffect = effect.GetComponent<VisualEffect>();
        }

        // Initialize based on equipment type
        if(equipment.GetType() == typeof(StatsModificationEquipment)) InitializeAsStatsModification();
    }

    void InitializeAsStatsModification() {}

    public virtual void SetModuleActive(bool a) {
        // If module is passive, return
        if(!equipment.activatable) return;
        // If module is activatable, check further conditions
        if(a) {
            if(equipment.mustBeTargeted && target == null) return;
            if(equipment.requireCharge && (loaded == null || amount <= 0)) return;
        } else {
            if(equipment.cycleInterruptable && activatedCount == 0) OnCycleInterrupt();
        }
        moduleActive = a;
    }

    void Update() {
        OnTick();
    }

    public void OnTick() {
        CheckState();
    }

    void CheckState() {
        if(moduleActive){
            if(cycleElapsed == 0.0f) OnCycleStart();
            ElapseCycle();
        } else {
            if(cycleElapsed > 0.0f) ElapseCycle();
        }
    }

    void ElapseCycle() {
        if(equipment.mustBeTargeted && target == null) SetModuleActive(false);
        cycleElapsed += Time.deltaTime;
        // Check if equipment should be activated
        for(int i = activatedCount; i < equipment.activations.Length; i++) {
            if(cycleElapsed >= equipment.activations[i]) {
                // If it should, activate and increase activatedCount
                OnActivate(i);
                activatedCount++;
            }
        }
        // If cycleElapsed is over the equipment's cycle time, end the cycle
        if(cycleElapsed >= equipment.cycleTime) OnCycleEnd();
    }

    void OnActivate(int n) {
        if(equipment.requireCharge) {
            if(amount <= 0) return;
            amount -= 1;
        }
        if(equipment.GetType() == typeof(StatsModificationEquipment)) OnActivateAsStatsModification(n);
    }

    void OnActivateAsStatsModification(int n) {
        // Convert equipment to StatsModificationEquipment
        StatsModificationEquipment statsModificationEquipment = equipment as StatsModificationEquipment;
        // Setup packages
        StructureStatModifiersPackage selfModifiersPackage = new StructureStatModifiersPackage(new List<StructureStatModifier>(), statsModificationEquipment.duration);
        StructureStatModifiersPackage targetModifiersPackage = new StructureStatModifiersPackage(new List<StructureStatModifier>(), statsModificationEquipment.duration);
        // Add self-granted modifiers to cache list
        for(int i = 0; i < statsModificationEquipment.effects.Length; i++)
            if(!statsModificationEquipment.grantToTarget[i])
                selfModifiersPackage.modifiers.Add(new StructureStatModifier(statsModificationEquipment.effects[i], statsModificationEquipment.modifierTypes[i], statsModificationEquipment.values[i] * (loaded == null ? 1.0f : loaded.value)));
        // Add target-granted modifiers to cache list
        if(target != null)
            for(int i = 0; i < statsModificationEquipment.effects.Length; i++)
                if(statsModificationEquipment.grantToTarget[i])
                    targetModifiersPackage.modifiers.Add(new StructureStatModifier(statsModificationEquipment.effects[i], statsModificationEquipment.modifierTypes[i], statsModificationEquipment.values[i] * (loaded == null ? 1.0f : loaded.value)));
        // Add modifiers packages to both self (and target)
        fitterStatsManager.AddModifiersPackage(selfModifiersPackage);
        if(target != null) target.GetComponent<StructureStatsManager>().AddModifiersPackage(targetModifiersPackage);
        // If there is a visual effect, send event to activate it
        if(visualEffect != null) visualEffect.SendEvent("Activate");
    }

    void OnCycleStart() {}

    void OnCycleInterrupt() {
        cycleElapsed = 0.0f;
        activatedCount = 0;
    }

    void OnCycleEnd() {
        cycleElapsed = 0.0f;
        activatedCount = 0;
        if(!equipment.repeating) SetModuleActive(false);
    }
    
    public bool LoadCharge(Charge c, int a) {
        bool found = false;
        for(int i = 0; i < equipment.accepted.Length; i++) {
            if(equipment.accepted[i] == c) {
                found = true;
                break;
            }
        }
        if(found) {
            loaded = c;
            amount = a;
            return true;
        } else return false;
    }
}
