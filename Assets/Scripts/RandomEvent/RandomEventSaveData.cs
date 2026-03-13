using System.Collections.Generic;
using UnityEngine;

public class RandomEventSaveData : SaveBase
{
    public Dictionary<int, List<RandomEvent>> studentEventList; // 학생별 이벤트 리스트

    public RandomEventSaveData(Dictionary<int, List<RandomEvent>> studentEventList)
    {
        this.studentEventList = studentEventList;
    }

    public RandomEventSaveData()
    {
        
    }
}
