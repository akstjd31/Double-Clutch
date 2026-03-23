using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

// 달마다 있는 이 주차는 고정이기 떄문에 이렇게 정의했음
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

        var gm = GameManager.Instance;
        if (gm == null) return;

        // 임시 테스트용
        var weekId = gm.SaveData.weekId;

        // 만약 전체 일정이 끝나게 된다면 weekId 0으로 시작(1월 1일)
        if (weekId >= _calReader.DataList.Count)
            weekId = 0;

        // 1. 주차 시작(주차 계산)
        CalcWeek(weekId, gm);
        //주차가 끝나면 이벤트 쿨다운-HJ
        EventManager.Instance.WeekendCooldown();

        // CalcWeek 안에서 변경된 최신 weekId를 다시 가져옵니다.
        int nextWeekId = gm.SaveData.weekId;

        // 2. 시작 컷신 체크
        if (HasExistStartCutscene(nextWeekId))
        {

        }

        // 3. 페이즈 체크
        if (!CheckPhaseType(nextWeekId)) return;

        // 4. 종료 컷신 체크
        if (HasExistEndCutscene(nextWeekId))
        {

        }

        // 5. 턴 종료 시
        Init();
    }

    private void Init()
    {
        IsEndPhase = false;
    }

    public void CalcWeek(int weekId, GameManager gm)
    {
        if (gm == null) return;
        var data = _calReader.DataList[weekId - 1];

        if (weekId == 8) // 3월 1일이라면 년차 추가
        {
            // 영입이 3월 1일 기준으로 진행되기 떄문에 연차++ 작업을 이떄 해줌
            var y = gm.SaveData.year;
            gm.SetYear(y + 1);
        }

        bool flag = false;
        if (PlayerPrefs.GetInt(PrefKeys.KEY_FIRST_RUN_DONE) == 0)
        {
            // 튜토리얼 수행 완료
            flag = true;
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
                    weekId = LeagueManager.Instance.IsPlayerSeasonOut() ? data.targetidSpecial : data.targetidDefault; // 리그 시즌아웃 처리는 따로 해줘야할듯 
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

        // 만약 시즌아웃을 당했다면 현재 달과 타겟 달 차이를 비교하여 누적시킨 지원금을 추가로 받는다.
        int accSub = data.hasSeasonOut ? (data.month - calendar.month + 12) % 12 : 1;

        calendar.month = data.month;
        calendar.week = data.weekNo;

        if (IsFundingDay())
        {
            var m = gm.SaveData.money;

            if (!flag)
                gm.SetMoney(m + (1000 * accSub));
        }

        gm.SetWeekId(weekId);

        // 이벤트 페이즈면서 어떤 날인지 구분하는게 필요함. (ex. 졸업, 영입 등)
        if (CheckEventDay(weekId))
        {
            if (calendar.month == 2 && calendar.week == 4)
                gm.GoToGraduation();
        }

        var leagueDataMgr = LeagueDataManager.Instance;
        if (leagueDataMgr != null && !CheckEventDay(weekId))
        {
            string leagueId = GetLeagueIdByWeekId(weekId);
            // 이번 주차 캘린더에 리그 일정이 있다면!
            if (!string.IsNullOrEmpty(leagueId) && leagueId != "-")
            {
                var masterData = leagueDataMgr.GetMasterDataById(leagueId);

                // 마스터 데이터를 찾지 못했을 때의 에러 로그 
                if (!masterData.HasValue)
                {
                    Debug.LogError($"<color=red>리그 마스터 테이블에서 '{leagueId}'를 찾을 수 없습니다! 엑셀 파일에 오타나 띄어쓰기가 있는지 확인하세요.</color>");
                }
                else
                {
                    if (masterData.HasValue)
                    {
                        // isSelectionRequired가 TRUE일 때만 리그가 정상 생성
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
        }

        OnWeekChanged?.Invoke(calendar);
    }

    public bool IsFundingDay() => calendar.week == 1;

    public bool CheckEventDay(int weekId) => _calReader.DataList[weekId - 1].phase.Equals(phaseType.Event);


    // 일단 컷신 부분은 패스(-)
    public bool HasExistStartCutscene(int weekId) => _calReader.DataList[weekId - 1].startCutscene.Equals("");

    public bool HasExistEndCutscene(int weekId) => _calReader.DataList[weekId - 1].endCutscene.Equals("");
     public bool CheckPhaseType(int weekId)
     {
         if (_calReader == null) return false;

         switch (_calReader.DataList[weekId - 1].phase)
         {
             case phaseType.League:
                 
                 return true;

             // 경우에 따라 작성 (이벤트일떄) phaseType.Event ..
         }

         return IsEndPhase;
     }

    public Calendar GetCalendar() => this.calendar;
    public phaseType CurrentGetPhaseType()
    {
        return _calReader.DataList[GameManager.Instance.SaveData.weekId - 1].phase;
    }

    // 현재 달에 해당되는 1주차 ~ 끝 Desc 값 리스트 반환
    public List<string> GetDescArrayByMonth(int weekId)
    {
        var descList = new List<string>();
        int start = 0, end = 0;
        // 1주면 계산 필요 없음
        if (calendar.week == 1)
        {
            start = weekId;
            end = weekId + MonthWeekTable.weekCounts[calendar.month - 1];
        }

        else
        {
            start = weekId - calendar.week;
            end = start + MonthWeekTable.weekCounts[calendar.month - 1];
        }

        for (int i = start; i < end; i++)
        {
            string desc = StringManager.Instance.GetString(_calReader.DataList[i].weekDescKey);
            descList.Add(desc);
        }
            

        return descList;
    }

    // 다음 달 1주차 ~ 마지막 주 데이터 리스트 계산
    public List<string> GetDescArrayByNextMonth(int weekId)
    {
        var descList = new List<string>();

        // 남은 주 + 1
        int start = weekId + MonthWeekTable.weekCounts[calendar.month - 1] - calendar.week;
        int end = start + MonthWeekTable.weekCounts[calendar.month];

        for (int i = start; i < end; i++)
        {
            string desc = StringManager.Instance.GetString(_calReader.DataList[i].weekDescKey);
            descList.Add(desc);
        }

        return descList;
    }

    // 현 저장된 데이터의 weekID의 leagueID
    public string GetLeagueIdByWeekId(int weekId)
    {
        if (_calReader == null) return null;
        return _calReader.DataList[weekId - 1].leagueId;
    }
}
