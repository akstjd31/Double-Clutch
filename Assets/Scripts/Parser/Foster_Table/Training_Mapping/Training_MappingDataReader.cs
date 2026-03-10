using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Training_MappingDataReader", menuName = "Scriptable Object/Training_MappingDataReader", order = int.MaxValue)]
public class Training_MappingDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Training_MappingData> DataList = new List<Training_MappingData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        potential potential = default;
        int category = 0;
        string desc = null, categorydescKey = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "Potential":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) potential = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) potential = e;
                    }
                    break;
                case "category":
                    int.TryParse(val, out category);
                    break;

                case "desc":
                    desc = val;
                    break;
                case "categorydescKey":
                    categorydescKey = val;
                    break;
                
            }
        }
        DataList.Add(new Training_MappingData(
            potential, category,desc,categorydescKey
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
