using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/Calendar_TableDataReader", order = int.MaxValue)]
public class Calendar_TableDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Calendar_TableData> DataList = new List<Calendar_TableData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int weekId = 0, month = 0, weekNo = 0, targetIdDefault = 0, targetIdSpecial = 0;
        string desc = null;
        phaseType phase = default;
        bool isSpecialWeek = false, hasSeasonOut = false;
        string leagueId = null, startCutscene = null, endCutscene = null, tutorialId = null, backgroundImageId = null, backgroundMusicId = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "weekId":
                    int.TryParse(val, out weekId);
                    break;

                case "desc":
                    desc = val;
                    break;

                case "month":
                    int.TryParse(val, out month);
                    break;

                case "weekNo":
                    int.TryParse(val, out weekNo);
                    break;

                case "phaseType":
                    // "Event", "League", "Training" 같은 문자열이 들어오는 형태
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) phase = (phaseType)eInt;
                        else if (Enum.TryParse(val, true, out phaseType e)) phase = e;
                    }
                    break;

                case "isSpecialWeek":
                    isSpecialWeek = ParseBool(val);
                    break;

                case "hasSeasonOut":
                    hasSeasonOut = ParseBool(val);
                    break;

                case "targetIdDefault":
                    int.TryParse(val, out targetIdDefault);
                    break;

                case "targetIdSpecial":
                    int.TryParse(val, out targetIdSpecial);
                    break;

                case "leagueId":
                    leagueId = val;
                    break;

                case "startCutscene":
                    startCutscene = val;
                    break;

                case "endCutscene":
                    endCutscene = val;
                    break;

                case "TutorialId":
                    tutorialId = val;
                    break;

                case "backgroundImageId":
                    backgroundImageId = val;
                    break;

                case "backgroundMusicId":
                    backgroundMusicId = val;
                    break;
            }
        }

        // ✅ weekId가 없으면 스킵 (타입행/빈행 방지)
        if (weekId <= 0) return;

        DataList.Add(new Calendar_TableData(
            weekId, desc, month, weekNo, phase,
            isSpecialWeek, hasSeasonOut, targetIdDefault, targetIdSpecial,
            leagueId, startCutscene, endCutscene, tutorialId,
            backgroundImageId, backgroundMusicId
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
