using UnityEngine;
using System;

[Serializable]
public struct Player_StartingStateData
{
    public int grade;
    public int startMin;
    public int startMax;

    public Player_StartingStateData(int grade, int startMin, int startMax)
    {
        this.grade = grade;
        this.startMin = startMin;
        this.startMax = startMax;
    }
}