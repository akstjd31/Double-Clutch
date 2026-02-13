using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_ReputationDataReader", menuName = "Scriptable Object/Player_ReputaionDataReader", order = int.MaxValue)]
public class Player_ReputationDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_ReputationData> DataList = new List<Player_ReputationData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int key = 0, minReputationScore = 0, minStepValue = 0, maxReputationScore = 0, maxStepValue = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "key":
                    int.TryParse(val, out key);
                    break;
                case "minReputationScore":
                    int.TryParse(val, out minReputationScore);
                    break;
                case "minStepValue":
                    int.TryParse(val, out minStepValue);
                    break;
                case "maxReputationScore":
                    int.TryParse(val, out maxReputationScore);
                    break;
                case "maxStepValue":
                    int.TryParse(val, out maxStepValue);
                    break;
            }
        }

        // ✅ weekId가 없으면 스킵 (타입행/빈행 방지)
        if (key <= 0) return;

        DataList.Add(new Player_ReputationData(key, minReputationScore, minStepValue, maxReputationScore, maxStepValue
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
