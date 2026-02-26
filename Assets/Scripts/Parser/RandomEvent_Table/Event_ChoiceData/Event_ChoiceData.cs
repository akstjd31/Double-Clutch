using System;
using UnityEngine;

public enum textType
{
    Choice, Desc, End
}

[Serializable]
public struct Event_ChoiceData
{
    public string index;
    public string scriptId;
    public int currentId;
    public textType textType;

    public string textKey;
    public string standingLeft;
    public string standingRight;
    public string background;
    public string cg;

    public string choice01;
    public string choice02;
    public string choice03;

    public string bgm;
    public string sfx;
    public string buttonEffectId;
    public string hoverSeId;

    public float textSpeed;
    public string cameraEffectId;
    public string startEffectId;


    public Event_ChoiceData
        (
        string index, string scriptId, int currentId,
        textType textType, string textKey, string standingLeft, string standingRight, string background, string cg,
        string choice01, string choice02, string choice03,
        string bgm, string sfx, string buttonEffectId, string hoverSeId,
        float textSpeed, string cameraEffectId, string startEffectId

        )
    {
        this.index = index;
        this.scriptId = scriptId;
        this.currentId = currentId;
        this.textType = textType;

        this.textKey = textKey;
        this.standingLeft = standingLeft;
        this.standingRight = standingRight;
        this.background = background;
        this.cg = cg;

        this.choice01 = choice01;
        this.choice02 = choice02;
        this.choice03 = choice03;

        this.bgm = bgm;
        this.sfx = sfx;
        this.buttonEffectId = buttonEffectId;
        this.hoverSeId = hoverSeId;

        this.textSpeed = textSpeed;
        this.cameraEffectId = cameraEffectId;
        this.startEffectId = startEffectId;
    }

}
