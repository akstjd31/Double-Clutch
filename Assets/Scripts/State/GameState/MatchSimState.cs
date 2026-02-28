using System.Collections.Generic;
using UnityEngine;

public class MatchSimState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    // 씬에 있는 매니저들을 찾기 위한 변수
    private MatchEngine _engine;
    private MatchState _state;

    // 데이터 보관용 변수
    private List<Student> _homeRoster;
    private List<Student> _awayRoster;
    public MatchSimState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }
    public void SetRosters(List<Student> home, List<Student> away)
    {
        _homeRoster = home;
        _awayRoster = away;
    }

    public void Enter()
    {
        // 씬에 있는 매니저들 찾기
        _engine = Object.FindFirstObjectByType<MatchEngine>();
        _state = Object.FindFirstObjectByType<MatchState>();

        if (_engine == null || _state == null)
        {
            Debug.LogError("MatchEngine 또는 MatchState를 찾을 수 없습니다! (경기 씬에 매니저가 있는지 확인하세요)");
            return;
        }

        // 저장된 '출전 명단' 5명씩을 가져옵니다.
        MatchTeam homeTeam = ConvertToTeam(TeamSide.Home, GameManager.Instance.SaveData.schoolName, "TC_BAL_Base", _homeRoster);
        MatchTeam awayTeam = ConvertToTeam(TeamSide.Away, "라이벌 고교", "TC_DEF_Base", _awayRoster);

        // 초기화 및 시작
        _state.InitializeMatch(homeTeam, awayTeam);

        // 경기 종료 시 ResultState로 넘어가도록 이벤트 연결
        _engine.OnMatchEnded = () =>
        {
            _gm.StartCoroutine(CoGoToResult());
        };

        _engine.StartSimulation();
    }

    private System.Collections.IEnumerator CoGoToResult()
    {
        Debug.Log("경기 종료! 3초 뒤 결과 창으로 이동합니다...");
        yield return new WaitForSeconds(3.0f); // 연출 볼 시간 확보
        _sm.ChangeState<ResultState>();        // ResultState로 전환
    }

    public void Exit()
    {
        // 이벤트 연결 해제 등 정리
        if (_engine != null) _engine.OnMatchEnded = null;
    }

    public void Update() { }

    private MatchTeam ConvertToTeam(TeamSide side, string teamName, string tactic, List<Student> students)
    {
        MatchTeam team = new MatchTeam(side, teamName, tactic);

        // 홈팀은 10000번대, 어웨이팀은 20000번대 고유 ID 임시 부여
        int startId = side == TeamSide.Home ? 10000 : 20000;

        for (int i = 0; i < students.Count; i++)
        {
            team.AddPlayer(ConvertStudentToMatchPlayer(students[i], startId + i, students[i].Position));
        }

        return team;
    }

    private MatchPlayer ConvertStudentToMatchPlayer(Student s, int id, Position pos)
    {
        Dictionary<MatchStatType, int> stats = new Dictionary<MatchStatType, int>();
        stats.Add(MatchStatType.TwoPoint, s.GetCurrentStat(potential.Stat2pt));
        stats.Add(MatchStatType.ThreePoint, s.GetCurrentStat(potential.Stat3pt));
        stats.Add(MatchStatType.Pass, s.GetCurrentStat(potential.StatPass));
        stats.Add(MatchStatType.Steal, s.GetCurrentStat(potential.StatSteal));
        stats.Add(MatchStatType.Block, s.GetCurrentStat(potential.StatBlock));
        stats.Add(MatchStatType.Rebound, s.GetCurrentStat(potential.StatRebound));
        stats.Add(MatchStatType.Dribble, 50);

        string actualVisualKey = s.VisualData.playerImageResource;
        if (string.IsNullOrEmpty(actualVisualKey)) actualVisualKey = "Default_Player_Sprite";

        return new MatchPlayer(id, s.Name, pos, stats, actualVisualKey, s.Passive);
    }
}
