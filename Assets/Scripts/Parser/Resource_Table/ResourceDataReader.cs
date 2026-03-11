using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDataReader", menuName = "Scriptable Object/ResourceDataReader", order = int.MaxValue)]
public class ResourceDataReader : DataReaderBase
{
    [SerializeField] public List<ResourceData> DataList = new List<ResourceData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string resourceId = null, resourcePath = null;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "resourceId":
                    resourceId = val;
                    break;
                case "resourcePath":
                    resourcePath = val;
                    break;

                
            }
        }

        var ResourceData = new ResourceData
        (
            resourceId,resourcePath
        );

        DataList.Add(ResourceData);
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
