using System;


[Serializable]
public struct Team_TrainingData
{

    public string teamTrainingId;
    public string desc;    
    public int trainingMode;
    public int allGain;
    public int mainGain;
    public int subGain;
    public int trainingCost;
    public int conditionCostMin;
    public int conditionCostMax;
    public string trainingNameKey;
    public string trainingdescKey;
    public string icon;




    public Team_TrainingData(string teamTrainingId, string desc, int trainingMode, int allGain, int mainGain, int subGain, int trainingCost, int conditionCostMin, int conditionCostMax, string trainingNameKey, string trainingdescKey, string icon)
    {
        this.teamTrainingId = teamTrainingId;
        this.desc = desc;        
        this.trainingMode = trainingMode;        
        this.allGain = allGain;
        this.mainGain = mainGain;
        this.subGain = subGain;
        this.trainingCost = trainingCost;
        this.conditionCostMin = conditionCostMin;
        this.conditionCostMax = conditionCostMax;
        this.trainingNameKey = trainingNameKey;
        this.trainingdescKey = trainingdescKey;
        this.icon = icon;
    }
}
