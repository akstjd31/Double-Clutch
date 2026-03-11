using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.LightTransport;

[System.Serializable]
public class EventManager : MonoBehaviour
{
    [SerializeField] private Event_DataModelReader _dataModelReader;
    [SerializeField] private Event_ScriptSelectorReader _scriptSelectorReader;
    [SerializeField] private Event_ChoiceDataReader _choiceDataReader;
    [SerializeField] private String_RandomEventStringReader _randomEventStringReader;
    [SerializeField] private List<Student> _myStudents;

    [SerializeField] private EventUI _eventUI;
    [SerializeField] private EventString _eventString;

    //1. 선수별 이벤트 후보 저장 딕셔너리 : 선수ID, 이벤트
    private Dictionary<int, List<RandomEvent>> _candidateDictionary = new();

    //2. 선수별 이벤트 스크립트 선정 딕셔너리
    private Queue<string> _screenplayIdList = new();
    [Header("이벤트 순서")]
    [SerializeField] private List<string> debugQueue = new();

    //<이벤트 스크립트 id, 출력텍스트 리스트>를 저장한 딕셔너리
    Dictionary<string, List<Event_ChoiceData>> _eventScript = new();
    [Header("대본 딕셔너리 아이디 리스트 - 전체 이벤트 개수를 의미함")]
    [SerializeField] private List<string> _debugList;

    int _max;

    //어떤 선수의 이벤트인지 대상 선택.
    //매서드가 호출되면 이벤트가 실행되도록 함.

    //훈련시작 시에 호출
    public void EventCheck()
    {
        _myStudents = StudentManager.Instance.MyStudents;
        CharacterEvent();
        CooldownTurnCheck();
        CreateList();
    }


    //학생별 가능한 이벤트 리스트를 딕셔너리에 저장
    //1. 요구 잠재력 조건 만족
    private void CharacterEvent()
    {
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
                    _candidateDictionary[studentID].Add(new RandomEvent(data[j].eventId, data[j].cooldownTurn, data[j].eventPriority));
                }
            }
            Debug.Log($"------[1차 후보]------");
            foreach (var n in _candidateDictionary[studentID])
            {
                Debug.Log($"{i} - {n.EventId}");
            }
            Debug.Log($"----------------------");
        }
    }

    //2. 쿨타임 체크 및 우선순위 비교
    private void CooldownTurnCheck()
    {
        List<RandomEvent> events = new List<RandomEvent>();
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
            Debug.Log($"------[2차 후보]------");
            foreach (var n in events)
            {
                Debug.Log($"{i} - {n.EventId}");
            }
            Debug.Log($"----------------------");

            //우선순위가 같은 이벤트가 2개 이상이라면 그 중에 하나만 뽑기
            //Q. 우선순위가 같은 세개의 이벤트 중에 하나를 뽑았는데 뽑힌 이벤트가 발생확률이 낮으면 아무 이벤트도 발생하지 않을 수 있나?
            if (events.Count > 1)
            {
                int random = Random.Range(0, events.Count);
                Debug.Log($"------[결과 ]------");
                Debug.Log($"{i} - {events[random].EventId}");
                Debug.Log($"----------------------");
                PersonalityEvent(_myStudents[i], events[random]);
            }
            else if (events.Count == 1)
            {
                //이벤트발생 리스트에 넣기.
                Debug.Log($"------[결과 ]------");
                Debug.Log($"{i} - {events[0].EventId}");
                Debug.Log($"----------------------");
                PersonalityEvent(_myStudents[i], events[0]);
            }
        }
        debugQueue = new List<string>(_screenplayIdList);
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
        var script = _scriptSelectorReader.DataList;
        string eventId = selectedEvent.EventId;

        //eventId, 학생의 coreType이 같은 스크립트의 selectorId를 저장
        //_scriptSelectorReader개수만큼 반복
        for (int i = 0; i < script.Count; i++)
        {
            //결정된 이벤트 아이디와 같은 행 찾기
            //Debug.Log($"{student.StudentId} - 이벤트Id:{eventId} / 스크립트:{script[i].eventId} : {eventId.Equals(script[i].eventId)}");
            //Debug.Log($"{student.StudentId} - 학생 성격:{student.PersonalityData.core} / 스크립트:{script[i].selectCoreType} : {student.PersonalityData.core.Equals(script[i].selectCoreType)}");
            if (eventId.Equals(script[i].eventId))
            {
                if (student.PersonalityData.core.Equals(script[i].selectCoreType))
                {
                    _screenplayIdList.Enqueue(script[i].scriptId);
                    Debug.Log($"큐 추가 {student.StudentId} - {script[i].scriptId}");
                    break;
                }
            }
        }
    }
    //Context 로드
    //시스템 시작 시 캐릭터 테이블을 조회하여 캐릭터의 현재 스탯과 잠재력을 시스템 메모리에 캐싱
    //실시간으로 캐릭터 스탯에 가감연산을 수행함
    //student > Stat2pt / Stat3pt , 컨디션, 상태

    //EventManager
    //후보 필터링 : 요구 잠재력, 캐릭터의 현재 잠재력 , 쿨타임 체크 > 이벤트 선정
    //가중치 추첨 : 후보 목록 중에서 랜덤으로 이벤트 하나 선택
    //쿨타임관리 : 이벤트 종료 시 이벤트 쿨타임 기록


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
            if (!_eventScript.TryGetValue(currentId, out List<Event_ChoiceData> list))
            {
                list = new List<Event_ChoiceData>();
                _eventScript.Add(currentId, list);
            }
            list.Add(data[i]);
        }
        _debugList = new List<string>(_eventScript.Keys);
    }

    int next;
    List<Event_ChoiceData> screenPlayList;
    string eventId;

    //이벤트 시작할때마다 실행
    public void StartEvent()
    {
        //큐 순서대로 이벤트 진행
        //이벤트 아이디 가져오기
        eventId = _screenplayIdList.Dequeue();
        //아이디에 해당하는 리스트 가져오기

        Debug.Log($"리스트 불러오기{_eventScript.Count}");
        if (!_eventScript.TryGetValue(eventId, out var List))
        {
            Debug.LogWarning($"스크립트 없음 : {eventId}");
            return;
        }

        screenPlayList = List;
        next = 0;
    }
    

    public void OnClickContinue()
    {
        string script = "대사 불러오기 실패";

        //대본 순서대로 화면에 출력
        if( next < screenPlayList.Count)
        {
            switch (screenPlayList[next].textType)
            {
                case textType.Choice:
                    {
                        //언어에 따라서 다른 딕셔너리 선택해야 함
                        var stringTable = _eventString.KoScreenPlay;
                        string choice1 = stringTable[screenPlayList[next].choice01];
                        string choice2 = stringTable[screenPlayList[next].choice02];
                        string choice3 = stringTable[screenPlayList[next].choice03];
                        _eventUI.UpdateChiceText(choice1, choice2, choice3);
                    }
                    break;
                case textType.Desc:
                    {
                        script = _eventString.KoScreenPlay[screenPlayList[next].textKey];
                        _eventUI.UpdateText(screenPlayList[next].playerName, script);
                        next++;
                    }
                    break;
                case textType.End:
                    {
                        //텍스트는 출력, 버튼 누르면 결과 팝업 떠야 함.
                        //캐릭터 능력치 변동 적용
                        script = _eventString.KoScreenPlay[screenPlayList[next].textKey];
                        _eventUI.EventResult();
                    }
                    break;
            }
        }
    }
}
