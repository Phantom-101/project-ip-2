using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigModuleAttachmentPoint : ActiveModuleAttachmentPoint
{
    public string targetStatName;
    public StructureStatModifierType modifierType;
    public float value;

    protected override void OnEachActivate()
    {
        base.OnActivate();
        transform.parent.parent.parent.GetComponent<StructureStatsManager>().AddModifier(targetStatName, new StructureStatModifier(modifierType, value));
    }
}
