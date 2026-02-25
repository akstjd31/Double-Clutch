using System;


[Serializable]
public struct Player_GrowthRateData
{
    public int key;
    public int minGrowthRate;
    public int maxGrowthRate;



    public Player_GrowthRateData(int key, int minGrowthRate, int maxGrowthRate)
    {
        this.key = key;
        this.minGrowthRate = minGrowthRate;
        this.maxGrowthRate = maxGrowthRate;
    }
}
