using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 팩토리에 있는 데이터를 가지고 원하는 데이터 추출
/// </summary>
public class LeagueManager : Singleton<LeagueManager>
{
    private LeagueFactory _leagueFactory;

    protected override void Awake()
    {
        base.Awake();
        _leagueFactory = this.GetComponent<LeagueFactory>();
    }

    public List<string> CreateLeagueTeams(League_TeamData rule)
    {
        if (_leagueFactory == null) return null;
        var allTeams = _leagueFactory.GetRivalMasterDataList();

        var priorityTeamIds = GetPriorityTeams(rule);
        string playerTeamId = "Player_Team";    // 임시
        
        int seed = rule.weekId; // 필요에 따라 사용
        var selector = new LeagueTeamSelector(seed);

        var selectedTeams = selector.SelectTeams
        (
            rule,
            allTeams,
            priorityTeamIds,
            playerTeamId
        );

        return selectedTeams;
    }

    // 이전 리그 상위팀 가져오기
    private List<string> GetPriorityTeams(League_TeamData rule)
    {
        return null;
    }

    // 위크 ID를 매개변수로 받아 리그 셀렉션 테이블에서 데이터 생성 날짜인지 확인
    public bool IsCachingDay(int weekId)
    {
        if (_leagueFactory == null) return false;

        var dataList = _leagueFactory.GetTeamSelectionDataList();

        foreach (var data in dataList)
        {
            if (data.weekId.Equals(weekId))
                return true;
        }
        return false;
    }

    // 리그 ID를 비교하여 해당 데이터 반환
    public League_MasterData? GetMasterDataById(string leagueId)
    {
        if (_leagueFactory == null) return null;

        var dataList = _leagueFactory.GetMasterDataList();

        foreach (var data in dataList)
        {
            if (data.leagueId.Equals(leagueId))
                return data;
        }

        return null;
    }

    //팀 ID를 비교하여 해당 팀 데이터 반환
    public Rival_MasterData? GetRivalMasterDataById(string teamId)
    {
        if (_leagueFactory == null) return null;

        var dataList = _leagueFactory.GetRivalMasterDataList();

        foreach (var data in dataList)
        {
            if (data.teamId.Equals(teamId))
                return data;
        }

        return null;
    }
    //아키타입 ID를 비교하여 해당 아키타입 데이터 반환
    public Team_ArchetypeData? GetArchetypeDataById(string teamArchetypeId)
    {
        if (_leagueFactory == null) return null;

        var dataList = _leagueFactory.GetArchetypeDataList();

        foreach (var data in dataList)
        {
            if (data.teamArchetypeId.Equals(teamArchetypeId))
                return data;
        }

        return null;
    }

    //아키타입 ID를 비교하여 해당 아키타입 데이터 반환
    public League_LevelData? GetLeagueLevelDataById(string leagueLevelId)
    {
        if (_leagueFactory == null) return null;

        var dataList = _leagueFactory.GetLevelDataList();

        foreach (var data in dataList)
        {
            if (data.leagueLevelId.Equals(leagueLevelId))
                return data;
        }

        return null;
    }

    //정해진 레벨 데이터 안에서 티어별 가중치 반환
    public float GetWeightByTier(League_LevelData data, teamTier tier)
    {
        switch(tier)
        {            
            case teamTier.D: return data.weightPotentialTierD;
            case teamTier.C: return data.weightPotentialTierC;
            case teamTier.B: return data.weightPotentialTierB;
            case teamTier.A: return data.weightPotentialTierA;
            case teamTier.S: return data.weightPotentialTierS;
            case teamTier.SS: return data.weightPotentialTierSS;
            case teamTier.SSS: return data.weightPotentialTierSSS;
            default: return 0f;                
        }
    }

    //정해진 아키타입 데이터 안에서 잠재력별 가중치 반환
    public float GetWeightByPotential(Team_ArchetypeData data, potential potential)
    {
        switch (potential)
        {
            case potential.Stat2pt: return data.weight2pt;
            case potential.Stat3pt: return data.weight3pt;
            case potential.StatPass: return data.weightPass;
            case potential.StatSteal: return data.weightSteal;
            case potential.StatBlock: return data.weightBlock;
            case potential.StatRebound: return data.weightRebound;
            
            default: return 0f;
        }
    }
}