using System;


[Serializable]
public struct Individual_TrainingData
{
    public string trainingId;
    public string desc;
    public potential mainPotential;
    public int mainGain;
    public potential subPotential;
    public int subGain;
    public int conditionCostMin;
    public int conditionCostMax;
    public int trainingCost;
    public string trainingNameKey;
    public string trainingdescKey;
    public string icon;

    


    public Individual_TrainingData(string trainingId, string desc, potential mainPotential, int mainGain, potential subPotential, int subGain, int conditionCostMin, int conditionCostMax, int trainingCost, string trainingNameKey, string trainingdescKey, string icon)
    {
        this.trainingId = trainingId;
        this.desc = desc;
        this.mainPotential = mainPotential;
        this.mainGain = mainGain;
        this.subPotential = subPotential;
        this.subGain = subGain;
        this.conditionCostMin = conditionCostMin;
        this.conditionCostMax = conditionCostMax;
        this.trainingCost = trainingCost;
        this.trainingNameKey = trainingNameKey;
        this.trainingdescKey = trainingdescKey;
        this.icon = icon;
    }
}