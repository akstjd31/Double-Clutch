using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_PersonalityDataReader", menuName = "Scriptable Object/Player_PersonalityDataReader", order = int.MaxValue)]
public class Player_PersonalityDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_PersonalityData> DataList = new List<Player_PersonalityData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int personalityId = 0;
        coreType core = default;
        personalityType personality = default;
        string personalityName = null;
        string desc = null;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "personalityId":
                    int.TryParse(val, out personalityId);
                    break;
                case "coreType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) core = (coreType)eInt;
                        else if (Enum.TryParse(val, true, out coreType e)) core = e;
                    }
                    break;
                case "personalityType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) personality = (personalityType)eInt;
                        else if (Enum.TryParse(val, true, out personalityType e)) personality = e;
                    }
                    break;

                case "personalityName":
                    personalityName = val;
                    break;
                case "desc":
                    desc = val;
                    break;
            }
        }

        // ✅ weekId가 없으면 스킵 (타입행/빈행 방지)
        if (personalityId <= 0) return;

        DataList.Add(new Player_PersonalityData(personalityId , core, personality, personalityName,desc
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
