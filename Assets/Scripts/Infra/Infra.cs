using System;
using System.Collections.Generic;

[Serializable]
public class Infra
{
    public string name;
    public string desc;
    public string nameKey;  // 추가
    public string descKey;  // 추가
    public int currentLevel;
    public int maxLevel;
    public int groupId;
    public infraEffectType infraEffectType;
    public List<int> infraEffectValue;

    public Infra(string name, string desc, string nameKey, string descKey, int maxLevel, int groupId)
    {
        this.name = name;
        this.desc = desc;
        this.nameKey = nameKey; 
        this.descKey = descKey;  
        this.maxLevel = maxLevel;
        this.groupId = groupId;
        infraEffectType = (infraEffectType)groupId;
    }

    public void SetInfraEffectValueList(List<int> infraEV) => infraEffectValue = infraEV;
}
