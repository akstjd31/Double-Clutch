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
        // 튜토리얼 시작
        MarkFirstRunDone();
        _gm.SetNextFlow("Test_Lobby", _sm.Get<LobbyState>());
    }

    public void Exit()
    {
        // 끝
    }

    // 튜토리얼 끝날 때쯤에 실행될 메서드 (이 플레이어는 튜토리얼이 필요없음)
    public void MarkFirstRunDone()
    {
        PlayerPrefs.SetInt("FIRST_RUN_DONE", 1);
        PlayerPrefs.Save();
    }

    public void Update() { }
}
