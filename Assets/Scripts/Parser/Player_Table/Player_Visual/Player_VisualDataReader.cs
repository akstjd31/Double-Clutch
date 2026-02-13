using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_VisualDataReader", menuName = "Scriptable Object/Player_VisualDataReader", order = int.MaxValue)]
public class Player_VisualDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_VisualData> DataList = new List<Player_VisualData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int visualId = 0;
        int speciesId = 0;
        string assetKey = null;
        string desc = null;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "visualId":
                    int.TryParse(val, out visualId);
                    break;
                case "speciesId":
                    int.TryParse(val, out speciesId);
                    break;
                case "assetKey":
                    assetKey = val;
                    break;
                case "desc":
                    desc = val;
                    break;
            }
        }

        // ✅ weekId가 없으면 스킵 (타입행/빈행 방지)
        if (visualId <= 0) return;

        DataList.Add(new Player_VisualData(visualId, speciesId, assetKey, desc
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
