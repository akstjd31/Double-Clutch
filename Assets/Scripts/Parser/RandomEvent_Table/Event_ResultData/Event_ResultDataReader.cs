using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Event_ResultDataReader", menuName = "Scriptable Object/Event_ResultDataReader", order = int.MaxValue)]
public class Event_ResultDataReader : DataReaderBase
{
    [SerializeField] public List<Event_ResultData> DataList = new List<Event_ResultData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string resultId = null;
        string scriptId = null; 
        string matchPersonalityId = null; 
        int outcomeGrade = 0; 
        potentialType potentialChangeType = default;

        int potentialChangeValue = 0;
        int conditionChange = 0;
        string statusChange = null; 
        string reactionPortraitId = null; 
        string resultscriptKey = null; 

        string seId = null; 
        string floatingText = null; 

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "resultId":
                    resultId = val;
                    break;
                case "scriptId":
                    scriptId = val;
                    break;
                case "matchPersonalityId":
                    matchPersonalityId = val;
                    break;
                case "outcomeGrade":
                    int.TryParse(val, out outcomeGrade);
                    break;
                case "potentialChangeType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) potentialChangeType = (potentialType)eInt;
                        else if (Enum.TryParse(val, true, out potentialType e)) potentialChangeType = e;
                    }
                    break;

                case "potentialChangeValue":
                    int.TryParse(val, out potentialChangeValue);
                    break;
                case "conditionChange":
                    int.TryParse(val, out conditionChange);
                    break;
                case "statusChange":
                    statusChange = val;
                    break;
                case "reactionPortraitId":
                    reactionPortraitId = val;
                    break;
                case "resultscriptKey":
                    resultscriptKey = val;
                    break;

                case "seId":
                    seId = val;
                    break;
                case "floatingText":
                    floatingText = val;
                    break;
            }
        }


        if (resultId == "") return;
        
        var synergyData = new Event_ResultData
        (
            resultId, scriptId, matchPersonalityId, outcomeGrade, potentialChangeType,
            potentialChangeValue, conditionChange, statusChange, reactionPortraitId, resultscriptKey,
            seId, floatingText
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
