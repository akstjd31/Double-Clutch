using UnityEngine;
using System;

[Serializable]
public struct PlayerSynergyData
{
    public string synergyId;
    public string traitID1;
    public string traitID2;
    public string synergyName;
    public effectType effectType;
    public float effectValue;
    public string synergyDesc;

    public PlayerSynergyData(string synergyId, string traitID1, string traitID2, string synergyName, effectType effectType, float effectValue, string synergyDesc)
    {
        this.synergyId = synergyId;
        this.traitID1 = traitID1;
        this.traitID2 = traitID2;
        this.synergyName = synergyName;
        this.effectType = effectType;
        this.effectValue = effectValue;
        this.synergyDesc = synergyDesc;
    }
}