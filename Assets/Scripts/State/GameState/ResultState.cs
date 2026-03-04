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
        int currentMatchId = 1; // 임시 매치 ID (라운드 번호)
        // 리그 보관소에 현재 경기 기록 저장 
        if (LeagueRecordManager.Instance != null)
        {
            LeagueRecordManager.Instance.SaveMatchRecord(currentMatchId, matchState, matchEngine.FullMatchLogs);
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
            () =>
            {
                // MVP를 위해 ReturnToLobby() 대신 리그 결산 패널을 띄웁니다.
                // 이전 채팅에서 추가하신 LeagueCalculatePanel을 호출하며, 풀 로그 데이터를 넘겨줍니다.
                uiManager.ShowLeagueCalculatePanel(currentMatchId, () => GoToLobby());
            }
         );
    }

    public void Exit()
    {
        // 상태를 빠져나갈 때 결과창을 닫거나 정리할 내용이 있다면 여기에 작성
    }

    public void Update() { }
    public void GoToLobby()
    {
        CalendarManager.Instance.NextTurn();
        
        // GameManager에 다음 씬(LOBBY)과 다음 상태(LobbyState)를 세팅
        _gm.SetNextFlow(SceneName.LOBBY, _sm.Get<LobbyState>());

        // 로딩 상태로 전환하여 자연스럽게 씬 이동 처리
        _sm.ChangeState<LoadingState>();
    }
}
