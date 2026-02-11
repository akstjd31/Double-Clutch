using UnityEngine;

public class EventState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public EventState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // UI On
    }

    public void Exit()
    {
        // UI Off
    }

    public void Update() { }    
}
