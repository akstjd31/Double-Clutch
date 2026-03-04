public class GraduationState : IState
{
    private GameManager _gm;
    private StateMachine _sm;
    public GraduationState(GameManager gm, StateMachine sm)
    {
        _gm = gm;
        _sm = sm;
    }

    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void Update() {}
}