using UnityEngine;
using System.Collections.Generic;

// 전술 데이터 구조체 (스탯 보정값 저장용)
public struct TeamTactics
{
    public float bonusTwoPoint;   // 2점슛 스탯 보정
    public float bonusThreePoint; // 3점슛 스탯 보정
    public float bonusPass;       // 패스 스탯 보정
    public float bonusBlock;      // 블록 스탯 보정
    public float bonusSteal;      // 스틸 스탯 보정
    public float bonusRebound;    // 리바운드 스탯 보정
    public float bonusDribble;    // 드리블 스탯 보정

    // 테이블 미정 → 일단 전부 1.0 (보정 없음)
    public TeamTactics(float tp = 1.0f, float three = 1.0f, float pass = 1.0f,
                       float block = 1.0f, float steal = 1.0f, float rebound = 1.0f,
                       float dribble = 1.0f)
    {
        bonusTwoPoint = tp;
        bonusThreePoint = three;
        bonusPass = pass;
        bonusBlock = block;
        bonusSteal = steal;
        bonusRebound = rebound;
        bonusDribble = dribble;
    }
}


public class MatchDataProxy : MonoBehaviour
{
    public static MatchDataProxy Instance { get; private set; }

    [Header("Balance Settings")]
    [SerializeField] private int W_Shot_Base = 1;     // 101: 슛 기본 가중치
    [SerializeField] private int W_Pass_Base = 1;     // 102: 패스 기본 가중치
    [SerializeField] private int W_Dribble_Base = 1;  // 103: 드리블 기본 가중치
    [SerializeField] private float Pen_Dist_Hoop = 0.5f; // 104: 골대 거리 페널티 계수
    [SerializeField] private float Pen_Def_Block = 1f;   // 105: 수비 블록 페널티 계수
    [SerializeField] private float Pen_Def_Steal = 1f;   // 106: 수비 스틸 페널티 계수
    [SerializeField] private int W_Default = 1;       // 107: 기본값

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public float GetBalance(string key)
    {
        switch (key)
        {
            case "W_Shot_Base": return W_Shot_Base;
            case "W_Pass_Base": return W_Pass_Base;
            case "W_Dribble_Base": return W_Dribble_Base;
            case "Pen_Dist_Hoop": return Pen_Dist_Hoop;
            case "Pen_Def_Block": return Pen_Def_Block;
            case "Pen_Def_Steal": return Pen_Def_Steal;
            case "W_Default": return W_Default;
            default:
                Debug.LogError($"[MatchDataProxy] 알 수 없는 키값: {key}");
                return 0f;
        }
    }

    public TeamTactics GetTactics(string teamColorId)
    {
        switch (teamColorId)
        {
            case "TC_DEF_Base":
            case "TC_OFF_Base":
            case "TC_BAL_Base":
            case "TC_SHT_Base":
            case "TC_SHT_Sniper":
            case "TC_TAC_Base":
            case "TC_BIG_Base":
            case "TC_SML_Base":
            default:
                return new TeamTactics();
        }
    }
}
