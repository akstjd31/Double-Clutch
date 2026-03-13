using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_PassiveDataReader", menuName = "Scriptable Object/Player_PassiveDataReader", order = int.MaxValue)]
public class Player_PassiveDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_PassiveData> DataList = new List<Player_PassiveData>();

    // ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string skillId = null;
        string skillName = "";
        int grade = 0;
        effectType effectType = default;
        float effectValue = 0;
        string passiveDesc = "";
        string passiveResource = "";

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "skillId":
                    skillId = val;
                    break;

                case "skillName":
                    skillName = val;
                    break;
                case "grade":
                    int.TryParse(val, out grade);
                    break;

                case "effectType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) effectType = (effectType)eInt;
                        else if (Enum.TryParse(val, true, out effectType e)) effectType = e;
                    }
                    break;

                case "effectValue":
                    float.TryParse(val, out effectValue);
                    break;

                case "passiveDesc":
                    passiveDesc = val;
                    break;
                case "passiveResource":
                    passiveResource = val;
                    break;

            }
        }

        // skillId 없으면 스킵 (타입행/빈행 방지)
        if (string.IsNullOrEmpty(skillId)) return;

        DataList.Add(new Player_PassiveData(
            skillId, skillName, grade, effectType,
            effectValue, passiveDesc, passiveResource
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
