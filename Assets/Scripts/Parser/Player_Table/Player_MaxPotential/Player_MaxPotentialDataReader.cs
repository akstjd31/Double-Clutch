using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_MaxPotentialDataReader", menuName = "Scriptable Object/Player_MaxPotentialDataReader", order = int.MaxValue)]
public class Player_MaxPotentialDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_MaxPotentialData> DataList = new List<Player_MaxPotentialData>();

    // ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int key = 0;
        int minPotentialValue = 0;
        int maxPotentialValue = 0;
        

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
                case "minPotentialValue":
                    int.TryParse(val, out minPotentialValue);
                    break;
                case "maxPotentialValue":
                    int.TryParse(val, out maxPotentialValue);
                    break;

            }
        }

        // skillId 없으면 스킵 (타입행/빈행 방지)
        if (key <= 0) return;

        DataList.Add(new Player_MaxPotentialData(key, minPotentialValue, maxPotentialValue));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
