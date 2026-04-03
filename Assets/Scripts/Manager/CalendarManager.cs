using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Calendar
{
    public int month;
    public int week;

    public Calendar(int m = 0, int w = 0)
    {
        month = m;
        week = w;
    }
}

// 달마다 있는 이 주차는 고정이기 때문에 이렇게 정의
public static class MonthWeekTable
{
    public static readonly int[] weekCounts =
    {
        4,
        4,
        5,
        4,
        4,
        4,
        5,
        4,
        4,
        5,
        4,
        5
    };
}

public class CalendarManager : Singleton<CalendarManager>
{
    private const int SALARY = 50000;

    public event Action<Calendar> OnWeekChanged;

    [SerializeField] private Calendar_TableDataReader _calReader;

    private Calendar calendar;
    public bool IsEndPhase { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.SaveData == null) return;
        if (_calReader == null || _calReader.DataList == null || _calReader.DataList.Count == 0) return;

        int weekId = GameManager.Instance.SaveData.weekId;
        var data = _calReader.DataList[weekId - 1];

        calendar.month = data.month;
        calendar.week = data.weekNo;

        OnWeekChanged?.Invoke(calendar);
    }

    public void NextTurn()
    {
        if (_calReader == null || _calReader.DataList == null || _calReader.DataList.Count == 0)
            return;

        var gm = GameManager.Instance;
        if (gm == null || gm.SaveData == null)
            return;

        // 1차 주차 진행
        AdvanceToNextWeek(gm);

        int currentWeekId = gm.SaveData.weekId;

        // weekId == 1 : 엔딩 조건 확인
        if (currentWeekId == 1)
        {
            if (CanGoEnding())
            {
                gm.GoToEnding();
                return;
            }
        }

        // 주차별 특수 처리
        HandleSpecialWeekEvent(currentWeekId, gm);
    }

    /// <summary>
    /// 실제 주차를 1회 진행시키는 함수
    /// </summary>
    public void AdvanceToNextWeek(GameManager gm)
    {
        if (gm == null || gm.SaveData == null) return;

        int weekId = gm.SaveData.weekId;

        // 1. 주차 계산
        CalcWeek(weekId, gm);

        // 주차 종료 후 이벤트 쿨다운
        if (EventManager.Instance != null)
            EventManager.Instance.WeekendCooldown();

        int nextWeekId = gm.SaveData.weekId;

        // 2. 시작 컷신 체크
        if (HasExistStartCutscene(nextWeekId))
        {
            // TODO: 시작 컷신 실행
        }

        // 3. 종료 컷신 체크
        if (HasExistEndCutscene(nextWeekId))
        {
            // TODO: 종료 컷신 실행
        }

        // 4. 튜토리얼 체크 (첫 해만)
        if (gm.SaveData.year == 1)
        {
            var tId = GetTutorialId(nextWeekId - 1);
            if (tId != null)
            {
                Debug.Log("튜토리얼 Id 체크 완료!");

                var tutorialMgr = TutorialManager.Instance;
                if (tutorialMgr == null) return;

                int idx = int.Parse(tId[tId.Length - 1].ToString()) - 1;
                if (idx >= 0 && idx < gm.SaveData.tutorialCompleted.Length)
                {
                    if (!gm.SaveData.tutorialCompleted[idx])
                    {
                        tutorialMgr.StartTutorial(tId);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 주차별 특수 이벤트 처리
    /// </summary>
    private void HandleSpecialWeekEvent(int weekId, GameManager gm)
    {
        if (gm == null || gm.SaveData == null) return;

        if (weekId == 8)
        {
            gm.GoToGraduation();
            return;
        }

        if (weekId == 9)
        {
            var data = gm.SaveData;
            bool flag = false;
            foreach (var hasComplete in data.tutorialCompleted)
            {
                if (!hasComplete)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                gm.SetYear(data.year + 1);
            return;
        }
    }

    /// <summary>
    /// 엔딩 진입 조건 판정만 담당
    /// </summary>
    private bool CanGoEnding()
    {
        var manager = GameManager.Instance;
        if (manager == null || manager.SaveData == null) return false;
        if (manager.SaveData.hasEnding) return false;
        if (LeagueDataManager.Instance == null) return false;

        var winRecord = manager.SaveData.leagueWinRecord;
        int requireCount = LeagueDataManager.Instance.GetEndingRequireNumber();

        return winRecord.Count == requireCount && winRecord.All(record => record.hasWon);
    }

    public void CalcWeek(int weekId, GameManager gm)
    {
        if (gm == null) return;
        if (_calReader == null || _calReader.DataList == null || _calReader.DataList.Count == 0) return;
        if (weekId <= 0 || weekId > _calReader.DataList.Count) return;

        var data = _calReader.DataList[weekId - 1];

        // 1. 특수 이동 유무 확인
        if (data.isSpecialWeek)
        {
            // 2. 시즌 아웃 조건 유무 확인
            if (data.hasSeasonOut)
            {
                weekId = LeagueManager.Instance.IsPlayerSeasonOut()
                    ? data.targetidSpecial
                    : data.targetidDefault;
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

        if (weekId <= 0 || weekId > _calReader.DataList.Count) return;

        data = _calReader.DataList[weekId - 1];

        // 시즌아웃 시 달 차이만큼 지원금 누적
        int accSub = data.hasSeasonOut
            ? (data.month - calendar.month + 12) % 12
            : 1;

        calendar.month = data.month;
        calendar.week = data.weekNo;

        if (IsFundingDay())
        {
            var myData = gm.SaveData;
            bool flag = false;
            foreach (var hasComplete in myData.tutorialCompleted)
            {
                if (!hasComplete)
                {
                    flag = true;
                    break;
                }
            }

            // 튜토리얼 완료 여부 (아직 완료가 안되어있다면 첫 달이라는 얘기)
            if (!flag)
            {
                int money = gm.SaveData.money;
                gm.SetMoney(money + (SALARY * accSub));
            }

            if (calendar.month == 3)
                gm.ClearLeagueWinData();
        }

        gm.SetWeekId(weekId);

        var leagueDataMgr = LeagueDataManager.Instance;
        if (leagueDataMgr != null && !CheckEventDay(weekId))
        {
            string leagueId = GetLeagueIdByWeekId(weekId);

            if (!string.IsNullOrEmpty(leagueId) && leagueId != "-")
            {
                var masterData = leagueDataMgr.GetMasterDataById(leagueId);

                if (!masterData.HasValue)
                {
                    Debug.LogError($"<color=red>리그 마스터 테이블에서 '{leagueId}'를 찾을 수 없습니다! 엑셀 파일에 오타나 띄어쓰기가 있는지 확인하세요.</color>");
                }
                else
                {
                    if (masterData.Value.isSelectionRequired)
                    {
                        string ruleId = masterData.Value.teamSelectionRuleId;
                        var selectionData = leagueDataMgr.GetTeamSelectionRuleById(ruleId);

                        if (selectionData != null)
                        {
                            var newLeague = LeagueDataManager.Instance.CreateAndSaveLeague(leagueId, selectionData);

                            if (newLeague != null)
                                Debug.Log($"<color=green>[{leagueId}] 새로운 리그 생성 완벽하게 성공!</color> (적용된 룰: {ruleId})");
                            else
                                Debug.LogError($"<color=red>[{leagueId}] 리그 생성에 실패했습니다! LeagueTeamSelector에서 팀을 다 채우지 못했을 수 있습니다.</color>");
                        }
                        else
                        {
                            LeagueDataManager.Instance.CreateAndSaveLeagueWithPrevTeams(leagueId);
                            Debug.Log($"<color=cyan>[{leagueId}] 이전 리그 팀 명단을 그대로 유지하여 새 리그 생성 완료!</color>");
                        }
                    }
                    else
                    {
                        LeagueDataManager.Instance.CreateAndSaveLeagueWithPrevTeams(leagueId);
                        Debug.Log($"<color=cyan>[{leagueId}] 이전 리그 팀 명단을 그대로 유지하여 새 리그 생성 완료!</color>");
                    }
                }
            }
        }

        OnWeekChanged?.Invoke(calendar);
    }

    public bool IsFundingDay() => calendar.week == 1;

    public bool CheckEventDay(int weekId)
    {
        if (_calReader == null || _calReader.DataList == null || weekId <= 0 || weekId > _calReader.DataList.Count)
            return false;

        return _calReader.DataList[weekId - 1].phase.Equals(phaseType.Event);
    }

    // 현재 함수명 기준으로 보면 "존재 여부"인데 구현은 반대로 되어 있음
    // 필요하면 아래처럼 != "" 로 수정하는 걸 추천
    public bool HasExistStartCutscene(int weekId)
    {
        if (_calReader == null || _calReader.DataList == null || weekId <= 0 || weekId > _calReader.DataList.Count)
            return false;

        return !_calReader.DataList[weekId - 1].startCutscene.Equals("");
    }

    public bool HasExistEndCutscene(int weekId)
    {
        if (_calReader == null || _calReader.DataList == null || weekId <= 0 || weekId > _calReader.DataList.Count)
            return false;

        return !_calReader.DataList[weekId - 1].endCutscene.Equals("");
    }

    public Calendar GetCalendar() => calendar;

    public phaseType CurrentGetPhaseType()
    {
        if (GameManager.Instance == null || GameManager.Instance.SaveData == null)
            return default;

        int weekId = GameManager.Instance.SaveData.weekId;
        return _calReader.DataList[weekId - 1].phase;
    }

    // 현재 달 1주차 ~ 마지막 주 desc 반환
    public List<string> GetDescArrayByMonth(int weekId)
    {
        var descList = new List<string>();

        if (_calReader == null || _calReader.DataList == null || _calReader.DataList.Count == 0)
            return descList;

        int currentMonthStart = weekId - calendar.week;
        int currentMonthWeekCount = MonthWeekTable.weekCounts[calendar.month - 1];
        int count = _calReader.DataList.Count;

        for (int i = 0; i < currentMonthWeekCount; i++)
        {
            int index = (currentMonthStart + i) % count;
            if (index < 0)
                index += count;

            string desc = StringManager.Instance.GetString(_calReader.DataList[index].weekDescKey);
            descList.Add(desc);
        }

        return descList;
    }

    // 다음 달 1주차 ~ 마지막 주 데이터 리스트 계산
    public List<string> GetDescArrayByNextMonth(int weekId)
    {
        var descList = new List<string>();

        if (_calReader == null || _calReader.DataList == null || _calReader.DataList.Count == 0)
            return descList;

        int currentMonthStart = weekId - calendar.week;
        int currentMonthWeekCount = MonthWeekTable.weekCounts[calendar.month - 1];

        int nextMonth = calendar.month + 1;
        if (nextMonth > 12)
            nextMonth = 1;

        int nextMonthWeekCount = MonthWeekTable.weekCounts[nextMonth - 1];
        int start = currentMonthStart + currentMonthWeekCount;
        int count = _calReader.DataList.Count;

        for (int i = 0; i < nextMonthWeekCount; i++)
        {
            int index = (start + i) % count;
            if (index < 0)
                index += count;

            string desc = StringManager.Instance.GetString(_calReader.DataList[index].weekDescKey);
            descList.Add(desc);
        }

        return descList;
    }

    public string GetLeagueIdByWeekId(int weekId)
    {
        if (_calReader == null || _calReader.DataList == null || weekId <= 0 || weekId > _calReader.DataList.Count)
            return null;

        return _calReader.DataList[weekId - 1].leagueId;
    }

    public string GetTutorialId(int index)
    {
        if (_calReader == null || _calReader.DataList == null || index < 0 || index >= _calReader.DataList.Count)
            return null;

        if (string.IsNullOrWhiteSpace(_calReader.DataList[index].tutorialId))
            return null;

        return _calReader.DataList[index].tutorialId;
    }

    public bool TryHandleWeek1LobbyFlow()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.SaveData == null) return true;

        if (gm.SaveData.weekId != 1)
            return false;

        if (CanGoEnding())
        {
            gm.GoToEnding();
            return true;
        }

        // 엔딩이 아니면 1월 1주차 팝업 출력
        var sOutChecker = GameObject.FindAnyObjectByType<SeasonOutChecker>();
        sOutChecker.SetPopupActivate(true);

        return true;
    }
}