using System;

[Serializable]
public struct Player_TraitData
{
    public string traitId;
    public string traitName;
    public string desc;
    public string traitResource;

    public Player_TraitData
        (
            string _traitId, string _traitName, string _desc, string _traitResource
        )
    {
        traitId = _traitId;
        traitName = _traitName;
        desc = _desc;
        traitResource = _traitResource;
    }
}
