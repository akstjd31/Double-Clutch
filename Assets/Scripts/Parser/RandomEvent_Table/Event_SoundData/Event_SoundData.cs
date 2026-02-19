using System;
using UnityEngine;

[Serializable]
public struct Event_SoundData
{
    public string soundId;
    public string filePath;
    public string soundType;

    public Event_SoundData(string soundId, string filePath, string soundType)
    {
        this.soundId = soundId;
        this.filePath = filePath;
        this.soundType = soundType;
    }
}
