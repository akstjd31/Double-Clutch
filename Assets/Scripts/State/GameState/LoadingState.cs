using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingState : IState
{
    private readonly StateMachine _sm;
    private AsyncOperation _op;

    public LoadingState(StateMachine sm)
    {
        _sm = sm;
    }

    public void Enter()
    {
        // 씬 로드
        // _op = SceneManager.LoadSceneAsync()
        // _op.allowSceneActivation = true;
    }

    public void Exit()
    {
        _op = null;
    }

    public void Update()
    {
        if (_op == null) return;
        // 씬을 넘어갈 수 있는 상태가 되면 전이
        // if (_op.isDone)
            // _sm.ChangeState<>();
    }
}
