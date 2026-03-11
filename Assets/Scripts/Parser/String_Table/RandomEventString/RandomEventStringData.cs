using System;
using UnityEngine;

[Serializable]
public struct RandomEventStringData
{
    public string stringKey;
    public string ko;
    public string en;
    public string ja;

    public RandomEventStringData(string stringKey, string ko, string en, string ja)
    {
        this.stringKey = stringKey;
        this.ko = ko;
        this.en = en;
        this.ja = ja;
    }
}
