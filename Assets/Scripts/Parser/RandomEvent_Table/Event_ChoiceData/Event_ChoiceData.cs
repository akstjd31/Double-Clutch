using System;
using UnityEngine;

[Serializable]
public struct Event_ChoiceData
{
    public int index;
    public string scriptId;
    public int currentId;
    public int nextId;
    public string textKey;

    public textType textType;
    public string standingRight;
    public string standingLeft;
    public string background;
    public string cg;

    public string choice01;
    public int nextId01;
    public string choice02;
    public int nextId02;
    public string choice03;

    public int nextId03;
    public string bgm;
    public string sfx;
    public string buttonEffectId;
    public string hoverSeId;

    public float textSpeed;
    public string cameraEffectId;
    public string startEffectId;


    public Event_ChoiceData
        (
        int index, string scriptId, int currentId, int nextId, string textKey,
        textType textType, string standingRight, string standingLeft, string background, string cg,
        string choice01, int nextId01, string choice02, int nextId02, string choice03,
        int nextId03, string bgm, string sfx, string buttonEffectId, string hoverSeId,
        float textSpeed, string cameraEffectId, string startEffectId

        )
    {
        this.index = index;
        this.scriptId = scriptId;
        this.currentId = currentId;
        this.nextId = nextId;
        this.textKey = textKey;

        this.textType = textType;
        this.standingRight = standingRight;
        this.standingLeft = standingLeft;
        this.background = background;
        this.cg = cg;

        this.choice01 = choice01;
        this.nextId01 = nextId01;
        this.choice02 = choice02;
        this.nextId02 = nextId02;
        this.choice03 = choice03;

        this.nextId03 = nextId03;
        this.bgm = bgm;
        this.sfx = sfx;
        this.buttonEffectId = buttonEffectId;
        this.hoverSeId = hoverSeId;

        this.textSpeed = textSpeed;
        this.cameraEffectId = cameraEffectId;
        this.startEffectId = startEffectId;
    }

}
