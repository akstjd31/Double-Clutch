using System.Collections.Generic;
using UnityEngine;

public class EnemyTeamFactory : MonoBehaviour
{
    public static EnemyTeamFactory Instance;

    [Header("Data Readers")]
    [SerializeField] private Rival_MasterDataReader _rivalReader;
    [SerializeField] private Team_ArchetypeDataReader _archetypeReader;
    [SerializeField] private League_LevelDataReader _leagueLevelReader;
    [SerializeField] private Player_SpeciesDataReader _speciesReader;
    [SerializeField] private Player_NameDataReader _nameReader;

    private void Awake()
    {
        Instance = this;
    }

    // 라이벌 팀 ID와 리그 레벨 ID를 받아서 완성된 적 팀을 반환
    public MatchTeam CreateEnemyTeam(string rivalTeamId, string leagueLevelId)
    {
        //  각 테이블에서 일치하는 데이터 찾기
        var rivalData = _rivalReader.DataList.Find(x => x.teamId == rivalTeamId);
        var archetypeData = _archetypeReader.DataList.Find(x => x.teamArchetypeId == rivalData.teamArchetypeId);
        var levelData = _leagueLevelReader.DataList.Find(x => x.leagueLevelId == leagueLevelId);

        if (string.IsNullOrEmpty(rivalData.teamId) || string.IsNullOrEmpty(archetypeData.teamArchetypeId))
        {
            Debug.LogError($"적군 생성 실패! 데이터를 찾을 수 없습니다. (Rival:{rivalTeamId} / Level:{leagueLevelId})");
            return null;
        }

        //  팀 티어에 따른 잠재력(스탯) 배율 구하기
        float tierWeight = GetTierWeight(rivalData.teamTier, levelData);

        //  기본 스탯 범위 설정 (최소~최대치에 티어 가중치 곱하기)
        int minStat = Mathf.RoundToInt(levelData.minPotential * tierWeight);
        int maxStat = Mathf.RoundToInt(levelData.maxPotential * tierWeight);

        // 팀 이름 (StringManager를 거쳐서 번역된 이름을 가져옴)
        string teamName = StringManager.Instance != null ? StringManager.Instance.GetString(rivalData.teamName) : rivalData.teamName;

        // 아키타입 ID를 팀 전술(TeamColorId)로 넘겨줌
        MatchTeam enemyTeam = new MatchTeam(TeamSide.Away, teamName, archetypeData.teamArchetypeId);

        //  아키타입에 명시된 포지션 인원수만큼 5명 구성
        List<Position> teamPositions = new List<Position>();
        for (int i = 0; i < archetypeData.countPG; i++) teamPositions.Add(Position.PG);
        for (int i = 0; i < archetypeData.countSG; i++) teamPositions.Add(Position.SG);
        for (int i = 0; i < archetypeData.countSF; i++) teamPositions.Add(Position.SF);
        for (int i = 0; i < archetypeData.countPF; i++) teamPositions.Add(Position.PF);
        for (int i = 0; i < archetypeData.countC; i++) teamPositions.Add(Position.C);

        //  종족 배정 (기획서 4.2.2: 최소 인원 + 가중치 추첨)
        List<speciesType> teamSpecies = GenerateSpeciesList(rivalData);

        //  5명의 선수 생성
        for (int i = 0; i < teamPositions.Count; i++)
        {
            Position pos = teamPositions[i];
            speciesType currentSpecies = teamSpecies[i];

            // 이름 로컬라이징 (기획서 4.2.3)
            string playerName = GenerateRivalName(rivalData.teamsector);

            var stats = new Dictionary<MatchStatType, int>();
            stats[MatchStatType.TwoPoint] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1) * archetypeData.weight2pt);
            stats[MatchStatType.ThreePoint] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1) * archetypeData.weight3pt);
            stats[MatchStatType.Pass] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1) * archetypeData.weightPass);
            stats[MatchStatType.Block] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1) * archetypeData.weightBlock);
            stats[MatchStatType.Steal] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1) * archetypeData.weightSteal);
            stats[MatchStatType.Rebound] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1) * archetypeData.weightRebound);
            stats[MatchStatType.Dribble] = Mathf.RoundToInt(Random.Range(minStat, maxStat + 1));

            MatchPlayer player = new MatchPlayer(
                id: 20000 + i,
                name: playerName,
                pos: pos,
                initStats: stats,
                resourceKey: "Enemy_Resource",
                passives: new List<Player_PassiveData>()
            );

            enemyTeam.AddPlayer(player);
        }

        return enemyTeam;
    }

    private float GetTierWeight(teamTier tier, League_LevelData levelData)
    {
        switch (tier)
        {
            case teamTier.D: return levelData.weightPotentialTierD;
            case teamTier.C: return levelData.weightPotentialTierC;
            case teamTier.B: return levelData.weightPotentialTierB;
            case teamTier.A: return levelData.weightPotentialTierA;
            case teamTier.S: return levelData.weightPotentialTierS;
            case teamTier.SS: return levelData.weightPotentialTierSS;
            case teamTier.SSS: return levelData.weightPotentialTierSSS;
            default: return 1.0f;
        }
    }
    // 종족 추첨 로직
    private List<speciesType> GenerateSpeciesList(Rival_MasterData rivalData)
    {
        List<speciesType> result = new List<speciesType>();

        //  최소 인원만큼 무조건 배정 (Android로 선언하신 변수명 사용)
        for (int i = 0; i < rivalData.minAndroidCount; i++) result.Add(speciesType.Humanoid);
        for (int i = 0; i < rivalData.minHumanCount; i++) result.Add(speciesType.Human);
        for (int i = 0; i < rivalData.minAnimalCount; i++) result.Add(speciesType.Animal);

        //  남은 자리는 가중치에 따라 랜덤 배정
        int remain = 5 - result.Count;
        float totalWeight = rivalData.weightAndroid + rivalData.weightHuman + rivalData.weightAnimal;

        for (int i = 0; i < remain; i++)
        {
            if (totalWeight <= 0)
            {
                result.Add(speciesType.Human); // 가중치 합이 0일 경우의 안전 장치
                continue;
            }

            float rand = Random.Range(0f, totalWeight);
            if (rand < rivalData.weightAndroid)
                result.Add(speciesType.Humanoid);
            else if (rand < rivalData.weightAndroid + rivalData.weightHuman)
                result.Add(speciesType.Human);
            else
                result.Add(speciesType.Animal);
        }

        // 포지션별로 랜덤하게 들어갈 수 있도록 리스트 셔플
        for (int i = 0; i < result.Count; i++)
        {
            int rnd = Random.Range(0, result.Count);
            var temp = result[i];
            result[i] = result[rnd];
            result[rnd] = temp;
        }

        return result;
    }
    // 이름 생성 로직
    private string GenerateRivalName(teamSector sector)
    {
        // DOM(국내)이면 한국 이름, OS(해외)이거나 NA(안드로이드)면 외국 이름 사용
        nation targetNation = (sector == teamSector.DOM) ? nation.Korea : nation.America;

        List<string> firsts = new List<string>();
        List<string> middles = new List<string>();
        List<string> lasts = new List<string>();

        foreach (var data in _nameReader.DataList)
        {
            if (data.nation == targetNation)
            {
                // 스트링 매니저에서 텍스트를 못 찾을 경우 엑셀의 desc(원문)을 그대로 사용
                string nameStr = (StringManager.Instance != null) ? StringManager.Instance.GetString(data.nameKey) : data.desc;
                if (string.IsNullOrEmpty(nameStr) || nameStr == data.nameKey) nameStr = data.desc;

                if (data.namePart == namePart.FirstName) firsts.Add(nameStr);
                else if (data.namePart == namePart.MiddleName) middles.Add(nameStr);
                else if (data.namePart == namePart.LastName) lasts.Add(nameStr);
            }
        }

        string f = firsts.Count > 0 ? firsts[Random.Range(0, firsts.Count)] : "";
        string m = middles.Count > 0 ? middles[Random.Range(0, middles.Count)] : "";
        string l = lasts.Count > 0 ? lasts[Random.Range(0, lasts.Count)] : "";

        // 한국은 "성+가운데+끝", 미국은 "First Name + Last Name" 조합
        if (targetNation == nation.Korea)
            return $"{f}{m}{l}";
        else
            return $"{f} {l}";
    }

}