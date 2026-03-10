using System.Collections.Generic;
using UnityEngine;

public class RandomEvent
{
    private Event_DataModelReader _Event_DataModelReader;

    [SerializeField] string _eventId; //이벤트 아이디
    [SerializeField] int _eventPriority; //이벤트 우선순위
    [SerializeField] int _cooldownTurn; //쿨타임
    [SerializeField] int _currentCooldownTurn; //쿨타임
    [SerializeField] bool _isReady; //이벤트 발생 시 false, 쿨다운 시작

    public string EventId => _eventId;
    public int EventPriority => _eventPriority;
    public int CooldownTurn => _currentCooldownTurn;
    public bool IsReady => _isReady;

    public RandomEvent(string eventID, int cooldownTurn, string eventPriority)
    {
        _eventId = eventID;
        _cooldownTurn = cooldownTurn;
        _eventPriority = int.Parse(eventPriority);
        _currentCooldownTurn = _cooldownTurn;
        _isReady = true;
    }

    //주차 지날때마다 모든 이벤트의 쿨다운값이 감소되어야 함.
    public void Cooldown()
    {
        _currentCooldownTurn--;

        //쿨다운값이 0이되면 카운트 다운 초기화 및 stop
        if(_currentCooldownTurn == 0)
        {
            _currentCooldownTurn = _cooldownTurn;
            _isReady = true;
        }
    }
}
