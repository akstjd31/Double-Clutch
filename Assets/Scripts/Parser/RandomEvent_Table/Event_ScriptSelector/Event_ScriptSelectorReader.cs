using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_ScriptSelectorReader", menuName = "Scriptable Object/Event_ScriptSelectorReader", order = int.MaxValue)]
public class Event_ScriptSelectorReader : DataReaderBase
{
    [SerializeField] public List<Event_ScriptSelectorData> DataList = new List<Event_ScriptSelectorData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int selectorId = 0;
        string eventId = null;
        coreType selectCoreType = default;
        string scriptId = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "selectorId":
                    int.TryParse(val, out selectorId);
                    break;

                case "coreType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) selectCoreType = (coreType)eInt;
                        else if (Enum.TryParse(val, true, out coreType e)) selectCoreType = e;
                    }
                    break;

                case "eventId":
                    eventId = val;
                    break;

                case "scriptId":
                    scriptId = val;
                    break;
            }
        }


        if (selectorId <= 0) return;

        var synergyData = new Event_ScriptSelectorData
        (
            selectorId, eventId, selectCoreType, scriptId
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
