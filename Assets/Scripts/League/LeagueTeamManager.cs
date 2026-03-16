using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 라이벌 팀 일괄 생성 및 관리
/// 리그 돌입 전 라이벌 스탯 일괄 재부여.
/// </summary>

public class LeagueTeamManager : Singleton<LeagueTeamManager>
{
    const string SAVE_FILE = "TeamSave.json";

    // 리그 전 캐싱 데이터에 담긴 선수 목록  
    [SerializeField] List<string> _currentLeagueTeamIdList = new List<string>();
    List<Team> _currentLeagueTeamList = new List<Team>();
    Dictionary<string, Team> _currentTeamDict = new Dictionary<string, Team>();    

    public Team GetTeamById(string teamId)
    {
        // 딕셔너리에 해당 팀이 존재하면 안전하게 꺼내서 반환
        if (_currentTeamDict.TryGetValue(teamId, out Team team))
        {
            return team;
        }

        // 만약 리그 추첨에서 떨어졌거나 하드코딩으로 인해 팀을 찾을 수 없다면?
        Debug.LogWarning($"[LeagueTeamManager] '{teamId}' 팀이 현재 참가 명단에 없습니다! 게임 튕김을 방지하기 위해 임의의 NPC 팀을 반환합니다.");

        // 참가 중인 팀 리스트를 뒤져서, 유저 팀이 아닌 첫 번째 NPC 팀을 대신 던져줍니다.
        foreach (var fallbackTeam in _currentLeagueTeamList)
        {
            if (fallbackTeam.TeamId != StudentManager.TEAM_ID)
            {
                return fallbackTeam;
            }
        }

        return null; // 최악의 경우 (리그에 NPC가 아예 0명일 때)
    }

    public List<Team> GetAllTeams()
    {
        return _currentLeagueTeamList;
    }

    // 저장된 데이터로 각 팀의 Id 받아오기
    public void InitDatas(LeagueSaveData saveData) 
    {
        _currentLeagueTeamIdList.Clear();

        // 어차피 플레이어도 추가되어 있기 떄문에 따로 추가 작업 안해도 됨.
        foreach (var teamEntryInfo in saveData.teams)
        {
            _currentLeagueTeamIdList.Add(teamEntryInfo.teamId);
        }

        BuildTeams(saveData);
    }

    private void BuildTeams(LeagueSaveData saveData) //모든 팀 일괄 생성하고 딕셔너리에 채워넣기
    {
        _currentTeamDict.Clear();
        _currentLeagueTeamList.Clear();    

        var teamDataList = LeagueDataManager.Instance.GetRivalDatasByTeamIds(_currentLeagueTeamIdList);
        var archDataList = LeagueDataManager.Instance.GetFactory().GetArchetypeDataList();

        foreach (var master in teamDataList)
        {           
            Team newTeam = null; 
            if (master.teamId.Equals(LeagueManager.PLAYER_TEAM_ID))
                newTeam = StudentManager.Instance.CurrentTeam;
            else
                newTeam = new Team(master.teamId);

            // 매니저가 데이터를 찾아서 직접 주입 (Dependency Injection)
            var arch = archDataList.Find(x => x.teamArchetypeId == master.teamArchetypeId);
            newTeam.Init(master, arch);
            _currentLeagueTeamList.Add(newTeam);
            _currentTeamDict.Add(newTeam.TeamId, newTeam);
            FillRivalStudents(newTeam);
        }

        SaveGame();
    }

