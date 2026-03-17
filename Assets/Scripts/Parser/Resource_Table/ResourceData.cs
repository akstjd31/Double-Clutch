using UnityEngine;
using System;
[Serializable]
public struct ResourceData
{
    public string resourceId;
    public string resourcePath;

    public ResourceData(string resourceId, string resourcePath)
    {
        this.resourceId = resourceId;
        this.resourcePath = resourcePath;
    }
}
