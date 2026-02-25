using GoogleSheetsToUnity;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/Infra_DataReader", order = int.MaxValue)]
public class Infra_DataReader : DataReaderBase
{
    [SerializeField] public List<Infra_Data> DataList = new List<Infra_Data>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int idx = 0, group = 0, infraLv = 0, infraCost = 0, infraEV = 0;
        string desc = null, infraNK = null, infraDK = null, icon = null;
        infraEffectType infraET = default;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "index":
                    int.TryParse(val, out idx);
                    break;

                case "group":
                    int.TryParse(val, out group);
                    break;

                case "desc":
                    desc = val;
                    break;

                case "infraLevel":
                    int.TryParse(val, out infraLv);
                    break;

                case "infraEffectType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) infraET = (infraEffectType)eInt;
                        else if (Enum.TryParse(val, true, out infraEffectType e)) infraET = e;
                    }
                    break;

                case "infraEffectValue":
                    int.TryParse(val, out infraEV);
                    break;

                case "infraCost":
                    int.TryParse(val, out infraCost);
                    break;

                case "infraNameKey":
                    infraNK = val;
                    break;

                case "infraDescKey":
                    infraDK = val;
                    break;

                case "Icon":
                    icon = val;
                    break;
            }
        }

        var infraData = new Infra_Data
        (
            idx, group, desc, infraLv, infraET, infraEV, infraCost, infraNK, infraDK, icon
        );

        DataList.Add(infraData);
    }
}