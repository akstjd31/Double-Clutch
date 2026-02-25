using System;
[Serializable]
public struct Player_VisualData
{
    public int visualId;
    public int species;
    public string assetKey;
    public string desc;

    public Player_VisualData(int visualId, int speciesId, string assetKey, string desc)
    {
        this.visualId = visualId;
        this.species = speciesId;
        this.assetKey = assetKey;
        this.desc = desc;
    }
}
