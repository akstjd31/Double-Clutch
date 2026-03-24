using System;
using UnityEngine;

[Serializable]
public struct TutorialData 
{
    public string slideOrder;
    public string tutorialImageId;
    public string narrationKey;
    public string dialogueKey;
    public string speakerKey;
    public bool isSkippable;

    public TutorialData(string slideOrder, string tutorialImageId,string narrationKey, string dialogueKey, string speakerKey,bool isSkippable)
    {
        this.slideOrder = slideOrder;
        this.tutorialImageId = tutorialImageId;
        this.narrationKey = narrationKey;
        this.dialogueKey = dialogueKey;
        this.speakerKey = speakerKey;
        this.isSkippable = isSkippable;
    }
}
