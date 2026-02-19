using System;
using UnityEngine;

[Serializable]
public struct Event_LocalizationData
{
    public string textKey;
    public string KR;
    public string EN;

    public Event_LocalizationData(string textKey, string KR, string EN)
    {
        this.textKey = textKey;
        this.KR = KR;
        this.EN = EN;
    }
}
