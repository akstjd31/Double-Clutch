using GoogleSheetsToUnity;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "League_LevelDataReader", menuName = "Scriptable Object/League_LevelDataReader", order = int.MaxValue)]
public class League_LevelDataReader : DataReaderBase
{
    [SerializeField] public List<League_LevelData> DataList = new List<League_LevelData>();

    internal void UpdateStats(List<GSTU_Cell> list)
    {
        string leagueLvId = null;
        bool isRivalTA = false, isRivalPA = false;
        int minP = 0, maxP = 0;
        float wPT_D = 0, wPT_C = 0, wPT_B = 0, wPT_A = 0, wPT_S = 0, wPT_SS = 0, wPT_SSS = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value ?? "";

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "leagueLevelId":
                    leagueLvId = val;
                    break;

                case "isRivalTraitApplied":
                    isRivalTA = ParseBool(val);
                    break;

                case "isRivalPassiveApplied":
                    isRivalPA = ParseBool(val);
                    break;

                case "minPotential":
                    int.TryParse(val, out minP);
                    break;

                case "maxPotential":
                    int.TryParse(val, out maxP);
                    break;

                case "weightPotentialTierD":
                    float.TryParse(val, out wPT_D);
                    break;

                case "weightPotentialTierC":
                    float.TryParse(val, out wPT_C);
                    break;

                case "weightPotentialTierB":
                    float.TryParse(val, out wPT_B);
                    break;

                case "weightPotentialTierA":
                    float.TryParse(val, out wPT_A);
                    break;

                case "weightPotentialTierS":
                    float.TryParse(val, out wPT_S);
                    break;

                case "weightPotentialTierSS":
                    float.TryParse(val, out wPT_SS);
                    break;

                case "weightPotentialTierSSS":
                    float.TryParse(val, out wPT_SSS);
                    break;
            }
        }

        var LeagueLvData = new League_LevelData
        (
            leagueLvId, isRivalTA, isRivalPA, minP, maxP, wPT_D, wPT_C, wPT_B, wPT_A, wPT_S, wPT_SS, wPT_SSS
        );

        DataList.Add(LeagueLvData);
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
