using System;


[Serializable]
public struct Team_RestData
{
    public string teamRestId;
    public string desc;
    public int conditionRecovery;
    public int cureOverworkProb;
    public int restCost;
    public string restNameKey;
    public string restdescKey;
    public string icon;




    public Team_RestData(string teamRestId, string desc, int conditionRecovery, int cureOverworkProb, int restCost, string restNameKey, string restdescKey, string icon)
    {
        this.teamRestId = teamRestId;
        this.desc = desc;
        this.conditionRecovery = conditionRecovery;
        this.cureOverworkProb = cureOverworkProb;
        this.restCost = restCost;
        this.restNameKey = restNameKey;
        this.restdescKey = restdescKey;
        this.icon = icon;
    }
}