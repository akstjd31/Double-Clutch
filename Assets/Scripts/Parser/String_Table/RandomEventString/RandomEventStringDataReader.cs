using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomEventStringDataReader", menuName = "Scriptable Object/RandomEventStringDataReader", order = int.MaxValue)]
public class RandomEventStringDataReader : DataReaderBase
{
    [SerializeField] public List<RandomEventStringData> DataList = new List<RandomEventStringData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string stringKey = null;
        string ko = null, en = null, ja = null;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";
            
            switch (col)
            {
                case "stringKey":
                    stringKey = val;
                    break;

                case "ko":
                    ko = val;
                    break;

                case "en":
                    en = val;
                    break;
                case "ja":
                    ja = val;
                    break;

            }
        }
        
        var synergyData = new RandomEventStringData
        (
            stringKey, ko, en, ja
        );

        DataList.Add(synergyData);
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
