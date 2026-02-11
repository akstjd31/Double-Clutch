using UnityEngine;

public class MatchSimState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public MatchSimState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // 시뮬레이션 시작
    }

    public void Exit()
    {
        // 시뮬레이션 끝
    }

    public void Update() { }    
}
