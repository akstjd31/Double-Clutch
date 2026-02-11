using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태 머신 클래스: 각 객체 생성 후 개별로 관리
/// </summary>
public class StateMachine
{
    private readonly Dictionary<Type, IState> _states = new();
    public IState CurrentState { get; private set; }

    // 상태 등록 (매번 new 할당하는 것을 방지)
    public void Register(IState state)
    {
        if (state == null) return;
        _states[state.GetType()] = state;
        Debug.Log($"{state} 상태 등록 완료!");
    }

    public T Get<T>() where T : class, IState
    {
        if (_states.TryGetValue(typeof(T), out var s)) 
            return s as T;
        
        return null;
    }

    public void ChangeState<T>() where T : class, IState => ChangeState(Get<T>());

    public void ChangeState(IState nextState)
    {
        if (nextState == null) return;
        if (ReferenceEquals(CurrentState, nextState)) return;

        Debug.Log($"{CurrentState} => {nextState} 상태 변경");
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
        Debug.Log($"{CurrentState} 상태 제거");
        CurrentState?.Exit();
        CurrentState = null;
    }
}
