using System;

public enum changeType
{
    None, Default, Wide, Narrow
}
[Serializable]
public struct Position_PresetData
{
    public int presetId;
    public Position positionType;
    public changeType changeType;
    public float offenseXMin;
    public float offenseXMax;
    public float offenseXMin2;
    public float offenseXMax2;
    public float offenseYMin;
    public float offenseYMax;
    public float P_LtoC;
    public float P_CtoC;

    public Position_PresetData(int presetId, Position positionType, changeType changeType, float offenseXMin, float offenseXMax, float offenseXMin2, float offenseXMax2, float offenseYMin, float offenseYMax, float pLtoC, float pCtoC)
    {
        this.presetId = presetId;
        this.positionType = positionType;
        this.changeType = changeType;
        this.offenseXMin = offenseXMin;
        this.offenseXMax = offenseXMax;
        this.offenseXMin2 = offenseXMin2;
        this.offenseXMax2 = offenseXMax2;
        this.offenseYMin = offenseYMin;
        this.offenseYMax = offenseYMax;
        this.P_LtoC = pLtoC;
        this.P_CtoC = pCtoC;
    }
}
