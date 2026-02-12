using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public LoadingState(GameManager gm, StateMachine sm)
    {
        _gm = gm;
        _sm = sm;
    }

    public void Enter()
    {
        // 로딩 씬 이동
        SceneManager.LoadScene("Test_Loading");
    }

    public void Exit() { }

    public void Update() {}
}
