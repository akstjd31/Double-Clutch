using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_ConfigDataReader", menuName = "Scriptable Object/Event_ConfigDataReader", order = int.MaxValue)]
public class Event_ConfigDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Event_ConfigData> DataList = new List<Event_ConfigData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int idx = 0, scAdd = 0;
        string logEC = null, tTemplate = null, sRId = null, cutInRId = null, vRId = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";
            
            switch (col)
            {
                case "index":
                    int.TryParse(val, out idx);
                    break;
                    
                case "logEventCode":
                    logEC = val;
                    break;

                case "textTemplate":
                    tTemplate = val;
                    break;

                case "scAdd":
                    int.TryParse(val, out scAdd);
                    break;

                case "soundResourceId":
                    sRId = val;
                    break;

                case "cutInResourceId":
                    cutInRId = val;
                    break;

                case "videoResourceId":
                    vRId = val;
                    break;
            }
        }

        var eventConfigData = new Event_ConfigData
        (
            idx, logEC, tTemplate, scAdd, sRId, cutInRId, vRId
        );

        DataList.Add(eventConfigData);
    }
}
