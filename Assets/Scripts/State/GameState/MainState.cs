using UnityEngine;

public class MainState : IState
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
        _sm.ChangeState<LobbyState>();
    }

    public void Exit() { }

    public void Update() { }
}
