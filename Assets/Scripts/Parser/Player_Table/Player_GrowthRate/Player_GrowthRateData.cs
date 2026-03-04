using System;


[Serializable]
public struct Player_GrowthRateData
{
    public int Grade;
    public int minGrowthRate;
    public int maxGrowthRate;



    public Player_GrowthRateData(int key, int minGrowthRate, int maxGrowthRate)
    {
        this.Grade = key;
        this.minGrowthRate = minGrowthRate;
        this.maxGrowthRate = maxGrowthRate;
    }
}
