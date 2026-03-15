using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 리그 관련 마스터 데이터 조회 / 참가 팀 선정 / 리그 저장 데이터 생성 담당
/// </summary>
public class LeagueDataManager : Singleton<LeagueDataManager>
{
    private LeagueFactory _leagueFactory;

    protected override void Awake()
    {
        base.Awake();
        _leagueFactory = GetComponent<LeagueFactory>();
    }

    /// <summary>
    /// weekId에 해당하는 팀 선정 룰 반환
    /// </summary>
    public League_TeamData? GetTeamSelectionRuleByWeekId(int weekId)
    {
        if (_leagueFactory == null) return null;

        var dataList = _leagueFactory.GetTeamSelectionDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.weekId == weekId)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 리그 ID로 리그 마스터 데이터 조회
    /// </summary>
    public League_MasterData? GetMasterDataById(string leagueId)
    {
        if (_leagueFactory == null) return null;
        if (string.IsNullOrEmpty(leagueId)) return null;

        var dataList = _leagueFactory.GetMasterDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.leagueId == leagueId)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 팀 ID로 라이벌 마스터 데이터 조회
    /// </summary>
    public Rival_MasterData? GetRivalMasterDataById(string teamId)
    {
        if (_leagueFactory == null) return null;
        if (string.IsNullOrEmpty(teamId)) return null;

        var dataList = _leagueFactory.GetRivalMasterDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.teamId == teamId)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 아키타입 ID로 아키타입 데이터 조회
    /// </summary>
    public Team_ArchetypeData? GetArchetypeDataById(string teamArchetypeId)
    {
        if (_leagueFactory == null) return null;
        if (string.IsNullOrEmpty(teamArchetypeId)) return null;

        var dataList = _leagueFactory.GetArchetypeDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.teamArchetypeId == teamArchetypeId)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 리그 레벨 ID로 레벨 데이터 조회
    /// </summary>
    public League_LevelData? GetLeagueLevelDataById(string leagueLevelId)
    {
        if (_leagueFactory == null) return null;
        if (string.IsNullOrEmpty(leagueLevelId)) return null;

        var dataList = _leagueFactory.GetLevelDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.leagueLevelId == leagueLevelId)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 룰에 따라 리그 참가 팀 선정
    /// </summary>
    public List<string> CreateLeagueTeams(League_TeamData? rule)
    {
        if (_leagueFactory == null) return null;
        if (rule == null) return null;

        var allTeams = _leagueFactory.GetRivalMasterDataList();
        if (allTeams == null || allTeams.Count == 0) return null;

        var ruleData = rule.Value;

        var priorityTeamIds = new List<string>();
        string playerTeamId = "Player_Team";        // 이건 임시 플레이어 팀 ID (아마 변경될 가능성이 높을듯)

        int seed = ruleData.weekId;
        var selector = new LeagueTeamSelector(seed);

        var selectedTeams = selector.SelectTeams(
            ruleData,
            allTeams,
            priorityTeamIds,
            playerTeamId
        );

        return selectedTeams;
    }

    /// <summary>
    /// 리그 저장 데이터 생성
    /// </summary>
    public LeagueSaveData CreateLeagueSaveData(string leagueId, League_TeamData? rule)
    {
        if (string.IsNullOrEmpty(leagueId)) return null;
        if (rule == null) return null;

        var masterData = GetMasterDataById(leagueId);
        if (masterData == null)
        {
            Debug.LogError($"리그 마스터 데이터를 찾을 수 없습니다. leagueId = {leagueId}");
            return null;
        }

        var selectedTeams = CreateLeagueTeams(rule);
        if (selectedTeams == null || selectedTeams.Count == 0)
        {
            Debug.LogError($"리그 참가 팀 생성 실패. leagueId = {leagueId}");
            return null;
        }

        // 저장될 데이터 디폴트 값
        var saveData = new LeagueSaveData
        {
            leagueId = leagueId,
            leagueType = masterData.Value.leagueType.ToString(),
            currentRoundIndex = 0,
            isFinished = false,
            isPlayerEliminated = false,
            teams = CreateTeamEntries(selectedTeams),
            matchRecords = CreateMatchRecords(selectedTeams),
            standings = CreateInitialStandings(selectedTeams)
        };

        return saveData;
    }

