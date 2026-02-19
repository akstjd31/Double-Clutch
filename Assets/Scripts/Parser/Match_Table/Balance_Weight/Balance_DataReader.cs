using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Balance_DataReader", menuName = "Scriptable Object/Balance_DataReader", order = int.MaxValue)]
public class Balance_DataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Balance_Data> DataList = new List<Balance_Data>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        int index = 0;
        string weightId = null;
        int value = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "index":
                    int.TryParse(val, out index);
                    break;
                case "weightId":
                    weightId = val;
                    break;
                case "value":
                    int.TryParse(val, out value);
                    break;
            }
        }
        DataList.Add(new Balance_Data(
            index, weightId, value
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}

