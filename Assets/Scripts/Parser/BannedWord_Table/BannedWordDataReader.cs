using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BannedWordDataReader", menuName = "Scriptable Object/BannedWordDataReader", order = int.MaxValue)]
public class BannedWordDataReader : DataReaderBase
{
    [SerializeField] public List<string> WordData = new List<string>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            WordData.Add(val);
        }
    }
}