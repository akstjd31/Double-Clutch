using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_GrowthRateDataReader", menuName = "Scriptable Object/Player_GrowthRateDataReader", order = int.MaxValue)]
public class Player_GrowthRateDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_GrowthRateData> DataList = new List<Player_GrowthRateData>();

    // ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int key = 0;
        int minGrowthRate = 0;
        int maxGrowthRate = 0;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "key":
                    int.TryParse(val, out key);
                    break;
                case "minGrowthRate":
                    int.TryParse(val, out minGrowthRate);
                    break;
                case "maxGrowthRate":
                    int.TryParse(val, out maxGrowthRate);
                    break;

            }
        }

        // skillId 없으면 스킵 (타입행/빈행 방지)
        if (key <= 0) return;

        DataList.Add(new Player_GrowthRateData(key, minGrowthRate, maxGrowthRate));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
