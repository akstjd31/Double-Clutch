using UnityEngine;
using System;

[Serializable]
public struct PlayerSynergyData
{
    public int synergyId;
    public int traitID1;
    public int traitID2;
    public string synergyName;
    public effectType effectType;
    public float effectValue;

    public PlayerSynergyData(int synergyId, int traitID1, int traitID2, string synergyName, effectType effectType, float effectValue)
    {
        this.synergyId = synergyId;
        this.traitID1 = traitID1;
        this.traitID2 = traitID2;
        this.synergyName = synergyName;
        this.effectType = effectType;
        this.effectValue = effectValue;
    }
}