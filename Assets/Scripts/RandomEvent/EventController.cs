using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;

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

    [SerializeField] private List<Student> _myStudents;

    private int _nextId;
    private string _eventId;
    private string _currentStudentName = "";
    private Event_ResultData _selectedResultData;

    private Dictionary<string, string> _stringTable;
    private Dictionary<int, Event_ChoiceData> _screenPlayDic;
    string[] choice = new string[3];


    //주간 리포트 확인 버튼 클릭 시에 검사 실행됨
    public void EventCheck()
    {
        _eventManager = EventManager.Instance;
        _myStudents = StudentManager.Instance.MyStudents;
        _currentStudentNum = 0;

        _eventManager.CharacterEvent();
        _eventManager.CreateList();
        _eventSelector.EventSelect();

        Debug.Log($"ScreenplayIdList : {_eventSelector.ScreenplayIdList.Count}개");
        if (_eventSelector.ScreenplayIdList.Count > 0)
        {
            _eventPanel.SetActive(true);
            StartEvent();
        }
        //이벤트 스타트 조건 > 이벤트리스트큐가 1개 이상일 때
    }

    //스크립트 id별 대본 리스트 생성
    //예)아이디가 'a'인 행만 모인 리스트 

    public void StartEvent()
    {
        //큐 순서대로 이벤트 진행
        //이벤트 아이디 가져오기
        _eventId = _eventSelector.ScreenplayIdList.Dequeue();
        //아이디에 해당하는 리스트 가져오기

        Debug.Log($"딕셔너리 불러오기{_eventManager.EventScript.Count}");
        if (!_eventManager.EventScript.TryGetValue(_eventId, out var Dic))
        {
            Debug.LogWarning($"스크립트 없음 : {_eventId}, 다음 학생");
            _currentStudentNum++;
            if (_eventSelector.ScreenplayIdList.Count > 0)
            {
                StartEvent();
            }

            return;
        }

        StringManager manager = StringManager.Instance;
        string nameSet =
            manager.GetString(_myStudents[_currentStudentNum].Name[0]) +
            manager.GetString(_myStudents[_currentStudentNum].Name[1]) +
            manager.GetString(_myStudents[_currentStudentNum].Name[2]);
        _currentStudentName = nameSet;

        _screenPlayDic = Dic;
        _nextId = _screenPlayDic[1].currentId;
        Debug.Log($"시작 이벤트 id : {_screenPlayDic[1].scriptId}");
    }
   

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
                        script = _stringTable[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentStudentName, script, _screenPlayDic[_nextId].speakDirection);

                        choice[0] = _stringTable[_screenPlayDic[_nextId].choice01];
                        choice[1] = _stringTable[_screenPlayDic[_nextId].choice02];
                        choice[2] = _stringTable[_screenPlayDic[_nextId].choice03];
                        _eventUI.UpdateChiceText(choice[0], choice[1], choice[2]);
                    }
                    break;
                case textType.Desc:
                    {
                        script = _stringTable[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentStudentName, script, _screenPlayDic[_nextId].speakDirection);
                        _nextId++;
                    }
                    break;
                case textType.End:
                    {
                        //텍스트는 출력, 버튼 누르면 결과 팝업 떠야 함.
                        script = _eventString.KoScreenPlay[_screenPlayDic[_nextId].textKey];
                        _eventUI.UpdateText(_currentStudentName, script, _screenPlayDic[_nextId].speakDirection);

                        //캐릭터 능력치 변동 적용
                        ResultCalculator();
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
                //코어성격타입 조건 체크
                if (_myStudents[_currentStudentNum].PersonalityData.personality == value[i].matchPersonalityId)
                {
                    //다음 아이디 가져오기
                    _nextId = value[i].nextId;
                    _selectedResultData = value[i];
                    Debug.Log($"resultScriptKey : {_selectedResultData.resultScriptKey}");
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

    public void OnClickOkButton()
    {
        _eventUI.ImageInit();
        _eventUI.TextInit();
        //+인스펙터에 ResultPanel 닫기 연결해둠

        //다음 학생, 다음 이벤트 없으면 패널 닫기
        //학생 있으면 다음 학생 진행
        if (_eventSelector.ScreenplayIdList.Count < 1)
        {
            Debug.Log($"패널 닫기");
            _eventPanel.SetActive(false);
        }
        else
        {
            Debug.Log($"다음 이벤트 시작");
            StartEvent();
        }
    }

    


    private void ResultCalculator()
    {
        Debug.Log($"resultScriptKey : {_selectedResultData.resultScriptKey}");
        //resultData 딕셔너리 필요
        _eventUI.UpdateEventResult(
            _selectedResultData.statusChange, //상태변경
            _selectedResultData.potentialChangeType, //잠재력
            _selectedResultData.potentialChangeValue, //잠재력 값
            _stringTable[_selectedResultData.resultScriptKey], //결과텍스트
            _selectedResultData.reactionPortraitId);

        //선수 컨디션 변경
        var beforeConditon = _myStudents[_currentStudentNum].Condition.ToString();
        _myStudents[_currentStudentNum].ChangeCondition(_selectedResultData.conditionChange);
        Debug.Log($"학생 ID : {_myStudents[_currentStudentNum].StudentId}" +
            $"컨디션 {beforeConditon} > {_myStudents[_currentStudentNum].Condition}");

        //선수 상태 변경
        var beforeState = _myStudents[_currentStudentNum].State.ToString();
        string getValue = _selectedResultData.statusChange;
        StudentState value = (StudentState)System.Enum.Parse(typeof(StudentState), getValue);
        _myStudents[_currentStudentNum].ChangeState(value);
        Debug.Log($"학생 ID : {_myStudents[_currentStudentNum].StudentId}" +
            $"상태 {beforeState} > {_myStudents[_currentStudentNum].State.ToString()}");


        //능력치 변경
        var beforeStat = _myStudents[_currentStudentNum].GetStat(_selectedResultData.potentialChangeType).Current;
        _myStudents[_currentStudentNum].GetStat(_selectedResultData.potentialChangeType).GrowAndReturn(_selectedResultData.potentialChangeValue);
        Debug.Log($"학생 ID : {_myStudents[_currentStudentNum].StudentId}" +
            $"스탯 {_selectedResultData.potentialChangeType.ToString()} : {beforeStat} > {_myStudents[_currentStudentNum].GetStat(_selectedResultData.potentialChangeType).Current}");


        Debug.Log($"다음 학생 ID : {_myStudents[_currentStudentNum].StudentId + 1}");
        _currentStudentNum++;

        //다음 학생으로
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

}
