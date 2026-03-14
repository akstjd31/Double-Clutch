using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeagueTeamSelector : MonoBehaviour
{
    private LeagueDataManager _leagueMgr;
    private readonly System.Random _random;

    private void Awake()
    {
        _leagueMgr = this.GetComponent<LeagueDataManager>();
    }

    public LeagueTeamSelector(int? seed = null)
    {
        _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
    }

    public List<string> SelectTeams(League_TeamData rule, List<Rival_MasterData> allTeams, List<string> priorityTeamIds, string playerTeamId = null)
    {
        List<string> result = new List<string>();

        // 1. 우선 포함 팀 먼저 처리하기
        AddPriorityTeams(rule, result, priorityTeamIds, playerTeamId);

        // 2. 후보 풀 필터링하기
        var candidatePool = BuildCandidatePool(rule, allTeams, result);

        // 3. 티어별 랜덤 추출
        PickTeamsByTier(rule, candidatePool, result);

        // 4. 검증
        ValidateResult(rule, result);

        return result;
    }

    // 우선 포함 팀 추가
    private void AddPriorityTeams(League_TeamData rule, List<string> result, List<string> priorityTeamIds, string playerTeamId)
    {
        if (priorityTeamIds == null) return;
        
        // 예외 처리 (공백, 중복 팀 확인)
        for (int i = 0; i < priorityTeamIds.Count; i++)
        {
            string teamId = priorityTeamIds[i];
            if (string.IsNullOrEmpty(teamId)) continue;
            if (result.Contains(teamId)) continue;

            result.Add(teamId);
        }

        // 플레이어 팀은 무조건 포함해야됨
        if (!string.IsNullOrEmpty(playerTeamId) && !result.Contains(playerTeamId))
        {
            result.Add(playerTeamId);
        }

        if (rule.priorityTeamCount > 0 && result.Count < rule.priorityTeamCount)
        {
            Debug.LogWarning($"우선 포함 팀 수 부족! 필요: {rule.priorityTeamCount}, 현재: {result.Count}");
        }
    }

    // 후보 풀 구성
    private List<Rival_MasterData> BuildCandidatePool(League_TeamData rule, List<Rival_MasterData> allTeams, List<string> alreadySelected)
    {
        List<Rival_MasterData> pool = new List<Rival_MasterData>();

        if (allTeams == null) return null;

        for (int i = 0; i < allTeams.Count; i++)
        {
            Rival_MasterData? team = allTeams[i];
            if (team == null) continue;
            if (string.IsNullOrEmpty(team.Value.teamId)) continue;

            // 우선으로 포함된 팀은 제외
            if (alreadySelected.Contains(team.Value.teamId))
                continue;

            // 마스크 필터 적용
            var sectorMask = Parse(rule.candidateSectorList);
            if (!IsMatchedSector(sectorMask, team.Value.teamsector))
                continue;

            pool.Add(team.Value);
        }

        return pool;
    }

    // 티어별 추출
    private void PickTeamsByTier(League_TeamData rule, List<Rival_MasterData> candidatePool, List<string> result)
    {
        PickRandomTeamsByTier(candidatePool, result, teamTier.D, rule.selectionCountD);
        PickRandomTeamsByTier(candidatePool, result, teamTier.C, rule.selectionCountC);
        PickRandomTeamsByTier(candidatePool, result, teamTier.B, rule.selectionCountB);
        PickRandomTeamsByTier(candidatePool, result, teamTier.A, rule.selectionCountA);
        PickRandomTeamsByTier(candidatePool, result, teamTier.S, rule.selectionCountS);
        PickRandomTeamsByTier(candidatePool, result, teamTier.SS, rule.selectionCountSS);
        PickRandomTeamsByTier(candidatePool, result, teamTier.SSS, rule.selectionCountSSS);
    }

    // 특정 티어에서 count 만큼 랜덤으로 뽑기 (+ 중복 방지)
    private void PickRandomTeamsByTier(List<Rival_MasterData> candidatePool, List<string> result, teamTier targetTier, int count)
    {
        if (count <= 0) return;
        if (candidatePool == null || candidatePool.Count == 0) return;

        var tierPool = new List<Rival_MasterData>();

        for (int i = 0; i < candidatePool.Count; i++)
        {
            if (candidatePool[i].teamTier.Equals(targetTier))
                tierPool.Add(candidatePool[i]);
        }

        Shuffle(tierPool);

        int pickCount = Math.Min(count, tierPool.Count);

        for (int i = 0; i < pickCount; i++)
        {
            var picked = tierPool[i];

            if (!result.Contains(picked.teamId))
                result.Add(picked.teamId);
            
            candidatePool.Remove(picked);
        }

        if (pickCount < count)
        {
            Debug.LogWarning($"{targetTier} 티어 팀이 부족합니다.");
        }
    }

    // 최종 결과 검증
    private void ValidateResult(League_TeamData rule, List<string> result)
    {
        if (result == null) return;

        HashSet<string> hash = new HashSet<string>();
        for (int i = 0; i < result.Count; i++)
        {
            if (!hash.Add(result[i]))
            {
                Debug.LogError($"중복 팀 있음! {result[i]}");
            }
        }

        if (result.Count != rule.leagueTeamTotal)
        {
            Debug.LogError($"최종 팀 수가 일치하지 않습니다! 결과: {result.Count}, 요구: {rule.leagueTeamTotal}");
        }

        int pickedByTierTotal = 
            rule.selectionCountD +
            rule.selectionCountC +
            rule.selectionCountB +
            rule.selectionCountA + 
            rule.selectionCountS +
            rule.selectionCountSS +
            rule.selectionCountSSS;

        // 뽑은 거랑 갯수가 일치 하지 않는다면
        if (pickedByTierTotal != rule.selectionCountTotal)
        {
            Debug.LogWarning($"갯수가 일치하지 않습니다! 뽑은 개수: {pickedByTierTotal} 테이블상 개수: {rule.selectionCountTotal}");
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        if (list == null || list.Count <= 1) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // string으로 된 (| or 떄문에) 섹터 리스트를 팀 섹터로 변환하는 자업
    private teamSector Parse(string sectorList)
    {
        if (string.IsNullOrEmpty(sectorList))
            return teamSector.None;

        var result = teamSector.None;

        string[] sectors = sectorList.Split('|');

        foreach (string sector in sectors)
        {
            if (Enum.TryParse(sector.Trim(), true, out teamSector parsed))
            {
                result |= parsed;
            }
            else
            {
                Debug.LogWarning($"파싱 불가! {sector}");
            }
        }

        return result;
    }
    
    // 섹터 마스크 비교
    private bool IsMatchedSector(teamSector ruleMask, teamSector teamMask) => (ruleMask & teamMask) != 0;
}