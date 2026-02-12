using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    //이벤트 저장
    private static Dictionary<Type, Action<object>> _eventTable = new Dictionary<Type, Action<object>>();

    //이벤트 구독
    public static void Subscribe<T>(Action<T> listener)
    {
        Type type = typeof(T);

        //기존에 존재할 시 구독자 추가
        if (_eventTable.ContainsKey(type))
        {
            _eventTable[type] += (obj) => listener((T)obj);
        }

        //기존에 존재할 하지 않을 시 새로 생성
        else
        {
            _eventTable[type] = (obj) => listener((T)obj);
        }

        Debug.Log($"[EventBus] {type.Name} -> {listener.Method.Name}");
    }

    //이벤트 구독 해제
    public static void Unsubscribe<T>(Action<T> listener)
    {
        Type type = typeof(T);

        if (_eventTable.ContainsKey(type))
        {
            _eventTable[type] -= (obj) => listener((T)obj);
        }

        Debug.Log($"[EventBus] 구독 해제 됨 : {type.Name} -/- {listener.Method.Name}");
    }

    //이벤트 발행
    public static void Publish<T>(T eventData)
    {
        Type type = typeof(T);
        if( _eventTable.ContainsKey(type))
        {
            _eventTable[type]?.Invoke(eventData);
            Debug.Log($"[EventBus] {type.Name} : 이벤트 발행됨");
        }
        else
        {
            Debug.Log($"[EventBus] {type.Name} : 구독 없음");
        }
    }
}
