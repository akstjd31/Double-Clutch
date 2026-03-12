using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 데이터들의 리더 데이터 리스트를 관리하는 클래스
/// </summary>
public class LeagueFactory : MonoBehaviour
{
    [Header("Reader")]
    [SerializeField] private League_MasterDataReader _leagueMasterReader;
    [SerializeField] private League_TeamDataReader _leagueTeamSelectionReader;
    [SerializeField] private Rival_MasterDataReader _rivalMasterReader;
    [SerializeField] private Team_ArchetypeDataReader _archetypeReader;
    [SerializeField] private League_LevelDataReader _leagueLevelReader;
    [SerializeField] private League_RewardDataReader _leagueRewardReader;

    public List<League_MasterData> GetMasterDataList()
    {
        if (_leagueMasterReader == null) return null;
        return _leagueMasterReader.DataList;
    }

    public List<League_TeamData> GetTeamSelectionDataList()
    {
        if (_leagueTeamSelectionReader == null) return null;
        return _leagueTeamSelectionReader.DataList;
    }

    public List<Rival_MasterData> GetRivalMasterDataList()
    {
        if (_rivalMasterReader == null) return null;
        return _rivalMasterReader.DataList;
    }

    public List<Team_ArchetypeData> GetArchetypeDataList()
    {
        if (_archetypeReader == null) return null;
        return _archetypeReader.DataList;
    }

    public List<League_LevelData> GetLevelDataList()
    {
        if (_leagueLevelReader == null) return null;
        return _leagueLevelReader.DataList;
    }

    public List<League_RewardData> GetRewardDataList()
    {
        if (_leagueRewardReader == null) return null;
        return _leagueRewardReader.DataList;
    }
}
