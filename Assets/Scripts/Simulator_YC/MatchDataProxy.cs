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
    [SerializeField] private float W_Pass_Base = 1;     // 102: 패스 기본 가중치
    [SerializeField] private int W_Dribble_Base = 1;  // 103: 드리블 기본 가중치
    [SerializeField] private float Pen_Dist_Hoop = 0.5f; // 104: 골대 거리 페널티 계수
    [SerializeField] private float Pen_Def_Block = 1f;   // 105: 수비 블록 페널티 계수
    [SerializeField] private float Pen_Def_Steal = 1f;   // 106: 수비 스틸 페널티 계수
    [SerializeField] private int W_Default = 1;       // 107: 기본값
    [SerializeField] private float Pen_Intercept_Dist = 0.03f; // 패스 차단 판정 거리 변수
    [SerializeField] private float Min_Shoot_Score = 25f; // 슛 시도점수 최소 보장
    [SerializeField] private float Def_Block_Dist = 0.15f; // 슛 수비(블록) 판정 거리 변수
    [SerializeField] private float W_Block_Base = 1f;
    [SerializeField] private float W_Steal_Base = 1f;
    [SerializeField] private float W_Dribble_Bonus = 1.5f;
    [SerializeField] private float W_Dist_Bonus = 1.2f;
    [SerializeField] private float R_Reb_Ball = 0.35f;
    [SerializeField] private float R_Reb_Cand = 0.35f;
    [SerializeField] private float C_gamma = 1.87f;

    [Header("Data Readers")]
    [SerializeField] private Team_ArchetypeDataReader _archetypeReader;
    [SerializeField] private Balance_DataReader _balanceReader;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        ApplyBalanceData();
    }
    public void ApplyBalanceData()
    {
        if (_balanceReader == null || _balanceReader.DataList == null) return;

        foreach (var data in _balanceReader.DataList)
        {
            switch (data.weightId)
            {
                case "W_Shot_Base": W_Shot_Base = (int)data.value; break;
                case "W_Pass_Base": W_Pass_Base = data.value; break;
                case "W_Dribble_Base": W_Dribble_Base = (int)data.value; break;
                case "Pen_Dist_Hoop": Pen_Dist_Hoop = data.value; break;
                case "Pen_Def_Block": Pen_Def_Block = data.value; break;
                case "Pen_Def_Steal": Pen_Def_Steal = data.value; break;
                case "W_Default": W_Default = (int)data.value; break;
                case "Pen_Intercept_Dist": Pen_Intercept_Dist = data.value; break;
                case "Min_Shoot_Score": Min_Shoot_Score = data.value; break;
                case "Def_Block_Dist": Def_Block_Dist = data.value; break;
                case "W_Block_Base": W_Block_Base = data.value; break;
                case "W_Steal_Base": W_Steal_Base = data.value; break;
                case "W_Dribble_Bonus": W_Dribble_Bonus = data.value; break;
                case "W_Dist_Bonus": W_Dist_Bonus = data.value; break;
                case "R_Reb_Ball": R_Reb_Ball = data.value; break;
                case "R_Reb_Cand": R_Reb_Cand = data.value; break;
                case "C_gamma": C_gamma = data.value; break;
            }
        }
        Debug.Log("[MatchDataProxy] 밸런스 엑셀 데이터 연동 완료!");
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
            case "Pen_Intercept_Dist": return Pen_Intercept_Dist;
            case "Min_Shoot_Score": return Min_Shoot_Score;
            case "Def_Block_Dist": return Def_Block_Dist;
            case "W_Block_Base": return W_Block_Base;
            case "W_Steal_Base": return W_Steal_Base;
            case "W_Dribble_Bonus": return W_Dribble_Bonus;
            case "W_Dist_Bonus": return W_Dist_Bonus;
            case "R_Reb_Ball": return R_Reb_Ball;
            case "R_Reb_Cand": return R_Reb_Cand;
            case "C_gamma": return C_gamma;
            default:
                Debug.LogError($"[MatchDataProxy] 알 수 없는 키값: {key}");
                return 0f;
        }
    }

    public TeamTactics GetTactics(string teamColorId)
    {
        if (_archetypeReader == null || _archetypeReader.DataList.Count == 0)
        {
            return new TeamTactics(1f, 1f, 1f, 1f, 1f, 1f, 1f);
        }

        // 테이블에서 일치하는 전술 아키타입 검색
        var archetype = _archetypeReader.DataList.Find(x => x.teamArchetypeId == teamColorId);

        if (string.IsNullOrEmpty(archetype.teamArchetypeId))
        {
            return new TeamTactics(1f, 1f, 1f, 1f, 1f, 1f, 1f);
        }

        // 테이블 데이터 기반으로 가중치 적용 (드리블은 테이블에 없으므로 기본값 1.0f)
        return new TeamTactics(
            tp: archetype.weight2pt,
            three: archetype.weight3pt,
            pass: archetype.weightPass,
            block: archetype.weightBlock,
            steal: archetype.weightSteal,
            rebound: archetype.weightRebound,
            dribble: 1.0f
        );
    }
}
