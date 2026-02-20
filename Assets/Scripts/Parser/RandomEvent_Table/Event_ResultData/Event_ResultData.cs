using System;
using UnityEngine;

[Serializable]
public struct Event_ResultData
{
    public string resultId;
    public string scriptId;
    public string matchPersonalityId;
    public int outcomeGrade;
    public potential potentialChangeType;

    public int potentialChangeValue;
    public int conditionChange;
    public string statusChange;
    public string reactionPortraitId;
    public string resultscriptKey;

    public string seId;
    public string floatingText;

    public Event_ResultData(
        string resultId, string scriptId, string matchPersonalityId, int outcomeGrade, potential potentialChangeType,
        int potentialChangeValue, int conditionChange, string statusChange, string reactionPortraitId, string resultscriptKey,
        string seId, string floatingText
        )
    {
        this.resultId = resultId;
        this.scriptId = scriptId;
        this.matchPersonalityId = matchPersonalityId;
        this.outcomeGrade = outcomeGrade;
        this.potentialChangeType = potentialChangeType;

        this.potentialChangeValue = potentialChangeValue;
        this.conditionChange = conditionChange;
        this.statusChange = statusChange;
        this.reactionPortraitId = reactionPortraitId;
        this.resultscriptKey = resultscriptKey;

        this.seId = seId;
        this.floatingText = floatingText;
    }
}
