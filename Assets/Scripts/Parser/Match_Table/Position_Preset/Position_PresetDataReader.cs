using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Position_PresetDataReader", menuName = "Scriptable Object/Position_PresetDataReader", order = int.MaxValue)]
public class Position_PresetDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Position_PresetData> DataList = new List<Position_PresetData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int presetId = 0;
        positionType positionType = default;
        changeType changeType = default;
        float offenseXMin = 0, offenseXMax = 0, offenseYMin = 0, offenseYMax = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "presetId":
                    int.TryParse(val, out presetId);
                    break;
                case "positionType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) positionType = (positionType)eInt;
                        else if (Enum.TryParse(val, true, out positionType e)) positionType = e;
                    }
                    break;
                case "changeType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) changeType = (changeType)eInt;
                        else if (Enum.TryParse(val, true, out changeType e)) changeType = e;
                    }
                    break;
                case "offenseXMin":
                    float.TryParse(val, out offenseXMin);
                    break;
                case "offenseXMax":
                    float.TryParse(val, out offenseXMax);
                    break;
                case "offenseYMin":
                    float.TryParse(val, out offenseYMin);
                    break;
                case "offenseYMax":
                    float.TryParse(val, out offenseYMax);
                    break;
            }
        }

        DataList.Add(new Position_PresetData(presetId,positionType,changeType,offenseXMin,offenseXMax,offenseYMin,offenseYMax
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
