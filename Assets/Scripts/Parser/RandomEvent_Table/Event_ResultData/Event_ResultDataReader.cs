using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Event_ResultDataReader", menuName = "Scriptable Object/Event_ResultDataReader", order = int.MaxValue)]
public class Event_ResultDataReader : DataReaderBase
{
    [SerializeField] public List<Event_ResultData> DataList = new List<Event_ResultData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string resultId = null;
        string choiceId = null;
        string scriptId = null; 
        int nextId = 0;
        personalityType matchPersonalityId = default; 
        potential potentialChangeType = default;

        int potentialChangeValue = 0;
        int conditionChange = 0;
        string statusChange = null; 
        string reactionPortraitId = null; 
        string resultScriptKey = null; 

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
                case "choiceId":
                    choiceId = val;
                    break;
                case "scriptId":
                    scriptId = val;
                    break;
                case "nextId":
                    int.TryParse(val, out nextId);
                    break;
                case "matchPersonalityId":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) matchPersonalityId = (personalityType)eInt;
                        else if (Enum.TryParse(val, true, out personalityType e)) matchPersonalityId = e;
                    }
                    break;
                case "potentialChangeType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) potentialChangeType = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) potentialChangeType = e;
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
                case "resultScriptKey":
                    resultScriptKey = val;
                    break;

                case "seId":
                    seId = val;
                    break;
                case "floatingText":
                    floatingText = val;
                    break;
            }
        }


        DataList.Add(new Event_ResultData(
        
            resultId, choiceId, scriptId, nextId, matchPersonalityId, potentialChangeType,
            potentialChangeValue, conditionChange, statusChange, reactionPortraitId, resultScriptKey,
            seId, floatingText
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
