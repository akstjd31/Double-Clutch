using System;
[Serializable]
public struct String_TableData
{
    public string stringKey;
    public string Kr;
    public string En;

    public String_TableData(string stringKey, string Kr, string En)
    {
        this.stringKey = stringKey;
        this.Kr = Kr;
        this.En = En;
    }
}
