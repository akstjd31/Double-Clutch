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
        potential state1 = default, state2 = default, state3 = default;
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

                case "state1":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) state1 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) state1 = e;
                    }
                    break;
                case "recommendation1":
                    int.TryParse(val, out recommendation1 );
                    break;

                case "state2":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) state2 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) state2 = e;
                    }
                    break;
                case "recommendation2":
                    int.TryParse(val, out recommendation2);
                    break;
                case "state3":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) state3 = (potential)eInt;
                        else if (Enum.TryParse(val, true, out potential e)) state3 = e;
                    }
                    break;
                case "recommendation3":
                    int.TryParse(val, out recommendation3);
                    break;
            }
        }

        var positionData = new Player_PositionData
        (
            recommendId,state1,recommendation1,state2,recommendation2,state3,recommendation3
        );

        DataList.Add(positionData);
    }
}
