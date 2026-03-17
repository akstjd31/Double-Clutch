using System;
using System.Collections.Generic;

/// <summary>
/// 저장을 위한 인프라 클래스
/// </summary>
[Serializable]
public class Infra
{
    public string name;
    public string desc;
    public int currentLevel;
    public int maxLevel;
    public int groupId;
    public infraEffectType infraEffectType;
    public List<int> infraEffectValue;

    public Infra(string name, string desc, int maxLevel, int groupId)
    {
        this.name = name;
        this.desc = desc;
        this.maxLevel = maxLevel;
        this.groupId = groupId;

        infraEffectType = (infraEffectType)groupId;
    }

    public void SetInfraEffectValueList(List<int> infraEV) => infraEffectValue = infraEV;
}