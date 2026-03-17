using System.Collections.Generic;
using UnityEngine;

public class RandomEventSaveData : SaveBase
{
    public List<StudentEventEntry> studentEventList = new(); // 학생별 이벤트 리스트
}

[System.Serializable]
public class StudentEventEntry
{
    public int studentId;
    public List<RandomEvent> events;
}
