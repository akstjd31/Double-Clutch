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
    public int minAndroidCount;
    public int minHumanCount;
    public int minAnimalCount;
    public int minCountSum;
    public int weightAndroid;
    public int weightHuman;
    public int weightAnimal;
    public int weightSum;

    public Rival_MasterData(
        string teamId, string desc, string teamNameKey, 
        teamSector teamsector, string teamArchetypeId, teamTier teamTier, 
        int minAndroidCount,int minHumanCount, int minAnimalCount, int minCountSum,
        int weightAndroid, int weightHuman, int weightAnimal, int weightSum)
    {
        this.teamId = teamId;
        this.desc = desc;
        this.teamNameKey = teamNameKey;
        this.teamsector = teamsector;
        this.teamArchetypeId = teamArchetypeId;
        this.teamTier = teamTier;
        this.minAndroidCount = minAndroidCount;
        this.minHumanCount = minHumanCount;
        this.minAnimalCount = minAnimalCount;
        this.minCountSum = minCountSum;
        this.weightAndroid = weightAndroid;
        this.weightHuman = weightHuman;
        this.weightAnimal = weightAnimal;
        this.weightSum = weightSum;
    }
}
