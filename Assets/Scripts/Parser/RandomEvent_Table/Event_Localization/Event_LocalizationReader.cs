using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_ChoiceDataReader", menuName = "Scriptable Object/Event_ChoiceDataReader", order = int.MaxValue)]
public class Event_LocalizationReader : DataReaderBase
{
    [SerializeField] public List<Event_LocalizationData> DataList = new List<Event_LocalizationData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string textKey = null;
        string KR = null;
        string EN = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";
            
            switch (col)
            {
                case "textKey":
                    textKey = val;
                    break;

                case "KR":
                    KR = val;
                    break;

                case "EN":
                    EN = val;
                    break;

            }
        }


        if (textKey == "") return;
        
        var synergyData = new Event_LocalizationData
        (
            textKey, KR, EN
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
