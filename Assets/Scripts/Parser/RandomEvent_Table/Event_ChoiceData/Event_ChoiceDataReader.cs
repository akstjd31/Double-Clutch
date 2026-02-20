using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Event_ChoiceDataReader", menuName = "Scriptable Object/Event_ChoiceDataReader", order = int.MaxValue)]
public class Event_ChoiceDataReader : DataReaderBase
{
    [SerializeField] public List<Event_ChoiceData> DataList = new List<Event_ChoiceData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int index = 0;
        string scriptId = null;
        int currentId = 0;
        int nextId = 0;
        string textKey = null;

        textType textType = default;
        string standingRight = null;
        string standingLeft = null;
        string background = null;
        string cg = null;

        string choice01 = null;
        int nextId01 = 0;
        string choice02 = null;
        int nextId02 = 0;
        string choice03 = null;

        int nextId03 = 0;
        string bgm = null;
        string sfx = null;
        string buttonEffectId = null;
        string hoverSeId = null;

        float textSpeed = 0f;
        string cameraEffectId = null;
        string startEffectId = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";
            
            switch (col)
            {
                case "index":
                    int.TryParse(val, out index);
                    break;
                case "scriptId":
                    scriptId = val;
                    break;
                case "currentId":
                    int.TryParse(val, out currentId);
                    break;
                case "nextId":
                    int.TryParse(val, out nextId);
                    break;
                case "textKey":
                    textKey = val;
                    break;


                case "textType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) textType = (textType)eInt;
                        else if (Enum.TryParse(val, true, out textType e)) textType = e;
                    }
                    break;
                case "standingRight":
                    standingRight = val;
                    break;
                case "standingLeft":
                    standingLeft = val;
                    break;
                case "background":
                    background = val;
                    break;
                case "cg":
                    cg = val;
                    break;

                case "choice01":
                    choice01 = val;
                    break;
                case "nextId01":
                    int.TryParse(val, out nextId01);
                    break;
                case "choice02":
                    choice02 = val;
                    break;
                case "nextId02":
                    int.TryParse(val, out nextId02);
                    break;
                case "choice03":
                    choice03 = val;
                    break;

                case "nextId03":
                    int.TryParse(val, out nextId03);
                    break;
                case "bgm":
                    bgm = val;
                    break;
                case "sfx":
                    sfx = val;
                    break;
                case "buttonEffectId":
                    buttonEffectId = val;
                    break;
                case "hoverSeId":
                    hoverSeId = val;
                    break;

                case "textSpeed":
                    float.TryParse(val, out textSpeed);
                    break;
                case "cameraEffectId":
                    cameraEffectId = val;
                    break;
                case "startEffectId":
                    startEffectId = val;
                    break;
            }
        }


        if (index <= 0) return;
        
        var synergyData = new Event_ChoiceData
        (
            index, scriptId, currentId, nextId, textKey,
            textType, standingRight, standingLeft, background, cg,
            choice01, nextId01, choice02, nextId02, choice03,
            nextId03, bgm, sfx, buttonEffectId, hoverSeId,
            textSpeed, cameraEffectId, startEffectId
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
