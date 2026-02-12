using System.Collections.Generic;
using UnityEngine;

public class MatchDebugSetup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MatchState _matchState;
    [SerializeField] private MatchEngine _matchEngine;

    private void Awake() // Start보다 먼저 실행됨
    {
        // 가짜 팀 데이터 만들기
        MatchTeam homeTeam = CreateFakeTeam(TeamSide.Home, "상북 고등학교", "TC_BAL_Base");
        MatchTeam awayTeam = CreateFakeTeam(TeamSide.Away, "진공 고등학교", "TC_DEF_Base");

        //  MatchState에 데이터 주입
        _matchState.InitializeMatch(homeTeam, awayTeam);

        //  눈에 보이는 캡슐 생성 및 연결 (비주얼 세팅)
        SpawnAndLinkVisuals(homeTeam, Color.blue, new Vector3(-5, 0, 0)); // 왼쪽
        SpawnAndLinkVisuals(awayTeam, Color.red, new Vector3(5, 0, 0));  // 오른쪽

        Debug.Log(">>> [Debug] 가짜 선수 생성 및 연결 완료!");

        //  세팅 끝났으니 경기 시작!
        _matchEngine.StartSimulation();
    }

    // 가짜 팀/선수 데이터 생성 함수
    private MatchTeam CreateFakeTeam(TeamSide side, string teamName, string tactic)
    {
        MatchTeam team = new MatchTeam(side, teamName, tactic);

        // 포지션 순서대로 5명 생성
        Position[] positions = { Position.PG, Position.SG, Position.SF, Position.PF, Position.C };

        for (int i = 0; i < 5; i++)
        {
            // 능력치는 일단 50으로 통일 (나중에 MatchDataProxy가 처리)
            var stats = new Dictionary<StatType, int>();
            stats.Add(StatType.TwoPoint, 50);
            stats.Add(StatType.ThreePoint, 50);
            stats.Add(StatType.Pass, 50);
            stats.Add(StatType.Steal, 50);
            stats.Add(StatType.Block, 50);
            stats.Add(StatType.Rebound, 50);

            // 임시 리소스 키 "test_res"
            MatchPlayer player = new MatchPlayer(i, $"{teamName}_{positions[i]}", positions[i], stats, "test_res");
            team.AddPlayer(player);
        }

        return team;
    }

    // 캡슐 생성 및 데이터 연결 함수
    private void SpawnAndLinkVisuals(MatchTeam team, Color color, Vector3 startPos)
    {
        float spacing = 2.0f; // 선수 간격

        for (int i = 0; i < team.Roster.Count; i++)
        {
            MatchPlayer player = team.Roster[i];

            //  유니티 기본 캡슐 생성
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = $"Visual_{player.PlayerName}";

            Vector3 spawnPos = startPos + new Vector3(0, i * spacing - 4.0f, 0);

            capsule.transform.position = spawnPos;

            var renderer = capsule.GetComponent<MeshRenderer>();
            renderer.material.color = color;

            player.VisualObject = capsule;
        }
    }
}
