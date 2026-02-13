using System;

public enum coreType
{
    None,
    Discipline,
    Bond,
    Cold,
    Instinct
}

public enum personalityType
{
    None, 
    Principled, Guardian, Tactician, Exemplary, 
    Kind, Loyal, Sincere, Moodmaker, 
    Planner, Opportunist, Cunning, Believer, 
    Bluffer, Wild, Egoist, Fighter
}

[Serializable]
public struct Player_PersonalityData
{
    public int personalityId;
    public coreType core;
    public personalityType personality;
    public string personalityName;
    public string desc;

    public Player_PersonalityData(int personalityId, coreType core, personalityType personality, string personalityName, string desc)
    {
        this.personalityId = personalityId;
        this.core = core;
        this.personality = personality;
        this.personalityName = personalityName;
        this.desc = desc;
    }
}