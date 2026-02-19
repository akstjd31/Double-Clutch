using System;
using UnityEngine;

[Serializable]
public struct Event_ScriptSelectorData
{
    public int selectorId;
    public string eventId;
    public coreType selectCoreType;
    public string scriptId;


    public Event_ScriptSelectorData(int selectorId, string eventId, coreType coreType, string scriptId)
    {
        this.selectorId = selectorId;
        this.eventId = eventId;
        selectCoreType = coreType;
        this.scriptId = scriptId;
    }

}
