using System;

public enum speciesType
{
    None,
    Animal,
    Human,
    Humanoid
}


[Serializable]
public struct Player_SpeciesData
{
    public string speciesId;
    public speciesType species;
    public string speciesName;
    public string desc;
    public Player_SpeciesData(string speciesId, speciesType species, string speciesName, string desc)
    {
        this.speciesId = speciesId;
        this.species = species;
        this.speciesName = speciesName;
        this.desc = desc;
    }
}

