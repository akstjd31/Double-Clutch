using UnityEngine;

public class MainState : IState, IUIActionHandler
{
    private readonly GameManager _gm;
    private readonly StateMachine _sm;

    public MainState(GameManager gm, StateMachine sm)
    {
        _gm = gm;
        _sm = sm;
    }

    public void Enter()
    {
        // 데이터 로드, 세팅 등 작업 먼저 수행
    }

    public void Exit() { }

    public void Update() { }

    // 어떤 버튼을 눌렀는지에 대한 정보에 따라 실행 결과가 달라짐
    public void Handle(UIAction action)
    {
        switch (action)
        {
            case UIAction.Main_Start:
                // _gm.Execute() 커맨드 수행

                GoLobby();
                break;

            case UIAction.Main_Quit:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                break;
        }
    }

    public void GoLobby()
    {
        // 씬 이름은 테스트용
        _gm.SetNextFlow("Test_Lobby", _sm.Get<LobbyState>());
        _sm.ChangeState<LoadingState>();
    }
}
