using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_NameDataReader", menuName = "Scriptable Object/Player_NameDataReader", order = int.MaxValue)]
public class Player_NameDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_NameData> DataList = new List<Player_NameData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int ID = 0;
        nation nation = default;
        namePart namePart = default;
        string nameKey = null;
        string desc = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "ID":
                    int.TryParse(val, out ID);
                    break;
                case "nation":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) nation = (nation)eInt;
                        else if (Enum.TryParse(val, true, out nation e)) nation = e;
                    }
                    break;
                case "namePart":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) namePart = (namePart)eInt;
                        else if (Enum.TryParse(val, true, out namePart e)) namePart = e;
                    }
                    break;

                case "nameKey":
                    nameKey = val;
                    break;
                case "desc":
                    desc = val;
                    break;
            }
        }

        // ✅ weekId가 없으면 스킵 (타입행/빈행 방지)
        if (ID <= 0) return;

        DataList.Add(new Player_NameData(ID, nation, namePart, nameKey, desc
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
