using System;
using UnityEngine;

[Serializable]
public class Event_ConfigData
{
    public string logEventCode;
    public string textTemplate;
    public int scAdd;
    public string soundResourceId;
    public string cutInResourceId;
    public string videoResourceId;

    public Event_ConfigData(string logEC, string tTemplate, int scAdd, string sRId, string cutInRId, string vRId)
    {
        logEventCode = logEC;
        textTemplate = tTemplate;
        this.scAdd = scAdd;
        soundResourceId = sRId;
        cutInResourceId = cutInRId;
        videoResourceId = vRId;
    }
}
