using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "League_MasterDataReader", menuName = "Scriptable Object/League_MasterDataReader", order = int.MaxValue)]
public class League_MasterDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<League_MasterData> DataList = new List<League_MasterData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string leagueId = null, desc = null, leagueName = null;
        leagueType leagueType = default;
        int leagueTeamCount = 0, outConditionValue = 0;
        bool isSelectionRequired = false;
        string teamSelectionRuleId = null, leagueLevelId = null, leagueRewardId = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "leagueId":
                    leagueId = val;
                    break;

                case "desc":
                    desc = val;
                    break;
                case "leagueName":
                    leagueName = val;
                    break;

                case "leagueType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) leagueType = (leagueType)eInt;
                        else if (Enum.TryParse(val, true, out leagueType e)) leagueType = e;
                    }
                    break;
                case "leagueTeamCount":
                    int.TryParse(val, out leagueTeamCount);
                    break;

                case "outConditionValue":
                    int.TryParse(val, out outConditionValue);
                    break;
                case "isSelectionRequired":
                    isSelectionRequired = ParseBool(val);
                    break;
                case "teamSelectionRuleId":
                    teamSelectionRuleId = val;
                    break;

                case "leagueLevelId":
                    leagueLevelId = val;
                    break;

                case "leagueRewardId":
                    leagueRewardId = val;
                    break;
            }
        }
        DataList.Add(new League_MasterData(
            leagueId, desc, leagueName, leagueType, leagueTeamCount, outConditionValue, isSelectionRequired, teamSelectionRuleId, leagueLevelId, leagueRewardId
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

