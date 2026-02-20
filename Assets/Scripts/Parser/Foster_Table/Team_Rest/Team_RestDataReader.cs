using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Team_RestDataReader", menuName = "Scriptable Object/Team_RestDataReader", order = int.MaxValue)]
public class Team_RestDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Team_RestData> DataList = new List<Team_RestData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string teamRestId = string.Empty;
        string desc = string.Empty;
        int conditionRecovery = 0;
        int cureOverworkProb = 0;
        int restCost = 0;
        string restNameKey = string.Empty;
        string restdescKey = string.Empty;
        string icon = string.Empty;



        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "teamRestId":
                    teamRestId = val;
                    break;

                case "desc":
                    desc = val;
                    break;

                case "conditionRecovery":
                    int.TryParse(val, out conditionRecovery);
                    break;

                case "cureOverworkProb":
                    int.TryParse(val, out cureOverworkProb);
                    break;

                case "restCost":
                    int.TryParse(val, out restCost);
                    break;

                case "restNameKey":
                    restNameKey = val;
                    break;

                case "restdescKey":
                    restdescKey = val;
                    break;

                case "icon":
                    icon = val;
                    break;

            }
        }

        // teamRestId 없으면 스킵 (타입행/빈행 방지)
        if (string.IsNullOrEmpty(teamRestId)) return;

        DataList.Add(new Team_RestData(teamRestId, desc, conditionRecovery, cureOverworkProb, restCost, restNameKey, restdescKey, icon));
    }
}
