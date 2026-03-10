using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BannedWordDataReader", menuName = "Scriptable Object/BannedWordDataReader", order = int.MaxValue)]
public class BannedWordDataReader : DataReaderBase
{
    [SerializeField] public List<BannedWordData> DataList = new List<BannedWordData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string[] wordData = new string[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            wordData[i] = val;
        }

        DataList.Add(new BannedWordData(wordData));
    }
}