using UnityEngine;

public class Event_ConfigData
{
    public int index;
    public string logEventCode;
    public string textTemplate;
    public int scAdd;
    public string soundResourceId;
    public string cutInResourceId;
    public string videoResourceId;

    public Event_ConfigData(int idx, string logEC, string tTemplate, int scAdd, string sRId, string cutInRId, string vRId)
    {
        index = idx;
        logEventCode = logEC;
        textTemplate = tTemplate;
        this.scAdd = scAdd;
        soundResourceId = sRId;
        cutInResourceId = cutInRId;
        videoResourceId = vRId;
    }
}
