using UnityEngine;
using System;
[Serializable]
public struct Training_MappingData
{
    public potential potential;
    public int category;
    public string desc;
    public string categorydescKey;

    public Training_MappingData(potential potential, int category, string desc, string categorydescKey)
    {
        this.potential = potential;
        this.category = category;
        this.desc = desc;
        this.categorydescKey = categorydescKey;
    }
}
