using UnityEngine;
using System;

[Serializable]
public struct Team_ArchetypeData
{
    public string teamArchetypeId;
    public string desc;
    public string teamColorName;
    public int countPG;
    public int countSG;
    public int countSF;
    public int countPF;
    public int countC;
    public float weight2pt;
    public float weight3pt;
    public float weightPass;
    public float weightBlock;
    public float weightSteal;
    public float weightRebound;

    public Team_ArchetypeData(string teamId, string desc, string teamCName, int cPG, int cSG, int cSF, int cPF, int cC, float w2pt, float w3pt, float wP, float wB, float wS, float wR)
    {
        teamArchetypeId = teamId;
        this.desc = desc;
        teamColorName = teamCName;
        countPG = cPG;
        countSG = cSG;
        countSF = cSF;
        countPF = cPF;
        countC = cC;
        weight2pt = w2pt;
        weight3pt = w3pt;
        weightPass = wP;
        weightBlock = wB;
        weightSteal = wS;
        weightRebound = wR;
    }
}