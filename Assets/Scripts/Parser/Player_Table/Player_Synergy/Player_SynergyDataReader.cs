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
        int synergyId = 0, traitID1 = 0, traitID2 = 0;
        string synergyName = null;
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
                    int.TryParse(val, out synergyId);
                    break;

                case "traitID1":
                    int.TryParse(val, out traitID1);
                    break;

                case "traitID2":
                    int.TryParse(val, out traitID2);
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

        if (synergyId <= 0) return;
        
        var synergyData = new PlayerSynergyData
        (
            synergyId, traitID1, traitID2, synergyName, eType, eValue
        );

        DataList.Add(synergyData);
    }
}