    /// <summary>
    /// 리그 생성 후 즉시 저장
    /// </summary>
    public LeagueSaveData CreateAndSaveLeague(string leagueId, League_TeamData? rule)
    {
        var saveData = CreateLeagueSaveData(leagueId, rule);
        if (saveData == null) return null;

        SaveLeague(saveData);
        return saveData;
    }

    /// <summary>
    /// 리그 데이터 저장
    /// </summary>
    public void SaveLeague(LeagueSaveData saveData)
    {
        if (SaveLoadManager.Instance == null) return;
        if (saveData == null) return;
        if (string.IsNullOrEmpty(saveData.leagueId)) return;

        // 해당 경로에 존재하는 
        string path = GetLeagueSavePath(saveData.leagueId);
        SaveLoadManager.Instance.Save<LeagueSaveData>(path, saveData);
    }

    /// <summary>
    /// 리그 데이터 로드
    /// </summary>
    public LeagueSaveData LoadLeague(string leagueId)
    {
        if (SaveLoadManager.Instance == null) return null;
        if (string.IsNullOrEmpty(leagueId)) return null;

        string path = GetLeagueSavePath(leagueId);
        bool loaded = SaveLoadManager.Instance.TryLoad<LeagueSaveData>(path, out var data);

        if (!loaded)
            return null;

        return data;
    }

    private string GetLeagueSavePath(string leagueId)
    {
        return $"{leagueId}.json";
    }

    /// <summary>
    /// 참가 팀 엔트리 생성
    /// </summary>
    private List<LeagueTeamEntry> CreateTeamEntries(List<string> selectedTeamIds)
    {
        var result = new List<LeagueTeamEntry>();
        if (selectedTeamIds == null) return result;

        for (int i = 0; i < selectedTeamIds.Count; i++)
        {
            string teamId = selectedTeamIds[i];

            result.Add(new LeagueTeamEntry
            {
                teamId = teamId,
                isPlayerTeam = teamId == "Player_Team",
                isEliminated = false
            });
        }

        return result;
    }

    /// <summary>
    /// 초기 순위 데이터 생성
    /// </summary>
    private List<LeagueStandingData> CreateInitialStandings(List<string> selectedTeamIds)
    {
        var result = new List<LeagueStandingData>();
        if (selectedTeamIds == null) return result;

        for (int i = 0; i < selectedTeamIds.Count; i++)
        {
            result.Add(new LeagueStandingData
            {
                teamId = selectedTeamIds[i],
                rank = i + 1,
                played = 0,
                win = 0,
                lose = 0,
                points = 0,
                scored = 0,
                conceded = 0,
                goalDiff = 0
            });
        }

        return result;
    }

    /// <summary>
    /// 경기표 생성
    /// 일단 단순 라운드 로빈 예시.
    /// 실제 규칙에 맞게 수정 필요.
    /// </summary>
    private List<LeagueMatchRecord> CreateMatchRecords(List<string> selectedTeamIds)
    {
        var result = new List<LeagueMatchRecord>();
        if (selectedTeamIds == null || selectedTeamIds.Count < 2) return result;

        int roundIndex = 0;

        for (int i = 0; i < selectedTeamIds.Count; i++)
        {
            for (int j = i + 1; j < selectedTeamIds.Count; j++)
            {
                result.Add(new LeagueMatchRecord
                {
                    roundIndex = roundIndex,
                    homeTeamId = selectedTeamIds[i],
                    awayTeamId = selectedTeamIds[j],
                    isPlayed = false,
                    homeScore = 0,
                    awayScore = 0,
                    specialNote = string.Empty,
                    replayLogKey = string.Empty
                });

                roundIndex++;
            }
        }

        return result;
    }

    /// <summary>
    /// 레벨 데이터에서 티어별 가중치 반환
    /// </summary>
    public float GetWeightByTier(League_LevelData data, teamTier tier)
    {
        switch (tier)
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

    /// <summary>
    /// 아키타입 데이터에서 잠재력별 가중치 반환
    /// </summary>
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