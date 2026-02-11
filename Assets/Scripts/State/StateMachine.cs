using UnityEngine;

/// <summary>
/// 상태 머신 클래스: 각 객체 생성 후 개별로 관리
/// </summary>
public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState nextState)
    {
        if (nextState == null) return;
        if (ReferenceEquals(CurrentState, nextState)) return;

        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void Clear()
    {
        CurrentState?.Exit();
        CurrentState = null;
    }
}
