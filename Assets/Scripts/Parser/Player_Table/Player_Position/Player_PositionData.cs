using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
[Serializable]

public struct Player_PositionData
{
    public string recommendId;
    public potential state1;
    public int recommendation1;
    public potential state2;
    public int recommendation2;
    public potential state3;
    public int recommendation3;

    public Player_PositionData(string recommendId, potential state1, int recommendation1, potential state2, int recommendation2, potential state3, int recommendation3)
    {
        this.recommendId = recommendId;
        this.state1 = state1;
        this.recommendation1 = recommendation1;
        this.state2 = state2;
        this.recommendation2 = recommendation2;
        this.state3 = state3;
        this.recommendation3 = recommendation3;
    }
}
