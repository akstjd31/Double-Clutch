using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Halftime_ListDataReader", menuName = "Scriptable Object/Halftime_ListDataReader", order = int.MaxValue)]
public class Halftime_ListDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Halftime_ListData> DataList = new List<Halftime_ListData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int scriptId = 0;
        string desc = null;
        triggerCond triggerCond = default;
        int triggerValue = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "scriptId":
                    int.TryParse(val, out scriptId);
                    break;

                case "desc":
                    desc = val;
                    break;
                case "triggerCond":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) triggerCond = (triggerCond)eInt;
                        else if (Enum.TryParse(val, true, out triggerCond e)) triggerCond = e;
                    }
                    break;
                case "triggerValue":
                    int.TryParse(val, out triggerValue);
                    break;
            }
        }
        DataList.Add(new Halftime_ListData(
            scriptId, desc, triggerCond, triggerValue
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

