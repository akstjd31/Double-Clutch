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
        // 각 버튼 리스너 할당
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(">>> [Lobby] 경기 시작 요청 (MatchSimState로 전환)");
            _sm.ChangeState<MatchSimState>();
        }
    }
}
