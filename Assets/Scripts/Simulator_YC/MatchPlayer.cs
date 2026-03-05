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
    public int Score { get; set; } = 0;
    // 비주얼 오브젝트 (리플레이어에서 사용)
    public GameObject VisualObject { get; set; }

    // 외형 리소스 키와 특성 ID를 저장할 프로퍼티
    public string ResourceKey { get; private set; }
    public string TraitId { get; set; } = string.Empty;

    public int PlayerId => _playerId;
    public string PlayerName => _playerName;
    public Position MainPosition => _position;
    public int CurrentCondition
    {
        get => _currentCondition;
        set => _currentCondition = Mathf.Clamp(value, 0, MAX_STAMINA);
    }
    // 하프타임 이벤트로 인한 임시 포지션(진형) 변경
    public changeType TempPositionChange { get; set; } = changeType.Default;
    // 하프타임 이벤트로 인한 임시 스탯 증감치 저장소
    private Dictionary<MatchStatType, float> _tempStatBuffs = new Dictionary<MatchStatType, float>();
    // 생성자: 데이터 로드시 초기화
    public MatchPlayer(int id, string name, Position pos, Dictionary<MatchStatType, int> initStats, string resourceKey, List<Player_PassiveData> passives = null)
    {
        _playerId = id;
        _playerName = name;
        _position = pos;
        _stats = new Dictionary<MatchStatType, int>(initStats);
        _currentCondition = MAX_STAMINA;
        Passives = passives ?? new List<Player_PassiveData>();
        ResourceKey = resourceKey;
        // 초기 위치 설정
        InitDefaultPosition();
        // [디버그] 초기화 완료 직후 들어온 스탯 확인
        Debug.Log($"<color=#00FF00>[MatchPlayer 변환 완료]</color> {_playerName} 생성됨. 전달받은 2점슛 스탯: {_stats[MatchStatType.TwoPoint]}");
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

    //  버프 부여 함수
    public void AddTempStatBuff(MatchStatType type, float value)
    {
        if (!_tempStatBuffs.ContainsKey(type)) _tempStatBuffs[type] = 0f;
        _tempStatBuffs[type] += value;
    }
    // 스탯 반환 시 임시 버프도 더해서 계산하도록 변경
    public int GetStat(MatchStatType type, float tacticBonus = 1.0f)
    {
        float baseStat = _stats.ContainsKey(type) ? _stats[type] : 0;
        float buff = _tempStatBuffs.ContainsKey(type) ? _tempStatBuffs[type] : 0;

        if (tacticBonus <= 0f)
        {
            tacticBonus = 1.0f;
        }

        return Mathf.RoundToInt((baseStat + buff) * tacticBonus);
    }

}
