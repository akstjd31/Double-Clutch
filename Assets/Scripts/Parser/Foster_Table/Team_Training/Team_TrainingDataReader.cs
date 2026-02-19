using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Team_TrainingDataReader", menuName = "Scriptable Object/Team_TrainingDataReader", order = int.MaxValue)]
public class Team_TrainingDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Team_TrainingData> DataList = new List<Team_TrainingData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string teamTrainingId = "";
        string desc = "";
        int trainingMode = 0;
        int allGain = 0;
        int mainGain = 0;
        int subGain = 0;
        int trainingCost = 0;
        int conditionCostMin = 0;
        int conditionCostMax = 0;
        string trainingNameKey = "";
        string trainingdescKey = "";
        string icon = "";



        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "teamTrainingId":
                    teamTrainingId = val;
                    break;

                case "desc":
                    desc = val;
                    break;
                
                case "trainingMode":
                    int.TryParse(val, out trainingMode);
                    break;

                case "allGain":
                    int.TryParse(val, out allGain);
                    break;

                case "mainGain":
                    int.TryParse(val, out mainGain);
                    break;

                case "subGain":
                    int.TryParse(val, out subGain);
                    break;

                case "trainingCost":
                    int.TryParse(val, out trainingCost);
                    break;

                case "conditionCostMin":
                    int.TryParse(val, out conditionCostMin);
                    break;

                case "conditionCostMax":
                    int.TryParse(val, out conditionCostMax);
                    break;

                case "trainingNameKey":
                    trainingNameKey = val;
                    break;

                case "trainingdescKey":
                    trainingdescKey = val;
                    break;

                case "icon":
                    icon = val;
                    break;

            }
        }

        // trainingId 없으면 스킵 (타입행/빈행 방지)
        if (string.IsNullOrEmpty(teamTrainingId)) return;

        DataList.Add(new Team_TrainingData(teamTrainingId, desc, trainingMode, allGain, mainGain, subGain,  trainingCost, conditionCostMin, conditionCostMax, trainingNameKey, trainingdescKey, icon));
    }
}
