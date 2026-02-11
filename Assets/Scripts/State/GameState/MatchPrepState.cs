using UnityEngine;

public class MatchPrepState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public MatchPrepState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // 선수 배치 관련 UI 
    }

    public void Exit() { }

    public void Update() { }    
}
