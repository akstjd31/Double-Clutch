using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProfileDataReader", menuName = "Scriptable Object/ProfileDataReader", order = int.MaxValue)]
public class ProfileDataReader : DataReaderBase
{
    [SerializeField] public List<ProfileData> DataList = new List<ProfileData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string profileId = null;
        int visualId = 0, unlockCount = 0;
        bool isDefault = false;
        string playerImage = null;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "profileId":
                    profileId = val;
                    break;

                case "visualId":
                    int.TryParse(val, out visualId);
                    break;

                case "unlockCount":
                    int.TryParse(val, out unlockCount);
                    break;
                case "isDefault":
                    isDefault = ParseBool(val);
                    break;
                case "playerImage":
                    playerImage = val;
                    break;
            }
        }

        var ProfileData = new ProfileData
        (
            profileId, visualId, unlockCount, isDefault, playerImage
        );

        DataList.Add(ProfileData);
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
