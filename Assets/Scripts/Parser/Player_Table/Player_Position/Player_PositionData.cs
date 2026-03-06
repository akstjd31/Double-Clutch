using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
[Serializable]

public struct Player_PositionData
{
    public Position recommendId;
    public potential stat1;
    public int recommendation1;
    public potential stat2;
    public int recommendation2;
    public potential stat3;
    public int recommendation3;

    public Player_PositionData(Position recommendId, potential stat1, int recommendation1, potential stat2, int recommendation2, potential stat3, int recommendation3)
    {
        this.recommendId = recommendId;
        this.stat1 = stat1;
        this.recommendation1 = recommendation1;
        this.stat2 = stat2;
        this.recommendation2 = recommendation2;
        this.stat3 = stat3;
        this.recommendation3 = recommendation3;
    }
}
