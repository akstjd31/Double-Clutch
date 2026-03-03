using GoogleSheetsToUnity;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Player_PositionDataReader", menuName = "Scriptable Object/Player_PositionDataReader", order = int.MaxValue)]
public class Player_PositionDataReader : DataReaderBase
{
    [SerializeField] public List<Player_PositionData> DataList = new List<Player_PositionData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string recommendId = null;
        potential stat1 = default, stat2 = default, stat3 = default;
        int recommendation1 = 0, recommendation2 = 0,recommendation3 = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "recommendId":
                    recommendId = val;
                    break;

                case "stat1":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) stat1 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) stat1 = e;
                    }
                    break;
                case "recommendation1":
                    int.TryParse(val, out recommendation1 );
                    break;

                case "stat2":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) stat2 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) stat2 = e;
                    }
                    break;
                case "recommendation2":
                    int.TryParse(val, out recommendation2);
                    break;
                case "stat3":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) stat3 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) stat3 = e;
                    }
                    break;
                case "recommendation3":
                    int.TryParse(val, out recommendation3);
                    break;
            }
        }

        var positionData = new Player_PositionData
        (
            recommendId,stat1,recommendation1,stat2,recommendation2,stat3,recommendation3
        );

        DataList.Add(positionData);
    }
}
