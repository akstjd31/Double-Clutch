using System;
using UnityEngine;

[Serializable]
public struct Event_ResultData
{
    public string resultId;
    public string choiceId;
    public string scriptId;
    public int nextId;
    public personalityType matchPersonalityId;
    public potential potentialChangeType;

    public int potentialChangeValue;
    public int conditionChange;
    public string statusChange;
    public string reactionPortraitId;
    public string resultScriptKey;

    public string seId;
    public string floatingText;

    public Event_ResultData(
        string resultId, string choiceId, string scriptId, int nextId, personalityType matchPersonalityId, potential potentialChangeType,
        int potentialChangeValue, int conditionChange, string statusChange, string reactionPortraitId, string resultScriptKey,
        string seId, string floatingText
        )
    {
        this.resultId = resultId;
        this.choiceId = choiceId;
        this.scriptId = scriptId;
        this.nextId = nextId;
        this.matchPersonalityId = matchPersonalityId;
        this.potentialChangeType = potentialChangeType;

        this.potentialChangeValue = potentialChangeValue;
        this.conditionChange = conditionChange;
        this.statusChange = statusChange;
        this.reactionPortraitId = reactionPortraitId;
        this.resultScriptKey = resultScriptKey;

        this.seId = seId;
        this.floatingText = floatingText;
    }
}
