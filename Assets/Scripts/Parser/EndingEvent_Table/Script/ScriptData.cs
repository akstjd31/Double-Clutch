using System;

[Serializable]
public struct ScriptData
{
    public string scriptId;
    public int currentId;
    public int scriptType;
    public string text;
    public bool speaking;
    public string name;
    public string background;
    public string portrait;
    public string cg;
    public string bgm;
    public string sfx;
    public float textSpeed;
    public string cameraEffectId;
    public string startEffectId;

    
    public ScriptData(string scriptId, int currentId, int scriptType, string text, bool speaking,
                      string name, string background, string portrait, string cg, string bgm,
                      string sfx, float textSpeed, string cameraEffectId, string startEffectId)
    {
        this.scriptId = scriptId;
        this.currentId = currentId;
        this.scriptType = scriptType;
        this.text = text;
        this.speaking = speaking;
        this.name = name;
        this.background = background;
        this.portrait = portrait;
        this.cg = cg;
        this.bgm = bgm;
        this.sfx = sfx;
        this.textSpeed = textSpeed;
        this.cameraEffectId = cameraEffectId;
        this.startEffectId = startEffectId;
    }
}