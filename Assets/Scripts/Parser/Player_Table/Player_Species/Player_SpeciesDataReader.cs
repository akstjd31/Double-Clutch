using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Player_SpeciesDataReader", menuName = "Scriptable Object/Player_SpeciesDataReader", order = int.MaxValue)]
public class Player_SpeciesDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_SpeciesData> DataList = new List<Player_SpeciesData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int speciesId = 0;
        speciesType species = default;
        string speciesName = null;
        string desc = null;
        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "speciesId":
                    int.TryParse(val, out speciesId);
                    break;
                case "speciesType":
                    if (!string.IsNullOrEmpty(val))
                    {
                        // 숫자(enum int)도 대응
                        if (int.TryParse(val, out var eInt)) species = (speciesType)eInt;
                        else if (Enum.TryParse(val, true, out speciesType e)) species = e;
                    }
                    break;
                case "spciesName":
                    desc = val;
                    break;
                case "desc":
                    desc = val;
                    break;
            }
        }

        // ✅ speciesId가 없으면 스킵 (타입행/빈행 방지)
        if (speciesId <= 0) return;

        DataList.Add(new Player_SpeciesData(
            speciesId,species,speciesName,desc
        ));
    }
}
