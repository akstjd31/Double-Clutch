using System;
using UnityEngine;


[Serializable]
public struct Player_PassiveGradeData
{
    public int gradeId;
    public string skillName;
    public float spawnRate;

    public Player_PassiveGradeData(int gradeId, string skillName, float spawnRate)
    {
        this.gradeId = gradeId;
        this.skillName = skillName;
        this.spawnRate = spawnRate;
    }
}