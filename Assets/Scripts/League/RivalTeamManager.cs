using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 라이벌 팀 일괄 생성 및 관리
/// 리그 돌입 전 라이벌 스탯 일괄 재부여.
/// </summary>

public class RivalTeamManager : Singleton<RivalTeamManager>
{
    [SerializeField] LeagueFactory _leagueFactory;
    [SerializeField] StudentFactory _studentFactory;

    List<string> RivalTeamList = new List<string>();
    

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
    public List<Stat> GetRivalStatsByLevel(string leagueLevelId, Team team)
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
            int finalValue = (int)(baseRandom * archWeight * potenWeight);

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

            float rand = Random.Range(0f, totalWeight);
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
            int rnd = Random.Range(0, result.Count);
            var temp = result[i];
            result[i] = result[rnd];
            result[rnd] = temp;
        }

        return result;
    }

    
}
