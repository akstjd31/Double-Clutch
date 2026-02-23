using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchPlayer
{
    public const int MAX_STAMINA = 100;

    private int _playerId;
    private string _playerName;
    private Position _position;
    private Dictionary<MatchStatType, int> _stats; // 6대 스탯 + @ 관리
    private int _currentCondition; // 경기 중 소모되는 컨디션

    public List<Player_PassiveData> Passives { get; private set; }
    // 시뮬레이션은 이 좌표로 계산하고, 리플레이어는 이걸 월드 좌표로 변환해서 보여줌.
    public Vector2 LogicPosition { get; set; }

    // 비주얼 오브젝트 (리플레이어에서 사용)
    public GameObject VisualObject { get; set; }
    public int PlayerId => _playerId;
    public string PlayerName => _playerName;
    public Position MainPosition => _position;
    public int CurrentCondition
    {
        get => _currentCondition;
        set => _currentCondition = Mathf.Clamp(value, 0, MAX_STAMINA);
    }

    // 생성자: 데이터 로드시 초기화
    public MatchPlayer(int id, string name, Position pos, Dictionary<MatchStatType, int> initStats, string resourceKey, List<Player_PassiveData> passives = null)
    {
        _playerId = id;
        _playerName = name;
        _position = pos;
        _stats = new Dictionary<MatchStatType, int>(initStats);
        _currentCondition = MAX_STAMINA;
        Passives = passives ?? new List<Player_PassiveData>();
        // 초기 위치 설정
        InitDefaultPosition();
    }

    private void InitDefaultPosition()
    {
        switch (_position)
        {
            case Position.PG: LogicPosition = new Vector2(0.5f, 0.65f); break; // 탑
            case Position.SG: LogicPosition = new Vector2(0.8f, 0.75f); break; // 우측 45도
            case Position.SF: LogicPosition = new Vector2(0.2f, 0.75f); break; // 좌측 45도
            case Position.PF: LogicPosition = new Vector2(0.65f, 0.85f); break; // 하이 포스트
            case Position.C: LogicPosition = new Vector2(0.5f, 0.9f); break;  // 골밑
            default: LogicPosition = new Vector2(0.5f, 0.5f); break;
        }
    }

    public int GetStat(MatchStatType type, float tacticBonus = 1.0f)
    {
        if (_stats.ContainsKey(type))
            return Mathf.RoundToInt(_stats[type] * tacticBonus);
        return 0;
    }

}
