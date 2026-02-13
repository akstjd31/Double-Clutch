using UnityEngine;
using System;

[Serializable]
public struct League_LevelData
{
    public string leagueLevelId;
    public bool isRivalTraitApplied;
    public bool isRivalPassiveApplied;
    public int minPotential;
    public int maxPotential;
    public float weightPotentialTierD;
    public float weightPotentialTierC;
    public float weightPotentialTierB;
    public float weightPotentialTierA;
    public float weightPotentialTierS;
    public float weightPotentialTierSS;
    public float weightPotentialTierSSS;

    public League_LevelData(string leagueLvId, bool isRivalTA, bool isRivalPA, int minP, int maxP, float wPT_D, float wPT_C, float wPT_B, float wPT_A, float wPT_S, float wPT_SS, float wPT_SSS)
    {
        leagueLevelId = leagueLvId;
        isRivalTraitApplied = isRivalTA;
        isRivalPassiveApplied = isRivalPA;
        minPotential = minP;
        maxPotential = maxP;
        weightPotentialTierD = wPT_D;
        weightPotentialTierC = wPT_C;
        weightPotentialTierB = wPT_B;
        weightPotentialTierA = wPT_A;
        weightPotentialTierS = wPT_S;
        weightPotentialTierSS = wPT_SS;
        weightPotentialTierSSS = wPT_SSS;
    }
}
