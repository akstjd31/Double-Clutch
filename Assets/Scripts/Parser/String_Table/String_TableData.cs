using System;
[Serializable]
public struct String_TableData
{
    public string stringKey;
    public string ko;
    public string en;
    public string ja;

    public String_TableData(string stringKey, string ko, string en, string ja)
    {
        this.stringKey = stringKey;
        this.ko = ko;
        this.en = en;
        this.ja = ja;
    }
}
