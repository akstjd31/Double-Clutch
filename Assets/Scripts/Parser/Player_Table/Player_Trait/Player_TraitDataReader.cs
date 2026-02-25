using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTraitDataReader", menuName = "Scriptable Object/PlayerTraitDataReader", order = int.MaxValue)]
public class Player_TraitDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_TraitData> DataList = new List<Player_TraitData>();

    // ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string traitId = null;
        string traitName = "";
        string desc = "";

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "traitId":
                    traitId = val;
                    break;

                case "traitName":
                    traitName = val;
                    break;

                case "desc":
                    desc = val;
                    break;

               
            }
        }

        DataList.Add(new Player_TraitData(
            traitId, traitName, desc
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
