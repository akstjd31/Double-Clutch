using UnityEngine;
using System;

[Serializable]
public struct League_RewardData
{
    public string leagueRewardId;
    public int rewardFameWin;
    public int rewardGoldWin;
    public int rewardGoldEach;
    public float rewardGoldMultiplier;

    public League_RewardData(string leagueRId, int rewardFW, int rewardGW, int rewardGE, float rewardGM)
    {
        leagueRewardId = leagueRId;
        rewardFameWin = rewardFW;
        rewardGoldWin = rewardGW;
        rewardGoldEach = rewardGE;
        rewardGoldMultiplier = rewardGM;
    }
}
