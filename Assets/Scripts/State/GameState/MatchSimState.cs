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
        MatchTeam homeTeam = EnemyTeamFactory.Instance.ConvertToTeam(TeamSide.Home, LeagueTeamManager.Instance.GetTeamById("id")); //이부분 실제 경기하는 팀들로 받아와야 함!!!!!!!
        MatchTeam awayTeam = EnemyTeamFactory.Instance.ConvertToTeam(TeamSide.Away, LeagueTeamManager.Instance.GetTeamById("id"));

        // 초기화 및 시작
        _state.InitializeMatch(homeTeam, awayTeam);

        // 경기 종료 시 ResultState로 넘어가도록 이벤트 연결
        _engine.OnMatchEnded = () =>
        {
            int currentMatchId = GameManager.Instance.SaveData.weekId;
            LeagueRecordManager.Instance.SaveMatchRecord(currentMatchId, _state, _engine.FullMatchLogs);

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

    //private MatchTeam ConvertToTeam(TeamSide side, Team team)
    //{
    //    MatchTeam matchTeam = new MatchTeam(side, team.TeamNameKey, team.Team_ArchetypeData.Value.teamArchetypeId);

    //    // 용병이나 적군을 위한 임시 ID 시작 번호
    //    int startId = side == TeamSide.Home ? 10000 : 20000;
    //    Student[] members = team.Members;

    //    for (int i = 0; i < members.Length; i++)
    //    {
    //        Position finalPos = members[i].MatchPosition != Position.None ? members[i].MatchPosition : members[i].Position;

    //        // 정식 학생(ID 0 이상)이면 고유 ID 사용, 용병(ID -1)이나 적군이면 임시 ID 부여
    //        int matchPlayerId = (members[i].StudentId >= 0) ? members[i].StudentId : (startId + i);

    //        matchTeam.AddPlayer(ConvertStudentToMatchPlayer(members[i], matchPlayerId, finalPos));
    //    }

    //    return matchTeam;
    //}

    //private MatchPlayer ConvertStudentToMatchPlayer(Student s, int id, Position pos)
    //{
    //    Dictionary<MatchStatType, int> stats = new Dictionary<MatchStatType, int>();

    //    stats.Add(MatchStatType.TwoPoint, s.GetCurrentStat(potential.Stat2pt));
    //    stats.Add(MatchStatType.ThreePoint, s.GetCurrentStat(potential.Stat3pt));
    //    stats.Add(MatchStatType.Pass, s.GetCurrentStat(potential.StatPass));
    //    stats.Add(MatchStatType.Steal, s.GetCurrentStat(potential.StatSteal));
    //    stats.Add(MatchStatType.Block, s.GetCurrentStat(potential.StatBlock));
    //    stats.Add(MatchStatType.Rebound, s.GetCurrentStat(potential.StatRebound));

    //    string actualVisualKey = s.VisualData.playerImageResource;
    //    if (string.IsNullOrEmpty(actualVisualKey)) actualVisualKey = "Default_Player_Sprite";
    //    MatchPlayer matchPlayer = new MatchPlayer(id, s.Name, pos, stats, actualVisualKey, s.Passive);
    //    matchPlayer.TraitId = s.TraitId;
    //    return matchPlayer;
    //}
}
