using UnityEngine;
using System;

[Serializable]
public struct PlayerSynergyData
{
    public string synergyId;
    public string traitId1;
    public string traitId2;
    public string synergyName;
    public effectType effectType;
    public float effectValue;
    public string synergyDesc;
    public string synergyResource;

    public PlayerSynergyData(string synergyId, string traitId1, string traitId2, string synergyName, effectType effectType, float effectValue, string synergyDesc, string synergyResource)
    {
        this.synergyId = synergyId;
        this.traitId1 = traitId1;
        this.traitId2 = traitId2;
        this.synergyName = synergyName;
        this.effectType = effectType;
        this.effectValue = effectValue;
        this.synergyDesc = synergyDesc;
        this.synergyResource = synergyResource;
    }
}