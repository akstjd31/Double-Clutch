using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Halftime_ScriptDataReader", menuName = "Scriptable Object/Halftime_ScriptDataReader", order = int.MaxValue)]
public class Halftime_ScriptDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Halftime_ScriptData> DataList = new List<Halftime_ScriptData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int index = 0, scriptId = 0, currentId = 0, nextId = 0;
        string textKey = null;
        textType textType = default;
        string standingRight = null, standingLeft = null, background = null, cg = null, choice01 = null;
        potential choiceStat01 = default;
        positionType choicePosition01 = default;   
        float changeStat01 = 0.0f;
        changeType changePosition01 = default;
        int nextId01 = 0;
        string choice02 = null;
        potential choiceStat02 = default;
        positionType choicePosition02 = default;
        float changeStat02 = 0.0f;
        changeType changePosition02 = default;
        int nextId02 = 0;
        string choice03 = null;
        potential choiceStat03 = default;
        positionType choicePosition03 = default;
        float changeStat03 = 0.0f;
        changeType changePosition03 = default;
        int nextId03 = 0;
        string bgm = null, sfx = null, buttonEffectId = null, hoverSeId = null;
        float textSpeed = 0.0f;
        string cameraEffectId = null, startEffectId = null;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "index":
                    int.TryParse(val, out index);
                    break;

                case "scriptId":
                    int.TryParse(val, out scriptId);
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
                case "choiceStat01":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) choiceStat01 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) choiceStat01 = e;
                    }
                    break;
                case "choicePosition01":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) choicePosition01 = (positionType)eInt;
                        else if (Enum.TryParse(val, true, out positionType e)) choicePosition01 = e;
                    }
                    break;
                case "changeStat01":
                    float.TryParse(val, out changeStat01);
                    break;
                case "changePosition01":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) changePosition01 = (changeType)eInt;
                        else if (Enum.TryParse(val, true, out changeType e)) changePosition01 = e;
                    }
                    break;
                case "nextId01":
                    int.TryParse(val, out nextId01);
                    break;
                case "choice02":
                    choice02 = val;
                    break;
                case "choiceStat02":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) choiceStat02 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) choiceStat02 = e;
                    }
                    break;
                case "choicePosition02":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) choicePosition02 = (positionType)eInt;
                        else if (Enum.TryParse(val, true, out positionType e)) choicePosition02 = e;
                    }
                    break;
                case "changeStat02":
                    float.TryParse(val, out changeStat02);
                    break;
                case "changePosition02":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) changePosition02 = (changeType)eInt;
                        else if (Enum.TryParse(val, true, out changeType e)) changePosition02 = e;
                    }
                    break;
                case "nextId02":
                    int.TryParse(val, out nextId02);
                    break;
                case "choice03":
                    choice03 = val;
                    break;
                case "choiceStat03":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) choiceStat03 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) choiceStat03 = e;
                    }
                    break;
                case "choicePosition03":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) choicePosition03 = (positionType)eInt;
                        else if (Enum.TryParse(val, true, out positionType e)) choicePosition03 = e;
                    }
                    break;
                case "changeStat03":
                    float.TryParse(val, out changeStat03);
                    break;
                case "changePosition03":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) changePosition03 = (changeType)eInt;
                        else if (Enum.TryParse(val, true, out changeType e)) changePosition03 = e;
                    }
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
                    float.TryParse (val, out textSpeed);
                    break;
                case "cameraEffectId":
                    cameraEffectId = val;
                    break;
                case "startEffectId":
                    startEffectId = val;
                    break;
            }
        }

        DataList.Add(new Halftime_ScriptData(index, scriptId, currentId,nextId,textKey,
            textType,standingRight,standingLeft,background,cg,
            choice01,choiceStat01,choicePosition01,changeStat01,changePosition01,nextId01,
            choice02,choiceStat02,choicePosition02,changeStat02,changePosition02,nextId02,
            choice03,choiceStat03,choicePosition03,changeStat03,changePosition03,nextId03,
            bgm,sfx,buttonEffectId,hoverSeId,textSpeed,cameraEffectId,startEffectId
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

