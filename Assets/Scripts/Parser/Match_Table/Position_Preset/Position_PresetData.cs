using System;

public enum changeType
{
    None, Default, Wide, Narrow
}
[Serializable]
public struct Position_PresetData
{
    public int presetId;
    public positionType positionType;
    public changeType changeType;
    public float offenseXMin;
    public float offenseXMax;
    public float offenseYMin;
    public float offenseYMax;

    public Position_PresetData(int presetId,positionType positionType,changeType changeType,float offenseXMin,float offenseXMax,float offenseYMin,float offenseYMax)
    {
        this.presetId = presetId;
        this.positionType = positionType;
        this.changeType = changeType;
        this.offenseXMin = offenseXMin;
        this.offenseXMax = offenseXMax;
        this.offenseYMin = offenseYMin;
        this.offenseYMax = offenseYMax;
    }
}
