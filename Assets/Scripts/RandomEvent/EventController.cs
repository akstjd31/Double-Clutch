using System.Collections.Generic;
using Game.Constants;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class EventController : MonoBehaviour
{
    [SerializeField] private Event_ChoiceDataReader _choiceDataReader;

    [Header("스크립트/오브젝트")]
    [SerializeField] private GameObject _eventPanel;
    [SerializeField] private EventUI _eventUI;
    [SerializeField] private EventString _eventString;
    [SerializeField] private EventSelector _eventSelector;

    private EventManager _eventManager;

    [Header("순서")]
    [SerializeField] int _currentStudentNum;
    private Queue<int> _studentTurnQueue;

    [Header("학생 순서")]
    [SerializeField] private List<int> _debugList_studentidList;


    [SerializeField] private List<Student> _myStudents;

    [Header("resultData Dic List")]
    [SerializeField] private List<string> _debugResultDataList;

    [SerializeField] private int _nextId;


    //현재 진행중인 이벤트 아이디
    private string _eventId;
    private string _currentSpeakerName = "";
    private Event_ResultData _selectedResultData;

    private Dictionary<string, string> _stringTable;
    private Dictionary<int, Event_ChoiceData> _screenPlayDic;
    string[] choice = new string[3];

    string _visualId = "이미지 리소스 못불러옴";
    string _speakerImageColomn = "";


    //주간 리포트 확인 버튼 클릭 시에 검사 실행됨
    public void EventCheck()
    {
        _eventManager = EventManager.Instance;
        _myStudents = StudentManager.Instance.MyStudents;

        _eventManager.LoadGame();

        _eventString.Init();
        if (_eventString.ResultData.Count < 1)
        {
            Debug.Log($"string 다시 읽어오기");
            _eventString.SaveEvent();
        }
        _debugResultDataList = new(_eventString.ResultData.Keys);

        _eventManager.CharacterEvent();
        _eventManager.CreateList();
        _eventSelector.EventSelect();

        _studentTurnQueue = _eventSelector.StudentidQueue;
        _debugList_studentidList = new List<int>(_studentTurnQueue);

        _eventUI.UIStart();

        //이벤트가 0개 이상이면 이벤트 실행
        Debug.Log($"ScreenplayIdList : {_eventSelector.ScreenplayIdList.Count}개");
        if (_eventSelector.ScreenplayIdList.Count > 0)
        {
            _currentStudentNum = _studentTurnQueue.Dequeue();
            _eventPanel.SetActive(true);
            StartEvent();
        }
        //이벤트 스타트 조건 > 이벤트리스트큐가 1개 이상일 때
    }

    //스크립트 id별 대본 리스트 생성
    //예)아이디가 'a'인 행만 모인 리스트 

    public void StartEvent()
    {
        Debug.Log($"랜덤이벤트 시작");

        

        if (_eventSelector.ScreenplayIdList.Count < 1)
        {
            Debug.LogWarning($"남은 이벤트 없음. 이벤트 종료");
            _eventPanel.SetActive(false);
            return;
        }
        //큐 순서대로 이벤트 진행
        //이벤트 아이디 가져오기
        _eventId = _eventSelector.ScreenplayIdList.Dequeue();
        //아이디에 해당하는 리스트 가져오기

        Debug.Log($"딕셔너리 불러오기{_eventManager.EventScript.Count}");
        Debug.Log($"이번 스크립트 : {_eventId}, 학생 {_currentStudentNum}의 이벤트");
        if (!_eventManager.EventScript.TryGetValue(_eventId, out var Dic))
        {
            _currentStudentNum = _studentTurnQueue.Dequeue();
            Debug.LogWarning($"스크립트 없음 : {_eventId}, 다음 학생 {_currentStudentNum}");
            StartEvent();
            return;
        }


        
        _screenPlayDic = Dic;
        _nextId = _screenPlayDic[1].currentId;
        Debug.Log($"시작 이벤트 id : {_screenPlayDic[1].scriptId}");

        OnClickContinue();
    }
   
    
    private void ReadName()
    {
        StringManager manager = StringManager.Instance;
        Debug.Log($"시트 이름 : {_screenPlayDic[_nextId].playerName}");
        if (_screenPlayDic[_nextId].playerName == "{TName}")
        {
            string nameSet =
                        manager.GetString(CurrentStudent(_currentStudentNum).Name[0]) +
                        manager.GetString(CurrentStudent(_currentStudentNum).Name[1]) +
                        manager.GetString(CurrentStudent(_currentStudentNum).Name[2]);
            _currentSpeakerName = nameSet;
        }
        else if (_screenPlayDic[_nextId].playerName == "{ME}")
        {
            _currentSpeakerName = GameManager.Instance.SaveData.coachName;
        }
        else
        {
            _currentSpeakerName = "";
        }
    }

    string direction;
    public void OnClickContinue()
    {
        string script = "대사 불러오기 실패";

        ReadName();

        Debug.Log($"스피커 이름 : {_currentSpeakerName}");

        ScreenPlayLanguage();

        Debug.Log($"다음 대사 ID : {_nextId}");

        //아이디에서 감정이미지 아이디 가져오기 Image_Resource_H1
        //학생의 스프라이트 이미지

        direction = _screenPlayDic[_nextId].speakDirection;
        Debug.Log($"스피커 위치 : {direction}");
        

        //스텐딩 이미지 소스 playerImageResource
        switch (direction)
        {
            case "Left":
                _speakerImageColomn = _screenPlayDic[_nextId].standingLeft;
                Debug.Log($"컬럼명 : {_screenPlayDic[_nextId].standingLeft}");
                break;
            case "Middle":
                _speakerImageColomn = _screenPlayDic[_nextId].standingMiddle;
                Debug.Log($"컬럼명 : {_screenPlayDic[_nextId].standingMiddle}");
                break;
            case "Right":
                _speakerImageColomn = _screenPlayDic[_nextId].standingRight;
                Debug.Log($"컬럼명 : {_screenPlayDic[_nextId].standingRight}");
                break;
            default:
                break;
        }

        Player_VisualData visualData = CurrentStudent(_currentStudentNum).VisualData;

        var field = visualData.GetType().GetField(_speakerImageColomn);

        if (field != null)
        {
            _visualId = field.GetValue(visualData).ToString();
            Debug.Log($"컬럼 입력됨 {_visualId}");
        }
        else
        {
            Debug.Log($"컬럼 없음, 입력안됨");
        }

        //대본 순서대로 화면에 출력
        if (_screenPlayDic.ContainsKey(_nextId))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySoundOneShot(SoundName.SE_TEXT_SCROLL);

            switch (_screenPlayDic[_nextId].textType)
            {
                case textType.Choice:
                    {
                        //언어에 따라서 다른 딕셔너리 선택해야 함

                        script = _stringTable[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentSpeakerName, script, direction, false, _visualId);

                        choice[0] = _stringTable[_screenPlayDic[_nextId].choice01];
                        choice[1] = _stringTable[_screenPlayDic[_nextId].choice02];
                        choice[2] = _stringTable[_screenPlayDic[_nextId].choice03];
                        _eventUI.UpdateChiceText(choice[0], choice[1], choice[2]);
                    }
                    break;
                case textType.Desc:
                    {
                        script = _stringTable[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentSpeakerName, script, direction, false, _visualId);
                        _nextId++;
                    }
                    break;
                case textType.End:
                    {
                        //텍스트는 출력, 버튼 누르면 결과 팝업 떠야 함.
                        script = _eventString.KoScreenPlay[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentSpeakerName, script, direction, true, _visualId);

                        //캐릭터 능력치 변동 적용
                        ResultCalculator();
                    }
                    break;
                default:
                    {
                        Debug.Log($"string Table에 없음");
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

        //Debug.Log($"선택지 id : {_eventSelector.ResultDataDebugList.Count}");
        //_resultDataDebugList = new List<string>(resultDic.Keys);
        //Debug.Log($"선택지 id : {selectedResultId}");

        //resultData에서 nextId값 받아오기
        

        if (resultDic.TryGetValue(selectedResultId, out var value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                Debug.Log($"{CurrentStudent(_currentStudentNum).PersonalityData.personality} == {value[i].matchPersonalityId}");
                //코어성격타입 조건 체크
                if (CurrentStudent(_currentStudentNum).PersonalityData.personality == value[i].matchPersonalityId)
                {
                    //다음 아이디 가져오기
                    _nextId = value[i].nextId;
                    _selectedResultData = value[i];
                    Debug.Log($"resultScriptKey : {_selectedResultData.resultScriptKey}");
                    break;
                }
                else
                {
                    Debug.Log($"{selectedResultId}");
                    Debug.Log($"해당 성격을 가진 이벤트 없음");
                    Debug.Log($"선수 번호가 맞지 않음");
                }
            }

            Debug.Log($"다음 대사 ID : {_nextId}");
            //선택한 대사 미리 넣어두기
            _eventUI.UpdateText(_currentSpeakerName, choice[choiceNum], direction, false, _visualId);
            //다음 대사로 넘어 가기
            OnClickContinue();
        }
        else
        {
            Debug.Log($"선택지 이후 대사 불러오기 실패..");
            Debug.Log($"selectedResultId : {selectedResultId}");
            foreach (var key in resultDic.Keys)
            {
                Debug.Log($"resultDic key : {key}");
            }
        }
    }

    public void OnClickOkButton()
    {
        _eventUI.ImageInit();
        _eventUI.TextInit();
        //+인스펙터에 ResultPanel 닫기 연결해둠

        //다음 학생, 다음 이벤트 없으면 패널 닫기
        //학생 있으면 다음 학생 진행
        Debug.Log($"패널 닫기");
        _eventPanel.SetActive(false);

        if (_eventSelector.ScreenplayIdList.Count > 0)
        { 
            _currentStudentNum = _studentTurnQueue.Dequeue();
            //다음 학생으로
            Debug.Log($"다음 이벤트 시작");
            _eventPanel.SetActive(true);
            StartEvent();
        }
        _eventManager.SaveGame();
    }

    


    private void ResultCalculator()
    {
        Debug.Log($"resultScriptKey : {_selectedResultData.resultScriptKey}");

        //resultData 딕셔너리 필요
        _eventUI.UpdateEventResult(
            _selectedResultData.potentialChangeType, //잠재력
            _selectedResultData.potentialChangeValue, //잠재력 값
            _stringTable[_selectedResultData.resultScriptKey], //결과텍스트
            _selectedResultData.reactionPortraitId,//이미지
            _selectedResultData.statusChange, //현재 상태
            CurrentStudent(_currentStudentNum).State.ToString()); //변할 상태

        //선수 컨디션 변경
        var beforeConditon = CurrentStudent(_currentStudentNum).Condition.ToString();
        CurrentStudent(_currentStudentNum).ChangeCondition(_selectedResultData.conditionChange);
        Debug.Log($"학생 ID : {CurrentStudent(_currentStudentNum).StudentId}" +
            $"컨디션 {beforeConditon} > {CurrentStudent(_currentStudentNum).Condition}");

        //선수 상태 변경
        var beforeState = CurrentStudent(_currentStudentNum).State;
        string getValue = _selectedResultData.statusChange;
        //StudentState value = (StudentState)System.Enum.Parse(typeof(StudentState), getValue);
        
        switch(getValue)
        {
            case "None": //변화 없음
                {
                    CurrentStudent(_currentStudentNum).ChangeState(beforeState);
                }
                break;
            case "Injured": //부상상태로 전환
                {
                    CurrentStudent(_currentStudentNum).ChangeState(StudentState.Injured);
                }
                break;
            case "OverWorked": //과로상태로 전환
                {
                    CurrentStudent(_currentStudentNum).ChangeState(StudentState.OverWorked);
                }
                break;
        }

        
        Debug.Log($"학생 ID : {CurrentStudent(_currentStudentNum).StudentId}" +
            $"상태 {beforeState} > {CurrentStudent(_currentStudentNum).State.ToString()}");



        //선수 스탯 변동치 초기화
        CurrentStudent(_currentStudentNum).PrepareStatChange();

        //선수 컨디션 변경
        var beforeCondi = CurrentStudent(_currentStudentNum).Condition.ToString(); //디버그용 변수
        CurrentStudent(_currentStudentNum).ChangeCondition(_selectedResultData.conditionChange);

        Debug.Log($"학생 ID : {CurrentStudent(_currentStudentNum).StudentId}" +
            $"컨디션 {beforeCondi} > {CurrentStudent(_currentStudentNum).Condition}");



        //능력치 변경
        var beforeStat = CurrentStudent(_currentStudentNum).GetStat(_selectedResultData.potentialChangeType).Current;
        CurrentStudent(_currentStudentNum).GetStat(_selectedResultData.potentialChangeType).GrowAndReturn(_selectedResultData.potentialChangeValue);
        Debug.Log($"학생 ID : {CurrentStudent(_currentStudentNum).StudentId}" +
            $"스탯 {_selectedResultData.potentialChangeType.ToString()} : {beforeStat} > {CurrentStudent(_currentStudentNum).GetStat(_selectedResultData.potentialChangeType).Current}");


        Debug.Log($"다음 학생 ID : {CurrentStudent(_currentStudentNum).StudentId + 1}");

        CurrentStudent(_currentStudentNum).OnStatChanged();

        CurrentStudent(_currentStudentNum).OnStatChanged();

        //현재 학생의 이벤트 리스트를 가져오기
        List<RandomEvent> studentEventList = _eventManager.CandidateDictionary[CurrentStudent(_currentStudentNum).StudentId];

        for (int i = 0; i < studentEventList.Count; i++)
        {

            string evnetNumber = $"{studentEventList[i].EventId}_{CurrentStudent(_currentStudentNum).PersonalityData.core.ToString()}";

            Debug.Log($"{_eventId}에서 {studentEventList[i].EventId} 검색");
            Debug.Log($"{evnetNumber == _eventId}");

            //진행했던 이벤트 아이디를 찾아 쿨다운모드로 변경
            if (evnetNumber == _eventId)
            {
                studentEventList[i].WaitingMode();
                return;
            }
        }
    }

    public void ScreenPlayLanguage()
    {
        switch (StringManager.Instance.CurrentLanguage)
        {
            case Language.Ko:
                _stringTable = _eventString.KoScreenPlay;
                break;
            case Language.En:
                _stringTable = _eventString.EnScreenPlay;
                break;
            case Language.Ja:
                _stringTable = _eventString.JaScreenPlay;
                break;
        }
    }

    private Student CurrentStudent(int currentStudentId)
    {
        Student student = null;
        for(int i = 0; i < _myStudents.Count; i++)
        {
            if (_myStudents[i].StudentId == currentStudentId)
            {
                student = _myStudents[i];
            }
        }
        return student;
    }
}
