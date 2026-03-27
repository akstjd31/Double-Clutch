using System;

[Serializable]
public struct EndingRollData
{
    public string rollId;
    public string typeKey;
    public string nameKey;

    
    public EndingRollData(string rollId, string typeKey, string nameKey)
    {
        this.rollId = rollId;
        this.typeKey = typeKey;
        this.nameKey = nameKey;
    }
}