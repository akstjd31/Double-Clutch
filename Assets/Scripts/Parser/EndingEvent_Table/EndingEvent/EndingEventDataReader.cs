using GoogleSheetsToUnity;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/EndingEventDataReader", order = int.MaxValue)]
public class EndingEventDataReader : DataReaderBase
{
    [SerializeField] public List<EndingEventData> DataList = new List<EndingEventData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string eventId = null;
        bool isDuplicatable = false;
        string desc = null;        

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "eventId":
                    eventId = val;
                    break;

                case "isDuplicatable":
                    ParseBool(val);
                    break;

                case "desc":
                    desc = val;
                    break;
            }
        }

        var endingEventData = new EndingEventData(eventId, isDuplicatable, desc);

        DataList.Add(endingEventData);
    }
    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes" || val == "TRUE";
    }
}