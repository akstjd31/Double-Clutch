using System.Collections.Generic;
using UnityEngine;

public class EventSelector : MonoBehaviour
{
    [SerializeField] private Event_DataModelReader _dataModelReader;
    [SerializeField] private Event_ScriptSelectorReader _scriptSelectorReader;

    //2. 이벤트 순서 결정 - 매번 새로 작성해야 함.
    private Queue<string> _screenplayIdList = new();
    [Header("이벤트 순서")]
    [SerializeField] private List<string> _debugList_screenplayIdList;

    int _max;
    [SerializeField] private List<Student> _myStudents;
    private Dictionary<int, List<RandomEvent>> _candidateDictionary;

    public Queue<string> ScreenplayIdList => _screenplayIdList;

    private void OnEnable()
    {
        _myStudents = StudentManager.Instance.MyStudents;
    }

    public void EventSelect()
    {
        _candidateDictionary = EventManager.Instance.CandidateDictionary;
        CooldownTurnCheck();
    }


    //2. 쿨타임 체크 및 우선순위 비교
    private void CooldownTurnCheck()
    {
        List<RandomEvent> events = new List<RandomEvent>();
        var data = _dataModelReader.DataList;
        _screenplayIdList.Clear();


        //학생들 전체 검사
        for (int i = 0; i < _myStudents.Count; i++)
        {
            events.Clear();

            var studentID = _myStudents[i].StudentId;
            //딕셔너리에 등록되지 않은 학생은 스킵
            if (!_candidateDictionary.TryGetValue(studentID, out var list))
            {
                continue;
            }

            Debug.Log($"쿨타임/우선순위검사");
            //딕셔너리>학생>이벤트리스트 개수만큼 체크
            events = MaxPriorityNumber(list);


            Debug.Log($"events {events.Count}");

            //Debug.Log($"------[2차 후보]------");
            //foreach (var n in events)
            //{
            //    Debug.Log($"{i} - {n.EventId}");
            //}
            //Debug.Log($"----------------------");

            //우선순위가 같은 이벤트가 2개 이상이라면 그 중에 하나만 뽑기
            //Q. 우선순위가 같은 세개의 이벤트 중에 하나를 뽑았는데 뽑힌 이벤트가 발생확률이 낮으면 아무 이벤트도 발생하지 않을 수 있나?
            if (events.Count > 1)
            {
                int random = Random.Range(0, events.Count);
                //Debug.Log($"------[결과]------");
                //Debug.Log($"이벤트 리스트에 추가 {events[random].EventId}");
                //Debug.Log($"----------------------");
                PersonalityEvent(_myStudents[i], events[random]);
            }
            else if (events.Count == 1)
            {
                //발생확률 체크
                //이벤트발생 리스트에 넣기.
                PersonalityEvent(_myStudents[i], events[0]);
                //Debug.Log($"------[결과]------");
                //Debug.Log($"후보 추가 {events[0].EventId}");
                //Debug.Log($"----------------------");
            }
        }
        _debugList_screenplayIdList = new List<string>(_screenplayIdList);
    }

    //우선순위 최댓값 찾기
    private List<RandomEvent> MaxPriorityNumber(List<RandomEvent> data)
    {
        _max = int.MinValue;
        List<RandomEvent> resultList = new List<RandomEvent>();

        for (int i = 0; i < data.Count; i++)
        {
            int num = data[i].EventPriority;

            //쿨다운 타이머 true인 이벤트만 통과
            if (data[i].IsReady == false)
            {
                continue;
            }

            //발생확률 체크
            float eventProbability = Random.Range(0, 1f);

            //랜덤으로 나온 값이 요구하는 값보다 크면 후보 탈락.
            if (eventProbability >= data[i].PotentialPercent)
            {
                continue;
            }

            //현재 숫자가 최대숫자라면
            if (num > _max)
            {
                _max = num;
                //초기화 후 추가
                resultList.Clear();
                resultList.Add(data[i]);
            }
            //같은 숫자면 그냥 추가
            else if (num == _max)
            {
                resultList.Add(data[i]);
            }
        }
        Debug.Log($"--------------------------------");

        return resultList;
    }

    //2. _studentEventList 에 있는 리스트에 기반하여 실행될 이벤트 스크립트 큐 제작 _scriptSelectorReader
    public void PersonalityEvent(Student student, RandomEvent selectedEvent)
    {
        var screenPlay = _scriptSelectorReader.DataList;
        string eventId = selectedEvent.EventId;

        //eventId, 학생의 coreType이 같은 스크립트의 selectorId를 저장
        //_scriptSelectorReader개수만큼 반복
        for (int i = 0; i < screenPlay.Count; i++)
        {
            //결정된 이벤트 아이디와 같은 행 찾기
            //Debug.Log($"{student.StudentId} - 이벤트Id:{eventId} / 스크립트:{script[i].eventId} : {eventId.Equals(script[i].eventId)}");
            //Debug.Log($"{student.StudentId} - 학생 성격:{student.PersonalityData.core} / 스크립트:{script[i].selectCoreType} : {student.PersonalityData.core.Equals(script[i].selectCoreType)}");
            if (eventId.Equals(screenPlay[i].eventId))
            {
                if (student.PersonalityData.core.Equals(screenPlay[i].selectCoreType))
                {
                    _screenplayIdList.Enqueue(screenPlay[i].scriptId);
                    Debug.Log($"큐 추가 {student.StudentId} - {screenPlay[i].scriptId}");
                    break;
                }
            }
        }
    }

}
