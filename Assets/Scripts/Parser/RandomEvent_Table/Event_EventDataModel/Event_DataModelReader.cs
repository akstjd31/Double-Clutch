using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_DataModelReader", menuName = "Scriptable Object/Event_DataModelReader", order = int.MaxValue)]
public class Event_DataModelReader : DataReaderBase
{
    [SerializeField] public List<Event_DataModel> DataList = new List<Event_DataModel>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string eventId = null;
        potentialType mainPotentialType = default;
        int requiredPotentialValue = 0;
        float potentialPercent = 0;
        string eventPriority = null;
        int cooldownTurn = 0;

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

                case "mainPotentialType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) mainPotentialType = (potentialType)eInt;
                        else if (Enum.TryParse(val, true, out potentialType e)) mainPotentialType = e;
                    }
                    break;

                case "requiredPotentialValue":
                    int.TryParse(val, out requiredPotentialValue);
                    break;

                case "potentialPercent":
                    float.TryParse(val, out potentialPercent);
                    break;
                
                case "eventPriority":
                    eventPriority = val;
                    break;
                
                case "cooldownTurn":
                    int.TryParse(val, out cooldownTurn);
                    break;
            }
        }


        if (eventId == "") return;
        
        var synergyData = new Event_DataModel
        (
            eventId, mainPotentialType, requiredPotentialValue, potentialPercent, eventPriority, cooldownTurn
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
