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
        int skillId = 0;
        string skillName = "";
        skillCategory category = default;
        string triggerType = "";
        int triggerValue = 0;
        //effefctType effectType = default;
        float effectValue = 0;
        int effectDuration = 0;
        int CoolTime = 0;
        string passiveDesc = "";

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "skillId":
                    int.TryParse(val, out skillId);
                    break;

                case "skillName":
                    skillName = val;
                    break;

                case "skillCategory":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) category = (skillCategory)eInt;
                        else if (Enum.TryParse(val, true, out skillCategory e)) category = e;
                    }
                    break;

                case "triggerType":
                    triggerType = val;
                    break;

                case "triggerValue":
                    int.TryParse(val, out triggerValue);
                    break;

                case "effectValue":
                    float.TryParse(val, out effectValue);
                    break;

                case "effectDuration":
                    int.TryParse(val, out effectDuration);
                    break;

                case "CoolTime":
                    int.TryParse(val, out CoolTime);
                    break;

                case "passiveDesc":
                    passiveDesc = val;
                    break;

            }
        }

        // skillId 없으면 스킵 (타입행/빈행 방지)
        if (skillId <= 0) return;

        DataList.Add(new Player_PassiveData(
            skillId, skillName, category, triggerType, triggerValue, //effefctType,
            effectValue, effectDuration, CoolTime, passiveDesc
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
