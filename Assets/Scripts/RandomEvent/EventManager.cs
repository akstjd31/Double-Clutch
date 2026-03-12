using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LightTransport;

[System.Serializable]
public class EventManager : MonoBehaviour
{
    [SerializeField] private Event_DataModelReader _dataModelReader;
    [SerializeField] private Event_ScriptSelectorReader _scriptSelectorReader;
    [SerializeField] private Event_ChoiceDataReader _choiceDataReader;


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
    Dictionary<string, Dictionary<int,Event_ChoiceData>> _eventScript = new();
    [Header("대본 딕셔너리 아이디 리스트 - 전체 이벤트 개수를 의미함")]
    [SerializeField] private List<string> _eventScriptDebugList;

    //<선택지 id, ResultData>를 저장한 딕셔너리
    [Header("Result 딕셔너리 아이디 리스트 - 전체 ID개수를 의미함")]
    [SerializeField] private List<string> _resultDataDebugList;

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
            string cId = data[i].scriptId;

            //ID 키값으로 딕셔너리가 없으면 새로 생성
            if (!_eventScript.TryGetValue(cId, out Dictionary<int, Event_ChoiceData> chDic))
            {
                chDic = new();
                _eventScript.Add(cId, chDic);
            }
            chDic.Add(data[i].currentId, data[i]);
        }
        _eventScriptDebugList = new List<string>(_eventScript.Keys);
    }

    int _nextId;
    Dictionary<int, Event_ChoiceData> _screenPlayDic;
    string _eventId;
    int _currentStudentNum = 0;
    string _currentStudentName = "";

    Event_ResultData _selectedResultData;

    //이벤트 시작할때마다 실행
    public void StartEvent()
    {
        //큐 순서대로 이벤트 진행
        //이벤트 아이디 가져오기
        _eventId = _screenplayIdList.Dequeue();
        //아이디에 해당하는 리스트 가져오기

        Debug.Log($"딕셔너리 불러오기{_eventScript.Count}");
        if (!_eventScript.TryGetValue(_eventId, out var Dic))
        {
            Debug.LogWarning($"스크립트 없음 : {_eventId}");
            return;
        }

        _currentStudentNum = 0;
        _currentStudentName = _myStudents[_currentStudentNum].Name[0] + _myStudents[_currentStudentNum].Name[1] + _myStudents[_currentStudentNum].Name[2];

        //버튼으로 테스트 시에 사용
        //_currentStudentNum++;

        _screenPlayDic = Dic;
        _nextId = _screenPlayDic[1].currentId;
        Debug.Log($"시작 이벤트 id : {_screenPlayDic[1].scriptId}");
    }

    Dictionary<string, string> stringTable;

    private void ScreenPlayLanguage()
    {
        switch (StringManager.Instance.CurrentLanguage)
        {
            case Language.Ko:
                stringTable = _eventString.KoScreenPlay;
                break;
            case Language.En:
                stringTable = _eventString.EnScreenPlay;
                break;
            case Language.Ja:
                stringTable = _eventString.JaScreenPlay;
                break;
        }
    }

    string[] choice = new string[3];

    public void OnClickContinue()
    {
        string script = "대사 불러오기 실패";

        ScreenPlayLanguage();
        Debug.Log($"다음 대사 ID : {_nextId}");

        //대본 순서대로 화면에 출력
        if (_screenPlayDic.ContainsKey(_nextId))
        {
            switch (_screenPlayDic[_nextId].textType)
            {
                case textType.Choice:
                    {
                        //언어에 따라서 다른 딕셔너리 선택해야 함
                        script = _eventString.KoScreenPlay[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentStudentName, script, _screenPlayDic[_nextId].speakDirection);

                        choice[0] = stringTable[_screenPlayDic[_nextId].choice01];
                        choice[1] = stringTable[_screenPlayDic[_nextId].choice02];
                        choice[2] = stringTable[_screenPlayDic[_nextId].choice03];
                        _eventUI.UpdateChiceText(choice[0], choice[1], choice[2]);
                    }
                    break;
                case textType.Desc:
                    {
                        script = _eventString.KoScreenPlay[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentStudentName, script, _screenPlayDic[_nextId].speakDirection);
                        _nextId++;
                    }
                    break;
                case textType.End:
                    {
                        //텍스트는 출력, 버튼 누르면 결과 팝업 떠야 함.
                        //캐릭터 능력치 변동 적용
                        script = _eventString.KoScreenPlay[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentStudentName, script, _screenPlayDic[_nextId].speakDirection);
                        ResultCalculator();
                        //다음 학생으로
                        _currentStudentNum++;
                    }
                    break;
            }
        }
        else
        {
            Debug.Log($"해당 id없음");
        }
    }

    public void OnClickChoice(int choiceNum)
    {
        var resultDic = _eventString.ResultData;
        string selectedResultId = "";

        switch (choiceNum)
        {
            case 0:
                {
                    selectedResultId = _screenPlayDic[_nextId].choice01;
                }
                break;
            case 1:
                {
                    selectedResultId = _screenPlayDic[_nextId].choice02;
                }
                break;
            case 2:
                {
                    selectedResultId = _screenPlayDic[_nextId].choice03;
                }
                break;
        }

        Debug.Log($"선택지 id : {_resultDataDebugList.Count}");
        _resultDataDebugList = new List<string>(resultDic.Keys);
        Debug.Log($"선택지 id : {selectedResultId}");

        //resultData에서 nextId값 받아오기
        if (resultDic.TryGetValue(selectedResultId, out var value))
        {
            for(int i = 0; i < value.Count; i++)
            {
                //코어성격타입 조건 체크
                if (_myStudents[_currentStudentNum].PersonalityData.personality == value[i].matchPersonalityId)
                {
                    //다음 아이디 가져오기
                    _nextId = value[i].nextId;
                    _selectedResultData = value[i];
                    break;
                }
                else
                {
                    Debug.Log($"해당 성격을 가진 이벤트 없음");
                }
            }

            Debug.Log($"다음 대사 ID : {_nextId}");
            //선택한 대사 미리 넣어두기
            _eventUI.UpdateText("", choice[choiceNum], "");
            //다음 대사로 넘어 가기
            OnClickContinue();
        }
        else
        {
            Debug.Log($"선택지 이후 대사 불러오기 실패..");
        }
    }

    private void ResultCalculator()
    {
        //resultData 딕셔너리 필요
        _eventUI.UpdateEventResult(_selectedResultData.statusChange, _selectedResultData.potentialChangeType, _selectedResultData.potentialChangeValue, _selectedResultData.resultscriptKey,_selectedResultData.reactionPortraitId);

        //선수 컨디션 변경
        _myStudents[_currentStudentNum].ChangeCondition(_selectedResultData.conditionChange);
        
        //선수 상태 변경
        string getValue = _selectedResultData.statusChange;
        StudentState value = (StudentState)System.Enum.Parse(typeof(StudentState), getValue);
        _myStudents[_currentStudentNum].ChangeState(value);

        //능력치 변경
        //매서드 만들어주시면 추가
    }

    public void OnClickOkButton()
    {
        //다음 학생 없으면 로비씬, 다음 이벤트 없으면 로비씬.. 같은거임
        //학생 있으면 다음 학생 진행
        if(_screenplayIdList.Count < 1)
        {
            Debug.Log($"로비씬으로");
        }
        else
        {
            Debug.Log($"다음 이벤트 시작");
            StartEvent();
        }
    }

}
