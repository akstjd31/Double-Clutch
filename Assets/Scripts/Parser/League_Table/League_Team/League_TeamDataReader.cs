using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "League_TeamDataReader", menuName = "Scriptable Object/League_TeamDataReader", order = int.MaxValue)]
public class League_TeamDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<League_TeamData> DataList = new List<League_TeamData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string teamSelectionRuleId = null, desc = null;
        int weekId = 0, priorityTeamCount = 0, selectionCountTotal = 0, leagueTeamTotal = 0, selectionCountD = 0, selectionCountC = 0, selectionCountB = 0, selectionCountA = 0;
        int selectionCountS = 0, selectionCountSS = 0, selectionCountSSS = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "teamSelectionRuleId":
                    teamSelectionRuleId = val;
                    break;

                case "desc":
                    desc = val;
                    break;

                case "weekId":
                    int.TryParse(val, out weekId);
                    break;

                case "priorityTeamCount":
                    int.TryParse(val, out priorityTeamCount);
                    break;

                case "selectionCountTotal":
                    int.TryParse(val, out selectionCountTotal);
                    break;
                case "leagueTeamTotal":
                    int.TryParse(val, out leagueTeamTotal);
                    break;
                case "selectionCountD":
                    int.TryParse(val, out selectionCountD);
                    break;
                case "selectionCountC":
                    int.TryParse(val, out selectionCountC);
                    break;
                case "selectionCountB":
                    int.TryParse(val, out selectionCountB);
                    break;
                case "selectionCountA":
                    int.TryParse(val, out selectionCountA);
                    break;
                case "selectionCountS":
                    int.TryParse(val, out selectionCountS);
                    break;
                case "selectionCountSS":
                    int.TryParse(val, out selectionCountSS);
                    break;
                case "selectionCountSSS":
                    int.TryParse(val, out selectionCountSSS);
                    break;
            }
        }
        DataList.Add(new League_TeamData(
            teamSelectionRuleId, desc, weekId, 
            priorityTeamCount,selectionCountTotal,
            leagueTeamTotal,selectionCountD,selectionCountC,
            selectionCountB,selectionCountA,selectionCountS,
            selectionCountSS,selectionCountSSS
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

