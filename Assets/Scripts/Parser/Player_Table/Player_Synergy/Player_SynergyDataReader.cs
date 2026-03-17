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
        string synergyId = null, traitId1 = null, traitId2 = null;
        string synergyName = null, synergyDesc = null;
        effectType eType = default;
        float eValue = 0;
        string synergyResource = null;  

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

                case "traitId1":
                    traitId1 = val;
                    break;

                case "traitId2":
                    traitId2 = val;
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
                
                case "synergyDesc":
                    synergyDesc = val;
                    break;
                case "synergyResource":
                    synergyResource = val;
                    break;
            }
        }

        var synergyData = new PlayerSynergyData
        (
            synergyId, traitId1, traitId2, synergyName, eType, eValue, synergyDesc, synergyResource
        );

        DataList.Add(synergyData);
    }
}
