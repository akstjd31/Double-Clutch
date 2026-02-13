using GoogleSheetsToUnity;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/Player_StartingStateDataReader", order = int.MaxValue)]
public class Player_StartingStateDataReader : DataReaderBase
{
    [SerializeField] public List<Player_StartingStateData> DataList = new List<Player_StartingStateData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int grade = 0, startMin = 0, startMax = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId?.Trim().ToLowerInvariant();
            string val = list[i].value ?? "";

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            val = val.Trim().Replace(",", "");

            int toInt = 0;
            if (!string.IsNullOrEmpty(val))
            {
                if (int.TryParse(val, out var iv)) toInt = iv;
                else if (float.TryParse(val, out var fv)) toInt = Mathf.RoundToInt(fv);
            }

            switch (col)
            {
                case "grade":
                    grade = toInt;
                    break;

                case "startmin":
                    startMin = toInt;
                    break;

                case "startmax":
                    startMax = toInt;
                    break;
            }
        }
        
        if (grade == 0 || startMin == 0 || startMax == 0)
        {
            Debug.LogWarning($"Row {rowIndex} skipped (has zero) -> G:{grade}, Min:{startMin}, Max:{startMax}");
            return;
        }

        DataList.Add(new Player_StartingStateData(grade, startMin, startMax));
        Debug.Log($"Row {rowIndex} added -> G:{grade}, Min:{startMin}, Max:{startMax}");
    }

}