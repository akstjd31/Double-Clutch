using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mercenary_DataReader", menuName = "Scriptable Object/Mercenary_DataReader", order = int.MaxValue)]
public class Mercenary_DataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Mercenary_Data> DataList = new List<Mercenary_Data>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int playerUId = 0;
        positionType positionType = default;
        string mercName = null;
        int traitId = 0, passiveId1 = 0, passiveId2 = 0, passiveId3 = 0, stat2ptValue = 0, stat3ptValue = 0, statAssistValue = 0, statBlockValue = 0, statStealValue = 0, statReboundValue = 0, currentCondition = 0;
        handicapState handicapState = default;
        string playerImageResource = null, portraitResource = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "playerUId":
                    int.TryParse(val, out playerUId);
                    break;
                case "positionType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) positionType = (positionType)eInt;
                        else if (Enum.TryParse(val, true, out positionType e)) positionType = e;
                    }
                    break;
                case "mercName":
                    mercName = val;
                    break;
                case "traitId":
                    int.TryParse(val, out traitId);
                    break;
                case "passiveId1":
                    int.TryParse(val, out passiveId1);
                    break;
                case "passiveId2":
                    int.TryParse(val, out passiveId2);
                    break;
                case "passiveId3":
                    int.TryParse(val, out passiveId3);
                    break;
                case "stat2ptValue":
                    int.TryParse(val, out stat2ptValue);
                    break;
                case "stat3ptValue":
                    int.TryParse(val, out stat3ptValue);
                    break;
                case "statAssistValue":
                    int.TryParse(val, out statAssistValue);
                    break;
                case "statBlockValue":
                    int.TryParse(val, out statBlockValue);
                    break;
                case "statStealValue":
                    int.TryParse(val, out statStealValue);
                    break;
                case "statReboundValue":
                    int.TryParse(val, out statReboundValue);
                    break;
                case "currentCondition":
                    int.TryParse(val, out currentCondition);
                    break;
                case "handicapState":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) handicapState = (handicapState)eInt;
                        else if (Enum.TryParse(val, true, out handicapState e)) handicapState = e;
                    }
                    break;
                case "playerImageResource":
                    playerImageResource = val;
                    break;
                case "portraitResource":
                    portraitResource = val;
                    break;
            }
        }
        DataList.Add(new Mercenary_Data(
            playerUId,positionType,mercName,traitId,passiveId1,
            passiveId2,passiveId3,stat2ptValue,stat3ptValue,
            statAssistValue,statBlockValue,statStealValue,
            statReboundValue,currentCondition,handicapState,
            playerImageResource,portraitResource
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

