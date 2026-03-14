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
    [SerializeField] LeagueFactory _leagueFactory;
    [SerializeField] StudentFactory _studentFactory;

    //플레이어 팀을 포함한 모든 팀 목록
    List<string> _allTeamIdList = new List<string>();    
    List<Team> _allTeamList = new List<Team>();
    SerializedDictionary<string, Team> _allTeamDict = new SerializedDictionary<string, Team>();
    

    private void Start()
    {
        InitDatas();
    }

    private void InitDatas() //리스트에 아이디만 채워넣기
    {
        _allTeamIdList.Clear();
        for (int i = 0; i < _leagueFactory.GetRivalMasterDataList().Count; i++)
        {
            _allTeamIdList.Add(_leagueFactory.GetRivalMasterDataList()[i].teamId);
        }
        _allTeamIdList.Add(StudentManager.Instance.CurrentTeam.TeamId);
    }

    public void BuildTeams() //모든 팀 일괄 생성하고 딕셔너리에 채워넣기
    {
        _allTeamDict.Clear();
        _allTeamList.Clear();

        var teamDataList = _leagueFactory.GetRivalMasterDataList();
        var archDataList = _leagueFactory.GetArchetypeDataList();

        foreach (var master in teamDataList)
        {
            if (_allTeamDict.ContainsKey(master.teamId)) continue;
            

            Team newTeam = new Team(master.teamId);

            // 매니저가 데이터를 찾아서 직접 주입 (Dependency Injection)
            var arch = archDataList.Find(x => x.teamArchetypeId == master.teamArchetypeId);
            newTeam.Init(master, arch);
            _allTeamList.Add(newTeam);
            _allTeamDict.Add(newTeam.TeamId, newTeam);
            FillRivalStudents(newTeam);
        }
        Team playerTeam = StudentManager.Instance.CurrentTeam;
        _allTeamList.Add(playerTeam);
        _allTeamDict.Add(playerTeam.TeamId, playerTeam);
        SaveGame();
    }

    

    //모든 라이벌 팀의 레벨보정 잠재력 재설정(라이벌 스탯 결정 시기에 호출)
    // + 플레이어 팀의 티어 재설정
    public void RefreshAllRivalStats(string leagueLevelId) 
    {
        foreach (var team in _allTeamDict.Values)
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
            team.UpdateTeamStats(teamStats);
        }
        League_LevelData levelData = LeagueManager.Instance.GetLeagueLevelDataById(leagueLevelId).Value;
        //StudentManager.Instance.CurrentTeam.SetTier(levelData.playerTeamTier);
        SaveGame();
    }
    public void SaveGame()
    {        
        TeamSaveData saveData = new TeamSaveData(_allTeamList);
        
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.Save(SAVE_FILE, saveData);
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance.TryLoad<TeamSaveData>(SAVE_FILE, out var data))
        {
            // 1. 리스트 데이터 먼저 복구
            _allTeamList = data.teamList;

            // 2. 리스트 내에서 플레이어 팀을 찾아 StudentManager의 실시간 객체로 교체
            string pId = StudentManager.Instance.CurrentTeam.TeamId;
            int idx = _allTeamList.FindIndex(x => x.TeamId == pId);

            if (idx != -1)
            {
                _allTeamList[idx] = StudentManager.Instance.CurrentTeam;
            }

            // 3. 교체 완료된 리스트를 기반으로 딕셔너리 생성 (참조 동기화 완료됨)
            MatchKeyAndTeams();

            Debug.Log("팀 데이터 로드 및 참조 동기화 완료");
        }
    }


    #region 내부 함수

    private void MatchKeyAndTeams() //로드 직후 수행
    {       
        _allTeamDict.Clear();
        for (int i = 0; i < _allTeamList.Count; i++)
        {
            _allTeamDict.Add(_allTeamList[i].TeamId, _allTeamList[i]);
        }
    }

    private void FillRivalStudents(Team team) //팀에 선수들 채워넣는 매서드
    {
        List<speciesType> speciesList = GenerateSpeciesList(team.Rival_MasterData.Value);

        for (int i = 0; i < team.Members.Length; i++)
        {
            Student rival = _studentFactory.MakeRivalStudentSkeleton(team.Rival_MasterData.Value.nation);
            team.SetMember(i, rival);
            rival.SetSpecie(_studentFactory.GetRandomSpecieByType(speciesList[i]));
            rival.SetVisual(_studentFactory.GetRandomVisual(rival.SpecieId));
            Position targetPos = team.Positions[i];
            rival.SetPosition(targetPos);
            rival.SetMatchPosition(targetPos);            
        }
    }

    //리그 레벨과 해당 팀에 알맞는 스탯 생성해서 반환
    private List<Stat> GetRivalStatsByLevel(string leagueLevelId, Team team)
    {
        List<Stat> newStats = new List<Stat>();

        League_LevelData levelData =  LeagueManager.Instance.GetLeagueLevelDataById(leagueLevelId).Value;
        Team_ArchetypeData archData = LeagueManager.Instance.GetArchetypeDataById(team.Team_ArchetypeData.Value.teamArchetypeId).Value;

        // 리그 레벨에 따른 기본 스탯 범위 (예: 10~20 사이에서 랜덤)
        int minBase = levelData.minPotential;
        int maxBase = levelData.maxPotential;

        foreach (potential type in System.Enum.GetValues(typeof(potential)))
        {
            if (type == potential.None) continue;

            // 1. 해당 리그 레벨의 기본 범위 내에서 난수 생성
            float baseRandom = UnityEngine.Random.Range(minBase, maxBase + 1);

            // 2. 팀 티어 가중치 가져오기
            float archWeight = LeagueManager.Instance.GetWeightByTier(levelData, team.Rival_MasterData.Value.teamTier);

            // 3. 팀의 잠재력 가중치 가져오기
            float potenWeight = LeagueManager.Instance.GetWeightByPotential(archData, type);

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
