using System;
using System.Xml.Schema;
using UnityEngine;

public class CalendarManager : Singleton<CalendarManager>
{
    public event Action OnWeekChanged;
    [SerializeField] private Calendar_TableDataReader _calReader;
    [SerializeField] private int _currentYear = 0;
    [SerializeField] private int _currentMonth;
    [SerializeField] private int _currentWeek;
    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        NextTurn();
    }

    private void NextTurn()
    {
        if (_calReader == null) return;

        var weekId = CalendarSaveData._weekId++;

        // 만약 전체 일정이 끝나게 된다면
        if (weekId >= _calReader.DataList.Count)
        {
            _currentYear++;
            weekId = 1;
        }

        // 1. 주차 시작(주차 계산)
        CalWeek(weekId);
        
        // 2. 시작 컷신 체크
        if (HasExistStartCutscene(weekId))
        {
            
        }
        
        // 3. 페이즈 체크
        CheckPhaseType(weekId);

        // 4. 종료 컷신 체크
        if (HasExistEndCutscene(weekId))
        {
            
        }

        // 5. 턴 종료 시
    }

    private void CalWeek(int weekId)
    {
        var data = _calReader.DataList[weekId];

        // 1. 특수 이동 유무 확인
        if (data.isSpecialWeek)
        {
            // 2. 시즌 아웃 조건 유무 확인
            if (data.hasSeasonOut)
            {
                // 경기 결과 정보 받기
            }
            else
            {
                weekId = data.targetidSpecial;
            }
        }
        else
        {
            weekId = data.targetidDefault;
        }

        CalendarSaveData._weekId = weekId;

        _currentMonth = data.month;
        _currentWeek = data.weekNo;
        OnWeekChanged?.Invoke();
    }

    public bool HasExistStartCutscene(int weekId) => _calReader.DataList[weekId].startCutscene.Equals("Yes");

    public bool HasExistEndCutscene(int weekId) => _calReader.DataList[weekId].endCutscene.Equals("Yes");
    public void CheckPhaseType(int weekId)
    {
        var curPhaseType = _calReader.DataList[weekId].phase;
        
        // 페이즈 타입에 따른 시스템 시작
        switch (curPhaseType)
        {
            case phaseType.Training:
                break;
            case phaseType.League:
                break;
            case phaseType.Event:
                break;
        }
    }
}

public static class CalendarSaveData
{
    private const string KEY_WEEK_ID = "WEEK_ID";

    public static int _weekId
    {
        get => PlayerPrefs.GetInt(KEY_WEEK_ID, 0);
        set
        {
            PlayerPrefs.SetInt(KEY_WEEK_ID, value);
            PlayerPrefs.Save();
        }
    }
}
