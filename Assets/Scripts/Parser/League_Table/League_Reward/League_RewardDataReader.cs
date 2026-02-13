using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "League_RewardDataReader", menuName = "Scriptable Object/League_RewardDataReader", order = int.MaxValue)]
public class League_RewardDataReader : DataReaderBase
{
    [SerializeField] public List<League_RewardData> DataList = new List<League_RewardData>();

    internal void UpdateStats(List<GSTU_Cell> list)
    {
        string leagueRId = null;
        int rewardFW = 0, rewardGW = 0, rewardGE = 0;
        float rewardGM = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value ?? "";

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "leagueRewardId":
                    leagueRId = val;
                    break;

                case "rewardFameWin":
                    int.TryParse(val, out rewardFW);
                    break;
                    
                case "rewardGoldWin":
                    int.TryParse(val, out rewardGW);
                    break;

                case "rewardGoldEach":
                    int.TryParse(val, out rewardGE);
                    break;

                case "rewardGoldMultiplier":
                    float.TryParse(val, out rewardGM);
                    break;
            }
        }

        var rewardData = new League_RewardData
        (
            leagueRId, rewardFW, rewardGW, rewardGE, rewardGM
        );

        DataList.Add(rewardData);
    }
}
