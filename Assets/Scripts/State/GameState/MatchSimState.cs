using System.Collections.Generic;
using UnityEngine;

public class MatchSimState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    // 씬에 있는 매니저들을 찾기 위한 변수
    private MatchEngine _engine;
    private MatchState _state;

    public MatchSimState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // 씬에 있는 MatchEngine과 MatchState 찾기
        _engine = Object.FindFirstObjectByType<MatchEngine>();
        _state = Object.FindFirstObjectByType<MatchState>();

        if (_engine == null || _state == null)
        {
            Debug.LogError("MatchEngine 또는 MatchState를 찾을 수 없습니다!");
            return;
        }

        // 팀 생성 및 데이터 연동 (MatchDebugSetup 로직 이식)
        MatchTeam homeTeam = CreateTeam(TeamSide.Home, "상북 고등학교", "TC_BAL_Base");
        MatchTeam awayTeam = CreateTeam(TeamSide.Away, "진공 고등학교", "TC_DEF_Base");

        // 초기화 및 시작
        _state.InitializeMatch(homeTeam, awayTeam);

        // 경기 종료 시 ResultState로 넘어가도록 이벤트 연결
        _engine.OnMatchEnded = () =>
        {
            // 경기가 끝나면 2초 뒤에 결과 상태로 전환 (코루틴 사용)
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
    private MatchTeam CreateTeam(TeamSide side, string teamName, string tactic)
    {
        MatchTeam team = new MatchTeam(side, teamName, tactic);
        Position[] positions = { Position.PG, Position.SG, Position.SF, Position.PF, Position.C };

        List<Student> students = null;
        if (StudentManager.Instance != null)
            students = StudentManager.Instance.GetAllStudents();

        if (side == TeamSide.Home && students != null && students.Count >= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                MatchPlayer player = ConvertStudentToMatchPlayer(students[i], i, positions[i]);
                team.AddPlayer(player);
            }
        }
        else
        {
            // 가짜 데이터 (적 팀 테스트용) 
            for (int i = 0; i < 5; i++)
            {
                var stats = new Dictionary<MatchStatType, int>
                {
                    { MatchStatType.TwoPoint, 50 }, { MatchStatType.ThreePoint, 50 },
                    { MatchStatType.Pass, 50 }, { MatchStatType.Steal, 50 },
                    { MatchStatType.Block, 50 }, { MatchStatType.Rebound, 50 },
                    { MatchStatType.Dribble, 50 }, { MatchStatType.Speed, 50 },
                    { MatchStatType.Stamina, 100 }
                };
                MatchPlayer player = new MatchPlayer(i, $"{teamName}_{positions[i]}", positions[i], stats, "test_res");
                team.AddPlayer(player);
            }
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
        stats.Add(MatchStatType.Speed, 50);
        stats.Add(MatchStatType.Stamina, 100);

        return new MatchPlayer(id, s.Name, pos, stats, "Student_Resource");
    }
}
