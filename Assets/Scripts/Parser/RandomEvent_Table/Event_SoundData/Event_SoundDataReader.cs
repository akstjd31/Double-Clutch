using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Event_SoundDataReader", menuName = "Scriptable Object/Event_SoundDataReader", order = int.MaxValue)]
public class Event_SoundDataReader : DataReaderBase
{
    [SerializeField] public List<Event_SoundData> DataList = new List<Event_SoundData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string soundId = null;
        string filePath = null;
        string soundType = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";
            
            switch (col)
            {
                case "soundId":
                    soundId = val;
                    break;
                case "filePath":
                    filePath = val;
                    break;
                case "soundType":
                    soundType = val;
                    break;
            }
        }


        if (soundId == "") return;
        
        var synergyData = new Event_SoundData
        (
            soundId, filePath, soundType
        );

        DataList.Add(synergyData);
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
