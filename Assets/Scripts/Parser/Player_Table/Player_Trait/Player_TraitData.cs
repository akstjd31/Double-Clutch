using System;

[Serializable]
public struct Player_TraitData
{
    public int traitId;
    public string traitName;
    public string desc;

    public Player_TraitData
        (
            int _traitId, string _traitName, string _desc
        )
    {
        traitId = _traitId;
        traitName = _traitName;
        desc = _desc;
    }
}
