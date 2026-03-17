using System;

public enum teamSector
{
    None = 0,
    NA = 1 << 0,    // 1
    DOM = 1 << 1,   // 2
    OS = 1 << 2     // 4
}

public enum teamTier
{
    None,
    D,C,B,A,S,SS,SSS
}

[Serializable]
public struct Rival_MasterData
{
    public string teamId;
    public string desc;
    public string teamNameKey;
    public teamSector teamsector;
    public nation nation;
    public string teamArchetypeId;
    public teamTier teamTier;
    public int minHumanoidCount;
    public int minHumanCount;
    public int minAnimalCount;
    public int minCountSum;
    public int weightHumanoid;
    public int weightHuman;
    public int weightAnimal;
    public int weightSum;

    public Rival_MasterData(
        string teamId, string desc, string teamNameKey, 
        teamSector teamsector, nation nation, string teamArchetypeId, teamTier teamTier, 
        int minHumanoidCount,int minHumanCount, int minAnimalCount, int minCountSum,
        int weightHumanoid, int weightHuman, int weightAnimal, int weightSum)
    {
        this.teamId = teamId;
        this.desc = desc;
        this.teamNameKey = teamNameKey;
        this.teamsector = teamsector;
        this.nation = nation;
        this.teamArchetypeId = teamArchetypeId;
        this.teamTier = teamTier;
        this.minHumanoidCount = minHumanoidCount;
        this.minHumanCount = minHumanCount;
        this.minAnimalCount = minAnimalCount;
        this.minCountSum = minCountSum;
        this.weightHumanoid = weightHumanoid;
        this.weightHuman = weightHuman;
        this.weightAnimal = weightAnimal;
        this.weightSum = weightSum;
    }
}
