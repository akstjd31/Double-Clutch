using UnityEngine;

public class LobbyState : IState
{
    private readonly GameManager _gm;
    private readonly StateMachine _sm;

    public LobbyState(GameManager gm, StateMachine sm)
    {
        _gm = gm;
        _sm = sm;
    }

    public void Enter()
    {
        if (CalendarManager.Instance == null) return;

        if (CalendarManager.Instance.CheckGraduationDay())
            _gm.GoToGraduation();
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
    }
}
