using System;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.PlayerLoop;

public struct Calendar
{
    public int year;
    public int month;
    public int week;

    public Calendar(int y = 0, int m = 0, int w = 0)
    {
        year = y;
        month = m;
        week = w;
    }
}

public class CalendarManager : Singleton<CalendarManager>
{
    public event Action<Calendar> OnWeekChanged;
    [SerializeField] private Calendar_TableDataReader _calReader;
    Calendar calendar;
    public bool IsEndPhase { get; set; } = false;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.SaveData == null) return;

        int weekId = GameManager.Instance.SaveData.weekId;
        var data = _calReader.DataList[weekId - 1];

        calendar.month = data.month;
        calendar.week = data.weekNo;

        OnWeekChanged?.Invoke(calendar);
    }

    public void NextTurn()
    {
        if (_calReader == null) return;
        if (GameManager.Instance == null) return;

        // 임시 테스트용
        var weekId = GameManager.Instance.SaveData.weekId;

        // 만약 전체 일정이 끝나게 된다면
        if (weekId >= _calReader.DataList.Count)
        {
            calendar.year++;
            weekId = 0;
        }

        // 1. 주차 시작(주차 계산)
        CalcWeek(weekId);

        // 2. 시작 컷신 체크
        if (HasExistStartCutscene(weekId))
        {

        }

        // 3. 페이즈 체크
        if (!CheckPhaseType(weekId)) return;

        // 4. 종료 컷신 체크
        if (HasExistEndCutscene(weekId))
        {

        }

        // 5. 턴 종료 시
        Init();
    }

    private void Init()
    {
        IsEndPhase = false;
    }

    public void CalcWeek(int weekId)
    {
        var data = _calReader.DataList[weekId - 1];

        if (PlayerPrefs.GetInt(PrefKeys.KEY_FIRST_RUN_DONE) == 0)
        {
            // 튜토리얼 수행 완료
            PlayerPrefs.SetInt(PrefKeys.KEY_FIRST_RUN_DONE, 1);
            PlayerPrefs.Save();
        }
        else
        {
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

            data = _calReader.DataList[weekId - 1];
        }

        calendar.month = data.month;
        calendar.week = data.weekNo;

        GameManager.Instance.SetWeekId(weekId);

        OnWeekChanged?.Invoke(calendar);
    }

    // 일단 컷신 부분은 패스(-)
    public bool HasExistStartCutscene(int weekId) => _calReader.DataList[weekId - 1].startCutscene.Equals("");

    public bool HasExistEndCutscene(int weekId) => _calReader.DataList[weekId - 1].endCutscene.Equals("");
    public bool CheckPhaseType(int weekId)
    {
        // var curPhaseType = _calReader.DataList[weekId].phase;

        // 페이즈 타입에 따른 시스템 시작
        // switch (curPhaseType)
        // {
        //     case phaseType.Training:

        //         break;
        //     case phaseType.League:
        //         break;
        //     case phaseType.Event:
        //         break;
        // }

        return IsEndPhase;
    }

    public Calendar GetCalendar() => this.calendar;
    public phaseType CurrentGetPhaseType()
    {
        return _calReader.DataList[GameManager.Instance.SaveData.weekId - 1].phase;
    }
}
