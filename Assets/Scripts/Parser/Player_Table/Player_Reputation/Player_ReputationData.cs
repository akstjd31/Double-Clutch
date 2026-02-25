using System;
[Serializable]
public struct Player_ReputationData
{
    public string reputationId;
    public int minReputationScore;
    public int minStepValue;
    public int maxReputationScore;
    public int maxStepValue;

    public Player_ReputationData(string reputationId, int minReputationScore, int minStepValue, int maxReputationScore, int maxStepValue)
    {
        this.reputationId = reputationId;
        this.minReputationScore = minReputationScore;
        this.minStepValue = minStepValue;
        this.maxReputationScore= maxReputationScore;
        this.maxStepValue = maxStepValue;
    }
}
