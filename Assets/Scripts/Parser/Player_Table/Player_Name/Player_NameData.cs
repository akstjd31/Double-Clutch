using System;

public enum nation
{
    None, Kr, Jp, Us, Ch, It, Fr, Sp
}

public enum namePart
{
    None, FirstName, MiddleName, LastName
}

[Serializable]
public struct Player_NameData
{
    public string nameId;
    public nation nation;
    public namePart namePart;
    public string nameKey;
    public string desc;

    public Player_NameData(string nameId, nation nation, namePart namePart, string nameKey,string desc)
    {
        this.nameId = nameId;
        this.nation = nation;
        this.namePart = namePart;
        this.nameKey = nameKey;
        this.desc = desc;
    }

}