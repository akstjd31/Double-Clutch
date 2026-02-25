using System;
using UnityEngine;

[Serializable]
public struct Event_ScriptSelectorData
{
    public string selectorId;
    public string eventId;
    public coreType selectCoreType;
    public string scriptId;


    public Event_ScriptSelectorData(string selectorId, string eventId, coreType coreType, string scriptId)
    {
        this.selectorId = selectorId;
        this.eventId = eventId;
        selectCoreType = coreType;
        this.scriptId = scriptId;
    }

}
