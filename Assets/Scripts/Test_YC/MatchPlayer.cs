using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchPlayer
{
    public const int MAX_STAMINA = 100;

    private int _playerId;
    private string _playerName;
    private Position _position;
    private Dictionary<StatType, int> _stats; // 6대 스탯 + @ 관리
    private int _currentCondition; // 경기 중 소모되는 컨디션

    // 시뮬레이션 상 현재 위치 (코트 위 좌표, 나중에 연출 연동용)
    private Vector2 _currentPosition;

    private string _resourceKey;
    private GameObject _visualObject;

    public GameObject VisualObject
    {
        get => _visualObject;
        set => _visualObject = value;
    }

    public int PlayerId => _playerId;
    public string PlayerName => _playerName;
    public Position MainPosition => _position;
    public int CurrentCondition
    {
        get => _currentCondition;
        set => _currentCondition = Mathf.Clamp(value, 0, MAX_STAMINA);
    }

    // 생성자: 데이터 로드시 초기화
    public MatchPlayer(int id, string name, Position pos, Dictionary<StatType, int> initStats, string resourceKey)
    {
        _playerId = id;
        _playerName = name;
        _position = pos;
        _stats = new Dictionary<StatType, int>(initStats);
        _currentCondition = MAX_STAMINA; // 기본값 100 시작
        _resourceKey = resourceKey;
    }

    public int GetStat(StatType type)
    {
        if (_stats.ContainsKey(type))
        {
            // 컨디션에 따른 스탯 페널티 로직 추가 필요 (  보류중  )
            return _stats[type];
        }
        return 0;
    }

    /// <summary>
    /// 경기 중 위치 업데이트
    /// </summary>
    public void UpdatePosition(Vector2 newPos)
    {
        _currentPosition = newPos;
    }
}
