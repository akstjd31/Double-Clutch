using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    [SerializeField] private Event_ChoiceDataReader _choiceDataReader;
    [SerializeField] private String_RandomEventStringReader _randomEventStringReader;

    [SerializeField] private List<string> _debugList;

    //<이벤트 스크립트 id, 출력텍스트 리스트>를 저장한 딕셔너리
    Dictionary<string, List<Event_ChoiceData>> _eventScreenplay = new();

    //스크립트 id별 대본 리스트 생성
    //예)아이디가 'a'인 행만 모인 리스트 
    public void CreateList()
    {
        var data = _choiceDataReader.DataList;

        //시트 개수만큼 반복
        for (int i = 0; i < data.Count; i++)
        {
            string currentId = data[i].scriptId;

            //ID 키값으로 리스트가 없으면 새로 생성
            if (!_eventScreenplay.TryGetValue(currentId, out var list))
            {
                list = new List<Event_ChoiceData>();
                _eventScreenplay.Add(currentId, list);
            }
                list.Add(data[i]);
        }
        _debugList = new List<string>(_eventScreenplay.Keys);
    }
}