    //모든 라이벌 팀의 레벨보정 잠재력 재설정(라이벌 스탯 결정 시기에 호출)
    // + 플레이어 팀의 티어 재설정
    public void RefreshAllRivalStats(string leagueLevelId, bool isPassiveOn) 
    {
        foreach (var team in _currentTeamDict.Values)
        {
            if (team.IsPlayable)
            {
                continue;
            }
            // 각 팀원 5명에 대한 스탯을 생성하여 주입
            List<Stat>[] teamStats = new List<Stat>[5];
            for (int i = 0; i < 5; i++)
            {
                teamStats[i] = GetRivalStatsByLevel(leagueLevelId, team);
            }            
            team.UpdateTeamStats(teamStats, isPassiveOn);
        }
        League_LevelData levelData = LeagueDataManager.Instance.GetLeagueLevelDataById(leagueLevelId).Value;
        //StudentManager.Instance.CurrentTeam.SetTier(levelData.playerTeamTier);
        SaveGame();
    }
    public void SaveGame()
    {        
        TeamSaveData saveData = new TeamSaveData(_currentLeagueTeamList);
        
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.Save(SAVE_FILE, saveData);
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance.TryLoad<TeamSaveData>(SAVE_FILE, out var data))
        {
            // 1. 리스트 데이터 먼저 복구
            _currentLeagueTeamList = data.teamList;

            // 2. 리스트 내에서 플레이어 팀을 찾아 StudentManager의 실시간 객체로 교체
            string pId = StudentManager.Instance.CurrentTeam.TeamId;
            int idx = _currentLeagueTeamList.FindIndex(x => x.TeamId == pId);

            if (idx != -1)
            {
                _currentLeagueTeamList[idx] = StudentManager.Instance.CurrentTeam;
            }

            // 3. 교체 완료된 리스트를 기반으로 딕셔너리 생성 (참조 동기화 완료됨)
            MatchKeyAndTeams();

            Debug.Log("팀 데이터 로드 및 참조 동기화 완료");
        }
    }


    #region 내부 함수

    private void MatchKeyAndTeams() //로드 직후 수행
    {       
        _currentTeamDict.Clear();
        for (int i = 0; i < _currentLeagueTeamList.Count; i++)
        {
            _currentTeamDict.Add(_currentLeagueTeamList[i].TeamId, _currentLeagueTeamList[i]);
        }
    }

    private void FillRivalStudents(Team team) //팀에 선수들 채워넣는 매서드
    {
        List<speciesType> speciesList = GenerateSpeciesList(team.Rival_MasterData.Value);
        var studentFactory = StudentManager.Instance.GetFactory();

        for (int i = 0; i < team.Members.Length; i++)
        {
            Student rival = studentFactory.MakeRivalStudentSkeleton(team.Rival_MasterData.Value.nation);
            team.SetMember(i, rival);
            rival.SetSpecie(studentFactory.GetRandomSpecieByType(speciesList[i]));
            rival.SetVisual(studentFactory.GetRandomVisual(rival.SpecieId));
            Position targetPos = team.Positions[i];
            rival.SetPosition(targetPos);
            rival.SetMatchPosition(targetPos);            
        }
    }

    //리그 레벨과 해당 팀에 알맞는 스탯 생성해서 반환
    private List<Stat> GetRivalStatsByLevel(string leagueLevelId, Team team)
    {
        List<Stat> newStats = new List<Stat>();

        League_LevelData levelData =  LeagueDataManager.Instance.GetLeagueLevelDataById(leagueLevelId).Value;
        Team_ArchetypeData archData = LeagueDataManager.Instance.GetArchetypeDataById(team.Team_ArchetypeData.Value.teamArchetypeId).Value;

        // 리그 레벨에 따른 기본 스탯 범위 (예: 10~20 사이에서 랜덤)
        int minBase = levelData.minPotential;
        int maxBase = levelData.maxPotential;

        foreach (potential type in System.Enum.GetValues(typeof(potential)))
        {
            if (type == potential.None) continue;

            // 1. 해당 리그 레벨의 기본 범위 내에서 난수 생성
            float baseRandom = UnityEngine.Random.Range(minBase, maxBase + 1);

            // 2. 팀 티어 가중치 가져오기
            float archWeight = LeagueDataManager.Instance.GetWeightByTier(levelData, team.Rival_MasterData.Value.teamTier);

            // 3. 팀의 잠재력 가중치 가져오기
            float potenWeight = LeagueDataManager.Instance.GetWeightByPotential(archData, type);

            // 3. 최종 계산: (기본 난수 * 가중치)
            int finalValue = Mathf.FloorToInt(baseRandom * archWeight * potenWeight);
            finalValue = Mathf.Clamp(finalValue, 1, 100);

            // 4. 기타 수치(라이벌에서는 쓰지 않는 성장률, 최대잠재력 수치)
            int growthRate = 1;
            int potentialLimit = finalValue;

            newStats.Add(new Stat(type, finalValue, potentialLimit, growthRate));
        }

        return newStats;
    }
    private List<speciesType> GenerateSpeciesList(Rival_MasterData rivalData)
    {
        List<speciesType> result = new List<speciesType>();

        //  최소 인원만큼 무조건 배정 (Android로 선언하신 변수명 사용)
        for (int i = 0; i < rivalData.minHumanoidCount; i++) result.Add(speciesType.Humanoid);
        for (int i = 0; i < rivalData.minHumanCount; i++) result.Add(speciesType.Human);
        for (int i = 0; i < rivalData.minAnimalCount; i++) result.Add(speciesType.Animal);

        //  남은 자리는 가중치에 따라 랜덤 배정
        int remain = 5 - result.Count;
        float totalWeight = rivalData.weightHumanoid + rivalData.weightHuman + rivalData.weightAnimal;

        for (int i = 0; i < remain; i++)
        {
            if (totalWeight <= 0)
            {
                result.Add(speciesType.Human); // 가중치 합이 0일 경우의 안전 장치
                continue;
            }

            float rand = UnityEngine.Random.Range(0f, totalWeight);
            if (rand < rivalData.weightHumanoid)
                result.Add(speciesType.Humanoid);
            else if (rand < rivalData.weightHumanoid + rivalData.weightHuman)
                result.Add(speciesType.Human);
            else
                result.Add(speciesType.Animal);
        }

        // 포지션별로 랜덤하게 들어갈 수 있도록 리스트 셔플
        for (int i = 0; i < result.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(0, result.Count);
            var temp = result[i];
            result[i] = result[rnd];
            result[rnd] = temp;
        }

        return result;
    }

    #endregion


}
