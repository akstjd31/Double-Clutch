using UnityEngine;

public class ResultState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public ResultState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // 씬에 있는 데이터(MatchState)와 UI 관리자(MatchUIManager)를 찾습니다.
        MatchState matchState = Object.FindFirstObjectByType<MatchState>();
        MatchUIManager uiManager = Object.FindFirstObjectByType<MatchUIManager>();
        MatchEngine matchEngine = Object.FindFirstObjectByType<MatchEngine>();

        if (matchState == null || uiManager == null || matchEngine == null)
        {
            Debug.LogError("[ResultState] MatchState, matchEngine 또는 MatchUIManager를 찾을 수 없습니다.");
            return;
        }

        // 임시 지원금 (추후 보상 시스템 연동할 때 이 변수를 수정하시면 됩니다)
        int rewardAmount = 50;

        // 결과창 띄우기
        uiManager.ShowResultPopup(
            matchState.HomeTeam.TeamName,
            matchState.HomeTeam.Score,
            matchState.AwayTeam.TeamName,
            matchState.AwayTeam.Score,
            rewardAmount,
            () => ReturnToLobby() // <--- 콜백 전달!
         );
        //  리그 보관소에 현재 경기 기록 저장!
        // (참고: matchId는 나중에 대진표 시스템이 나오면 그 번호를 넘겨주면 됩니다. 임시로 1번 경기라고 가정)
        if (LeagueRecordManager.Instance != null)
        {
            int currentMatchId = 1; // 임시 매치 ID
            LeagueRecordManager.Instance.SaveMatchRecord(currentMatchId, matchState, matchEngine.FullMatchLogs);
        }
    }

    public void Exit()
    {
        // 상태를 빠져나갈 때 결과창을 닫거나 정리할 내용이 있다면 여기에 작성
    }

    public void Update() { }
    public void ReturnToLobby()
    {
        // GameManager에 다음 씬(LOBBY)과 다음 상태(LobbyState)를 세팅
        _gm.SetNextFlow(SceneName.LOBBY, _sm.Get<LobbyState>());

        // 로딩 상태로 전환하여 자연스럽게 씬 이동 처리
        _sm.ChangeState<LoadingState>();
    }
}
