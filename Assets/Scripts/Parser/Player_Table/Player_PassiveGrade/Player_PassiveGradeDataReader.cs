using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_PassiveGradeDataReader", menuName = "Scriptable Object/Player_PassiveGradeDataReader", order = int.MaxValue)]
public class Player_PassiveGradeDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_PassiveGradeData> DataList = new List<Player_PassiveGradeData>();

    
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int gradeId = 0;        
        string skillName = string.Empty;
        float spawnRate = 0;
        string passiveFrameResource = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "gradeId":
                    int.TryParse(val, out gradeId);
                    break;

                case "skillName":
                    skillName = val;
                    break;

                case "spawnRate":
                    float.TryParse(val, out spawnRate);
                    break;
                case "passiveFrameResource":
                    passiveFrameResource = val;
                    break;
            }
        }

        DataList.Add(new Player_PassiveGradeData(gradeId, skillName, spawnRate, passiveFrameResource));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
