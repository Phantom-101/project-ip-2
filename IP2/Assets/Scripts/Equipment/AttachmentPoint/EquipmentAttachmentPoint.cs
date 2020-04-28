using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EquipmentAttachmentPoint : MonoBehaviour {
    // Stats
    public float hitpoints = 0;
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
    // Audio source
    AudioSource audioSource;

    // Basic activation info
    public bool equipmentActive;
    public float cycleElapsed = 0.0f;
    public int activatedCount;
    public float currentCycleTime;

    void Awake() {
        // Get the StructureStatsManager component of the fitter
        fitterStatsManager = transform.parent.parent.GetComponent<StructureStatsManager>();
        // Add an audio source to this GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
        // Automatic equipment repair
        StartCoroutine(RepairEquipment());
    }

    public void Initialize() {
        // Initialize stats
        hitpoints = equipment.hitpoints;
        // Instantiate and store VFX
        if(equipment.vfx != null) {
            effect = Instantiate(equipment.vfx) as GameObject;
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector3.zero;
            visualEffect = effect.GetComponent<VisualEffect>();
        }
    }

    public virtual void SetEquipmentActive(bool a) {
        if(equipment == null) return;
        // If module is passive, return
        if (!equipment.activatable) return;
        // If module is activatable, check further conditions
        if (a) {
            if (equipment.mustBeTargeted) {
                if (target == null) return;
                if ((transform.position - target.transform.position).sqrMagnitude > equipment.range * equipment.range) return;
            }
        } else {
            if (equipment.cycleInterruptable && activatedCount == 0) OnCycleInterrupt();
        }
        equipmentActive = a;
    }

    void Update() {
        OnTick();
    }

    public void OnTick() {
        CheckState();
    }

    void CheckState() {
        if(equipment != null) {
            if(hitpoints < 0) hitpoints = 0;
            else if(hitpoints > equipment.hitpoints) hitpoints = equipment.hitpoints;
            if(equipmentActive){
                if(cycleElapsed == 0.0f) OnCycleStart();
                ElapseCycle();
            } else {
                if(cycleElapsed > 0.0f) ElapseCycle();
            }
        } else {
            equipmentActive = false;
            cycleElapsed = 0.0f;
        }
    }

    void ElapseCycle() {
        cycleElapsed += Time.deltaTime;
        if (equipment.mustBeTargeted && target == null) SetEquipmentActive(false);
        else if (equipment.mustBeTargeted && (transform.position - target.transform.position).sqrMagnitude > equipment.range * equipment.range) SetEquipmentActive(false);
        if (equipmentActive) {
            // Check if equipment should be activated
            for (int i = activatedCount; i < equipment.activations.Length; i++) {
                if (cycleElapsed >= equipment.activations[i]) {
                    // If it should, check requirements
                    bool shouldActivate = true;
                    for (int j = 0; j < equipment.requirementStats.Length; j++) {
                        float statValue = fitterStatsManager.GetStat(equipment.requirements[j]);
                        float mult = 1.0f;
                        foreach (string stat in equipment.requirementStats) mult *= fitterStatsManager.GetStat(stat);
                        if (!(statValue >= equipment.minValues[j] * mult && statValue <= equipment.maxValues[j] * mult)) {
                            shouldActivate = false;
                            break;
                        }
                    }
                    if(shouldActivate) OnActivate(i);
                    activatedCount++;
                }
            }
        }
        // If cycleElapsed is over the equipment's cycle time, end the cycle
        if (cycleElapsed >= equipment.cycleTime) OnCycleEnd();
    }

    void OnActivate(int n) {
        if (equipment.requireCharge) {
            if(amount <= 0) return;
            amount -= 1;
        }
        // If there is a visual effect, send event to activate it
        if(visualEffect != null) visualEffect.SendEvent("Activate");
        // Play audio
        if(equipment.activationClips.Length >= 1) audioSource.PlayOneShot(equipment.activationClips[Random.Range(0, equipment.activationClips.Length - 1)], 0.5f);
        // Setup packages
        StatModifiersPackage selfModifiersPackage = new StatModifiersPackage(new List<StatModifier>(),
            GetAffectedValue(equipment.duration, equipment.durationStats));
        StatModifiersPackage targetModifiersPackage = new StatModifiersPackage(new List<StatModifier>(),
            GetAffectedValue(equipment.duration, equipment.durationStats));
        // Add self-granted modifiers to cache list
        for(int i = 0; i < equipment.effects.Length; i++)
            if(!equipment.grantToTarget[i])
                selfModifiersPackage.modifiers.Add(new StatModifier(equipment.effects[i],
                    equipment.modifierTypes[i],
                    GetAffectedValue(equipment.values[i] * (loaded == null ? 1.0f : loaded.value), equipment.valueStats)));
        // Add target-granted modifiers to cache list
        if(target != null)
            for(int i = 0; i < equipment.effects.Length; i++)
                if(equipment.grantToTarget[i])
                    targetModifiersPackage.modifiers.Add(new StatModifier(equipment.effects[i],
                        equipment.modifierTypes[i],
                        GetAffectedValue(equipment.values[i] * (loaded == null ? 1.0f : loaded.value), equipment.valueStats)));
        // Add modifiers packages to both self (and target)
        fitterStatsManager.AddModifiersPackage(selfModifiersPackage);
        if(target != null) target.GetComponent<StructureStatsManager>().AddModifiersPackage(targetModifiersPackage);
        // Damage zone
        if(equipment.damageZoneProfile != null) {
            GameObject damageZoneApplier = new GameObject();
            if (target != null) damageZoneApplier.transform.position = target.transform.position;
            DamageZone damageZone = damageZoneApplier.AddComponent<DamageZone>();
            damageZone.damageZoneProfile = equipment.damageZoneProfile;
            damageZone.Initialize();
        }
    }

    void OnCycleStart() {}

    void OnCycleInterrupt() {
        cycleElapsed = 0.0f;
        activatedCount = 0;
    }

    void OnCycleEnd() {
        cycleElapsed = 0.0f;
        activatedCount = 0;
        if(equipment.repeating && equipmentActive) SetEquipmentActive(true);
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

    float GetAffectedValue(float baseStatValue, string[] affectors) {
        float mult = 1.0f;
        for(int i = 0; i < affectors.Length; i++) mult *= fitterStatsManager.GetStat(affectors[i]);
        return mult * baseStatValue;
    }

    public void ChangeHitpoints(float delta) {
        hitpoints += delta;
    }

    IEnumerator RepairEquipment() {
        hitpoints ++;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RepairEquipment());
    }
}
