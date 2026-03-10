using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BannedWordData
{
    public string[] wordData;
    public BannedWordData(string[] wordData)
    {
        this.wordData = wordData;
    }
}
