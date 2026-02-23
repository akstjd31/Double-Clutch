using System;

public enum teamSector
{
    None,
    NA,
    DOM,
    OS
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
        teamSector teamsector, string teamArchetypeId, teamTier teamTier, 
        int minHumanoidCount,int minHumanCount, int minAnimalCount, int minCountSum,
        int weightHumanoid, int weightHuman, int weightAnimal, int weightSum)
    {
        this.teamId = teamId;
        this.desc = desc;
        this.teamNameKey = teamNameKey;
        this.teamsector = teamsector;
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
