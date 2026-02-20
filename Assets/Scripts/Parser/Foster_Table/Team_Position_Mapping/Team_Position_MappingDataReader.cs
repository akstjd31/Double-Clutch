using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Team_Position_MappingDataReader", menuName = "Scriptable Object/Team_Position_MappingDataReader", order = int.MaxValue)]
public class Team_Position_MappingDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Team_Position_MappingData> DataList = new List<Team_Position_MappingData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        Position position = default;
        potential mainPotential = default;
        potential subPotential = default;



        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "position":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) position = (Position)eInt;
                        else if (Enum.TryParse(val, true, out Position e)) position = e;
                    }
                    break;

                case "mainPotential":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) mainPotential = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) mainPotential = e;
                    }
                    break;

                case "subPotential":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) subPotential = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) subPotential = e;
                    }
                    break;

            }
        }

        DataList.Add(new Team_Position_MappingData( position,  mainPotential,  subPotential));
    }
}
