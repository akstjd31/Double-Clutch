using System;

[Serializable]
public struct Individual_RestData
{
    public string restId;
    public string desc;
    
    public int conditionRecovery;
    public int isCureInjury;
    public int cureOverworkProb;
    public int restCost;

    public string restNameKey;
    public string restdescKey;

    public string icon;




    public Individual_RestData(string restId, string desc, int conditionRecovery, int isCureInjury, int cureOverworkProb, int restCost, string restNameKey, string restdescKey, string icon)
    {
        this.restId = restId;
        this.desc = desc;
        this.conditionRecovery = conditionRecovery;
        this.isCureInjury = isCureInjury;
        this.cureOverworkProb = cureOverworkProb;
        this.restCost = restCost;
        this.restNameKey = restNameKey;
        this.restdescKey = restdescKey;
        this.icon = icon;        
    }
}