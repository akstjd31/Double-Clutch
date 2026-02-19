using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchTestBootstrapper : MonoBehaviour
{
    [Header("필수 연결")]
    [SerializeField] private MatchEngine _engine;
    [SerializeField] private MatchState _state;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        Debug.LogWarning(">>> [TestBootstrapper] 테스트용 강제 경기 시작 루틴 가동");

        if (_engine == null) _engine = FindFirstObjectByType<MatchEngine>();
        if (_state == null) _state = FindFirstObjectByType<MatchState>();

        // 학생 데이터 없으면 강제 영입
        if (StudentManager.Instance != null && StudentManager.Instance.GetAllStudents().Count == 0)
        {
            Debug.Log("[Test] 학생 데이터 5명 강제 생성");
            StudentManager.Instance.MakeTestStudents(5);
        }

        RunMatchLogic();
    }

    private void RunMatchLogic()
    {
        MatchTeam homeTeam = CreateTeam(TeamSide.Home, "상북 고등학교", "TC_BAL_Base");
        MatchTeam awayTeam = CreateTeam(TeamSide.Away, "진공 고등학교", "TC_DEF_Base");

        _state.InitializeMatch(homeTeam, awayTeam);

        // 선수 비주얼(캡슐) 생성 및 연결!
        SpawnAndLinkVisuals(homeTeam, Color.blue, new Vector3(-5, 0, 0)); // 홈팀: 왼쪽
        SpawnAndLinkVisuals(awayTeam, Color.red, new Vector3(5, 0, 0));   // 원정팀: 오른쪽

        _engine.OnMatchEnded = () => { Debug.Log(">>> [Test] 경기 종료!"); };

        _engine.StartSimulation();
    }
    private void SpawnAndLinkVisuals(MatchTeam team, Color color, Vector3 startPos)
    {
        float spacing = 2.0f;
        for (int i = 0; i < team.Roster.Count; i++)
        {
            MatchPlayer player = team.Roster[i];

            // 캡슐 생성
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = $"Visual_{player.PlayerName}";

            // 위치 배치
            Vector3 spawnPos = startPos + new Vector3(0, i * spacing - 4.0f, 0);
            capsule.transform.position = spawnPos;

            // 색상 변경
            var renderer = capsule.GetComponent<MeshRenderer>();
            renderer.material.color = color;

            // 데이터와 비주얼 연결
            player.VisualObject = capsule;
        }
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