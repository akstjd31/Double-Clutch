using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventManager : Singleton<EventManager>
{
    #region DB
    [SerializeField] private Event_DataModelReader _dataModelReader;
    [SerializeField] private Event_ChoiceDataReader _choiceDataReader;
    #endregion
    [SerializeField] private List<Student> _myStudents;

    //1. 선수별 이벤트 후보 저장 딕셔너리 : 선수ID, 이벤트 - 이벤트 쿨타임 관리 해야 함.
    private Dictionary<int, List<RandomEvent>> _candidateDictionary = new();
    [SerializeField] private List<int> _debugList_candidateDictionary;

    //2. <이벤트 스크립트 id, 출력텍스트 리스트>를 저장한 딕셔너리
    private Dictionary<string, Dictionary<int, Event_ChoiceData>> _eventScript = new();
    [SerializeField] private List<string> _debugList_eventScript;


    public Dictionary<int, List<RandomEvent>> CandidateDictionary => _candidateDictionary;
    public Dictionary<string, Dictionary<int, Event_ChoiceData>> EventScript => _eventScript;

    //학생별 가능한 이벤트 리스트를 딕셔너리에 저장
    //1. 요구 잠재력 조건 만족
    public void CharacterEvent()
    {
        _myStudents = StudentManager.Instance.MyStudents;
        var data = _dataModelReader.DataList;
        _candidateDictionary.Clear();


        //학생들 전체 검사
        for (int i = 0; i < _myStudents.Count; i++)
        {
            var studentID = _myStudents[i].StudentId;

            //등록되지 않은 ID면 새로 딕셔너리에 등록
            if (_candidateDictionary.ContainsKey(studentID) == false)
            {
                _candidateDictionary.Add(studentID, new List<RandomEvent>());
            }

            //모든 이벤트 수만큼 검사
            for (int j = 0; j < data.Count; j++)
            {
                //스텟 검사
                if (_myStudents[i].GetCurrentStat(data[j].mainPotentialType) >= data[j].requiredPotentialValue)
                {
                    //이벤트 id랑 쿨타임 저장
                    _candidateDictionary[studentID].Add(new RandomEvent(data[j].eventId, data[j].potentialPercent, data[j].cooldownTurn, data[j].eventPriority));
                }
            }
            //Debug.Log($"------[1차 후보]------");
            //foreach (var n in _candidateDictionary[studentID])
            //{
            //    Debug.Log($"{i} - {n.EventId}");
            //}
            //Debug.Log($"----------------------");

        }
        _debugList_candidateDictionary = new List<int>(_candidateDictionary.Keys);
    }

    public void CreateList()
    {
        var data = _choiceDataReader.DataList;

        //시트 개수만큼 반복
        for (int i = 0; i < data.Count; i++)
        {
            string cId = data[i].scriptId;

            //ID 키값으로 딕셔너리가 없으면 새로 생성
            if (!_eventScript.TryGetValue(cId, out Dictionary<int, Event_ChoiceData> chDic))
            {
                chDic = new();
                _eventScript.Add(cId, chDic);
            }
            chDic.Add(data[i].currentId, data[i]);
        }

        _debugList_eventScript = new List<string>(_eventScript.Keys);
     }


    //주차가 끝나면 모든 학생이 가지고 있는 이벤트
    //IsReady = false만 쿨타임 감소
    public void WeekendCooldown()
    {
        var data = _dataModelReader.DataList;

        //학생들 전체 검사
        for (int i = 0; i < _myStudents.Count; i++)
        {
            var studentID = _myStudents[i].StudentId;
            //딕셔너리에 등록되지 않은 학생은 스킵
            if (_candidateDictionary.ContainsKey(studentID) == false)
            {
                continue;
            }

            //딕셔너리>학생>이벤트리스트 개수만큼 체크
            for (int j = 0; j < _candidateDictionary[studentID].Count; j++)
            {
                //쿨다운 시작된 이벤트만 감소
                if (_candidateDictionary[studentID][j].IsReady == false)
                {
                    continue;
                }
                //쿨다운 값 감소
                _candidateDictionary[studentID][j].Cooldown();
            }
        }
    }

}
