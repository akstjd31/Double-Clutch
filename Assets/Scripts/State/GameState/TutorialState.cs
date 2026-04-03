using UnityEngine;

public class TutorialState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public TutorialState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
        // 끝
    }


    public void Update() { }
}
