using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Individual_TrainingDataReader", menuName = "Scriptable Object/Individual_TrainingDataReader", order = int.MaxValue)]
public class Individual_TrainingDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Individual_TrainingData> DataList = new List<Individual_TrainingData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string trainingId = "";
        string desc = "";
        potential mainPotential = default;
        int mainGain = 0;
        potential subPotential = default;
        int subGain = 0;
        int conditionCostMin = 0;
        int conditionCostMax = 0;
        int trainingCost = 0;
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
                case "trainingId":
                    trainingId = val;
                    break;

                case "desc":
                    desc = val;
                    break;

                case "mainPotential":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) mainPotential = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) mainPotential = e;
                    }
                    break;

                case "mainGain":
                    int.TryParse(val, out mainGain);
                    break;

                case "subPotential":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) subPotential = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) subPotential = e;
                    }
                    break;

                case "subGain":
                    int.TryParse(val, out subGain);
                    break;

                case "conditionCostMin":
                    int.TryParse(val, out conditionCostMin);
                    break;

                case "conditionCostMax":
                    int.TryParse(val, out conditionCostMax);
                    break;                

                case "trainingCost":
                    int.TryParse(val, out trainingCost);
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
        if (string.IsNullOrEmpty(trainingId)) return;

        DataList.Add(new Individual_TrainingData(trainingId, desc, mainPotential, mainGain, subPotential, subGain, conditionCostMin, conditionCostMax, trainingCost, trainingNameKey, trainingdescKey, icon));
    }
}
