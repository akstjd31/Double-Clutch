using System;
[Serializable]
public struct Player_NameData
{
    public int ID;
    public nationType nation;
    public namePart namePart;
    public string nameKey;
    public string desc;

    public Player_NameData(int ID, nationType nation, namePart namePart, string nameKey,string desc)
    {
        this.ID = ID;
        this.nation = nation;
        this.namePart = namePart;
        this.nameKey = nameKey;
        this.desc = desc;
    }

}