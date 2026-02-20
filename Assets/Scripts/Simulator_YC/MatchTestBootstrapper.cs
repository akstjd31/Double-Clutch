using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 테스트 전용 부트스트래퍼
/// 실제 게임에서는 사용하지 않음
/// </summary>
public class MatchTestBootstrapper : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MatchEngine _simulator;
    [SerializeField] private MatchState _state;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        if (_simulator == null) _simulator = FindFirstObjectByType<MatchEngine>();
        if (_state == null) _state = FindFirstObjectByType<MatchState>();

        // 팀 생성
        MatchTeam homeTeam = CreateTeam(TeamSide.Home, "상북 고등학교", "TC_BAL_Base");
        MatchTeam awayTeam = CreateTeam(TeamSide.Away, "진공 고등학교", "TC_DEF_Base");

        // MatchState 초기화
        _state.InitializeMatch(homeTeam, awayTeam);

        // 실제 경기 흐름은 MatchEngine이 전담
        // Bootstrapper는 팀 세팅만 하고 시작 신호만 보냄
        _simulator.StartSimulation();
    }

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
            for (int i = 0; i < 5; i++)
            {
                var stats = new Dictionary<MatchStatType, int>
                {
                    { MatchStatType.TwoPoint, 60 },
                    { MatchStatType.ThreePoint, 40 },
                    { MatchStatType.Pass, 50 },
                    { MatchStatType.Steal, 50 },
                    { MatchStatType.Block, 50 },
                    { MatchStatType.Rebound, 50 },
                    { MatchStatType.Dribble, 50 }
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

        return new MatchPlayer(id, s.Name, pos, stats, "Student_Resource");
    }
}
