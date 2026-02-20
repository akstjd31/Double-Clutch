using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Individual_RestDataReader", menuName = "Scriptable Object/Individual_RestDataReader", order = int.MaxValue)]
public class Individual_RestDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Individual_RestData> DataList = new List<Individual_RestData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string restId = "";
        string desc = "";
        int conditionRecovery = 0;
        int isCureInjury = 0;
        int cureOverworkProb = 0;
        int restCost = 0;
        string restNameKey = "";
        string restdescKey = "";
        string icon = "";



        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "restId":
                    restId = val;
                    break;

                case "desc":
                    desc = val;
                    break;

                case "conditionRecovery":
                    int.TryParse(val, out conditionRecovery);
                    break;

                case "isCureInjury":
                    int.TryParse(val, out isCureInjury);
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

        // trainingId 없으면 스킵 (타입행/빈행 방지)
        if (string.IsNullOrEmpty(restId)) return;

        DataList.Add(new Individual_RestData(restId, desc, conditionRecovery, isCureInjury, cureOverworkProb, restCost, restNameKey, restdescKey, icon));
    }
}

