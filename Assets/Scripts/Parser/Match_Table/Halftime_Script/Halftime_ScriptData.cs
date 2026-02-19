using System;
using UnityEngine;

public enum textType
{
    None,Desc,Choice,End
}

public enum potential
{
    None,Stat2pt,Stat3pt,StatPass,StatSteal,StatBlock,StatRebound
}

[Serializable]
public struct Halftime_ScriptData
{
    public int index;
    public int scriptId;
    public int currentId;
    public int nextId;
    public string textKey;
    public textType textType;
    public string standingRight;
    public string standingLeft;
    public string background;
    public string cg;
    public string choice01;
    public potential choiceStat01;
    public positionType choicePosition01;
    public float changeStat01;
    public changeType changePosition01;
    public int nextId01;
    public string choice02;
    public potential choiceStat02;
    public positionType choicePosition02;
    public float changeStat02;
    public changeType changePosition02;
    public int nextId02;
    public string choice03;
    public potential choiceStat03;
    public positionType choicePosition03;
    public float changeStat03;
    public changeType changePosition03;
    public int nextId03;
    public string bgm;
    public string sfx;
    public string buttonEffectId;
    public string hoverSeId;
    public float textSpeed;
    public string cameraEffectId;
    public string startEffectId;

    public Halftime_ScriptData(int index,
int scriptId,
int currentId,
int nextId,
string textKey,
textType textType,
string standingRight,
string standingLeft,
string background,
string cg,
string choice01,
potential choiceStat01,
positionType choicePosition01,
float changeStat01,
changeType changePosition01,
int nextId01,
string choice02,
potential choiceStat02,
positionType choicePosition02,
float changeStat02,
changeType changePosition02,
int nextId02,
string choice03,
potential choiceStat03,
positionType choicePosition03,
float changeStat03,
changeType changePosition03,
int nextId03,
string bgm,
string sfx,
string buttonEffectId,
string hoverSeId,
float textSpeed,
string cameraEffectId,
string startEffectId)
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
        this.choiceStat01 = choiceStat01;
        this.choicePosition01 = choicePosition01;
        this.changeStat01 = changeStat01;
        this.changePosition01 = changePosition01;
        this.nextId01 = nextId01;
        this.choice02 = choice02;
        this.choiceStat02 = choiceStat02;
        this.choicePosition02 = choicePosition02;
        this.changeStat02 = changeStat02;
        this.changePosition02 = changePosition02;
        this.nextId02 = nextId02;
        this.choice03 = choice03;
        this.choiceStat03 = choiceStat03;
        this.choicePosition03 = choicePosition03;
        this.changeStat03 = changeStat03;
        this.changePosition03 = changePosition03;
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
