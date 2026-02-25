using System;

public enum nation
{
    None, Korea, America
}

public enum namePart
{
    None, FirstName, MiddleName, LastName
}

[Serializable]
public struct Player_NameData
{
    public string ID;
    public nation nation;
    public namePart namePart;
    public string nameKey;
    public string desc;

    public Player_NameData(string ID, nation nation, namePart namePart, string nameKey,string desc)
    {
        this.ID = ID;
        this.nation = nation;
        this.namePart = namePart;
        this.nameKey = nameKey;
        this.desc = desc;
    }

}