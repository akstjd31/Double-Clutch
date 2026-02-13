using System;
[Serializable]
public struct Player_ReputationData
{
    public int key;
    public int minReputationScore;
    public int minStepValue;
    public int maxReputationScore;
    public int maxStepValue;

    public Player_ReputationData(int key, int minReputationScore, int minStepValue, int maxReputationScore, int maxStepValue)
    {
        this.key = key;
        this.minReputationScore = minReputationScore;
        this.minStepValue = minStepValue;
        this.maxReputationScore= maxReputationScore;
        this.maxStepValue = maxStepValue;
    }
}
