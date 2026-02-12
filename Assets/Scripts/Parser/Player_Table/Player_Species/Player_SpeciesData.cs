using System;

[Serializable]
public struct Player_SpeciesData
{
    public int speciesId;
    public speciesType species;
    public string speciesName;
    public string desc;
    public Player_SpeciesData(int speciesId, speciesType species, string speciesName, string desc)
    {
        this.speciesId = speciesId;
        this.species = species;
        this.speciesName = speciesName;
        this.desc = desc;
    }
}

