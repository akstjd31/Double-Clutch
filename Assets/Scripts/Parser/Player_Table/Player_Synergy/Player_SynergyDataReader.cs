using GoogleSheetsToUnity;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/Player_SynergyDataReader", order = int.MaxValue)]
public class Player_SynergyDataReader : DataReaderBase
{
    [SerializeField] public List<PlayerSynergyData> DataList = new List<PlayerSynergyData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
<<<<<<< Updated upstream
        int synergyId = 0, traitID1 = 0, traitID2 = 0;
        string synergyName = null;
=======
        string synergyId = null, traitID1 = null, traitID2 = null;
        string synergyName = null, synergyDesc = null;
>>>>>>> Stashed changes
        effectType eType = default;
        float eValue = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";
            
            switch (col)
            {
                case "synergyId":
                    synergyId = val;
                    break;

                case "traitID1":
                    traitID1 = val;
                    break;

                case "traitID2":
                    traitID2 = val;
                    break;

                case "synergyName":
                    synergyName = val;
                    break;
                
                case "effectType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (int.TryParse(val, out var eInt)) eType = (effectType)eInt;
                        else if (Enum.TryParse(val, true, out effectType e)) eType = e;
                    }

                    break;
                
                case "effectValue":
                    float.TryParse(val, out eValue);
                    break;
            }
        }

        var synergyData = new PlayerSynergyData
        (
            synergyId, traitID1, traitID2, synergyName, eType, eValue
        );

        DataList.Add(synergyData);
    }
}
