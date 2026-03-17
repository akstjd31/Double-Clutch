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
    /// 팀 ID가 담긴 리스트들을 해당 테이블에서 찾는 메서드
    /// </summary>
    public List<Rival_MasterData> GetRivalDatasByTeamIds(List<string> teamIds)
    {
        if (_leagueFactory == null) return null;

        var dataList = _leagueFactory.GetRivalMasterDataList();
        if (dataList == null) return null;

        var rivalList = new List<Rival_MasterData>();
        foreach (var data in dataList)
        {
            if (teamIds.Contains(data.teamId))
            {
                rivalList.Add(data);
            }
        }

        return rivalList;
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
    /// 지원금 계산 후 반영
    /// </summary>
    public int CalculateLeagueMoney(string leagueId, LeagueStandingData playerStanding)
    {
        if (_leagueFactory == null) return 0;
        if (string.IsNullOrEmpty(leagueId)) return 0;

        var dataList = _leagueFactory.GetRewardDataList();
        int resultMoney = 0;

        foreach (var data in dataList)
        {
            if (data.leagueRewardId.Equals(leagueId))
            {
                // (승리횟수 * rewardGoldEach) + (패배횟수 * rewardGoldEach * rewardGoldMultiplier) + (우승여부 * rewardGoldWin)
                resultMoney = (playerStanding.win * data.rewardGoldEach) +
                         (int)(playerStanding.lose * data.rewardGoldEach * data.rewardGoldMultiplier) +
                         ((playerStanding.rank == 1 ? 1 : 0) * data.rewardGoldWin);

                return resultMoney;
            }
        }

        return resultMoney;
    }

    public int CalculateLeagueFame(string leagueId, LeagueStandingData playerStanding)
    {
        if (_leagueFactory == null) return 0;
        if (string.IsNullOrEmpty(leagueId)) return 0;

        var dataList = _leagueFactory.GetRewardDataList();
        int resultFame = 0;

        // 선수별 명성 누적값은 선수 데이터가 필요

        return resultFame;
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
        string playerTeamId = LeagueManager.PLAYER_TEAM_ID;

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
            matchRecords = new List<LeagueMatchRecord>(),
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
        LeagueManager.Instance.StartLeague(saveData);


        var leagueTeamMgr = LeagueTeamManager.Instance;
        if (leagueTeamMgr == null) return null;
        leagueTeamMgr.InitDatas(saveData);

        var masterData = GetMasterDataById(leagueId);
        leagueTeamMgr.RefreshAllRivalStats(masterData.Value.leagueLevelId, IsPassiveApplied(masterData.Value.leagueLevelId));

        return saveData;
    }


    /// <summary>
    /// 리그레벨ID로 패시브 유무 확인하기
    /// </summary>
    private bool IsPassiveApplied(string leagueLvId)
    {
        if (_leagueFactory == null) return false;

        var dataList = _leagueFactory.GetLevelDataList();
        foreach (var data in dataList)
        {
            if (data.leagueLevelId.Equals(leagueLvId))
                return data.isRivalPassiveApplied;
        }

        return false;
    }

    /// <summary>
    /// 리그 데이터 저장
    /// </summary>
    public void SaveLeague(LeagueSaveData saveData)
    {
        if (SaveLoadManager.Instance == null) return;
        if (saveData == null) return;
        if (string.IsNullOrEmpty(saveData.leagueId)) return;

        SaveLoadManager.Instance.Save<LeagueSaveData>(FilePath.LEAGUE_PATH, saveData);
    }

    /// <summary>
    /// 리그 데이터 로드
    /// </summary>
public LeagueSaveData LoadLeague()
{
    if (SaveLoadManager.Instance == null)
    {
        Debug.Log("SaveLoadManager.Instance == null");
        return null;
    }

    bool loaded = SaveLoadManager.Instance.TryLoad<LeagueSaveData>(FilePath.LEAGUE_PATH, out var data);

    Debug.Log($"loaded : {loaded}");

    if (!loaded)
    {
        Debug.Log("리그 저장 데이터 없음");
        return null;
    }

    if (data == null)
    {
        Debug.Log("로드는 성공했지만 data == null");
        return null;
    }

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

    /// <summary>
    /// 리그 보상 ID로 보상 데이터 조회
    /// </summary>
    public League_RewardData? GetLeagueRewardDataById(string rewardId)
    {
        if (_leagueFactory == null) return null;
        if (string.IsNullOrEmpty(rewardId)) return null;

        var dataList = _leagueFactory.GetRewardDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.leagueRewardId == rewardId)
                return data;
        }

        return null;
    }
    /// <summary>
    /// Rule ID(teamSelectionRuleId)로 팀 선정 룰 반환
    /// </summary>
    public League_TeamData? GetTeamSelectionRuleById(string ruleId)
    {
        if (_leagueFactory == null) return null;
        if (string.IsNullOrEmpty(ruleId)) return null;

        var dataList = _leagueFactory.GetTeamSelectionDataList();
        if (dataList == null) return null;

        foreach (var data in dataList)
        {
            if (data.teamSelectionRuleId == ruleId)
                return data;
        }

        return null;
    }
    public LeagueFactory GetFactory() => _leagueFactory;

}