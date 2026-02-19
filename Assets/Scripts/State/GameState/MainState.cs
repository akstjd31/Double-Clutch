using System;
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
                NextStep<LobbyState>("Test_Lobby");
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

    // 다음 스텝에 대한 정보
    public void NextStep<T>(string sceneName) where T : class, IState
    {
        _gm.SetNextFlow(sceneName, _sm.Get<T>());
        _sm.ChangeState<LoadingState>();
    }
}
