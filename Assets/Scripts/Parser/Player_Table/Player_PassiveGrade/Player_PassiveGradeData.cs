using System;
using UnityEngine;


[Serializable]
public struct Player_PassiveGradeData
{
    public int gradeId;
    public string skillName;
    public float spawnRate;
    public string passiveFrameResource;

    public Player_PassiveGradeData(int gradeId, string skillName, float spawnRate, string passiveFrameResource)
    {
        this.gradeId = gradeId;
        this.skillName = skillName;
        this.spawnRate = spawnRate;
        this.passiveFrameResource = passiveFrameResource;
    }
}