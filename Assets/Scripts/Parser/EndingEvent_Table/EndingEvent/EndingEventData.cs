using System;

[Serializable]
public struct EndingEventData
{
    public string eventId;
    public bool isDuplicatable;
    public string desc;


    public EndingEventData(string eventId, bool isDuplicatable, string desc)
    {
        this.eventId = eventId;
        this.isDuplicatable = isDuplicatable;
        this.desc = desc;
    }
}
