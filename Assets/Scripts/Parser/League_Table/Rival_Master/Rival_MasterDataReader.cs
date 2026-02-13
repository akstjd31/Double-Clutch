using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rival_MasterDataReader", menuName = "Scriptable Object/Rival_MasterDataReader", order = int.MaxValue)]
public class Rival_MasterDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Rival_MasterData> DataList = new List<Rival_MasterData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string teamId = null, desc = null, teamName = null;
        teamSector teamSector = default;
        string teamArchetypeId = null;
        teamTier teamTier = default;
        int minAndroidCount = 0, minHumanCount = 0, minAnimalCount = 0, minCountSum = 0, weightAndroid = 0, weightHuman = 0, weightAnimal = 0, weightSum = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "teamId":
                    teamId = val;
                    break;

                case "desc":
                    desc = val;
                    break;
                case "teamName":
                    teamName = val;
                    break;
                case "teamSector":
                    // "Event", "League", "Training" 같은 문자열이 들어오는 형태
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) teamSector = (teamSector)eInt;
                        else if (Enum.TryParse(val, true, out teamSector e)) teamSector = e;
                    }
                    break;
                case "teamArchetypeId":
                    teamArchetypeId = val;
                    break;
                case "teamTier":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) teamTier = (teamTier)eInt;
                        else if (Enum.TryParse(val, true, out teamTier e)) teamTier = e;
                    }
                    break;
                case "minAndroidCount":
                    int.TryParse(val, out minAndroidCount);
                    break;

                case "minHumanCount":
                    int.TryParse(val, out minHumanCount);
                    break;
                case "minAnimalCount":
                    int.TryParse(val, out minAnimalCount);
                    break;
                case "minCountSum":
                    int.TryParse(val, out minCountSum);
                    break;
                case "weightAndroid":
                    int.TryParse(val, out weightAndroid);
                    break;
                case "weightHuman":
                    int.TryParse(val, out weightHuman);
                    break;
                case "weightAnimal":
                    int.TryParse(val, out weightAnimal);
                    break;
                case "weightSum":
                    int.TryParse(val, out weightSum);
                    break;
            }
        }
        DataList.Add(new Rival_MasterData(
            teamId,desc,teamName,teamSector,
            teamArchetypeId,teamTier,minAndroidCount,
            minHumanCount,minAnimalCount,minCountSum,
            weightAndroid,weightHuman,weightAnimal,weightSum
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

