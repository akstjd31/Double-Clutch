using System;
using System.Collections.Generic;
using UnityEngine;

public class LeagueTeamSelector
{
    private readonly System.Random _random;

    public LeagueTeamSelector(int? seed = null)
    {
        _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
    }

    public List<string> SelectTeams(League_TeamData rule,
                                    List<Rival_MasterData> allTeams,
                                    List<string> priorityTeamIds = null,
                                    string playerTeamId = null)
    {
        List<string> result = new List<string>();

        // 1. 우선 포함 팀 먼저 처리하기
        AddPriorityTeams(rule, result, priorityTeamIds, playerTeamId);

        // 2. 후보 풀 필터링하기
        var candidatePool = BuildCandidatePool(rule, allTeams, result);

        // 3. 티어별 랜덤 추출
        bool success = PickTeamsByTier(rule, candidatePool, result);

        // 후보 풀 부족 (3.3.2)
        if (!success)
        {
            Debug.LogError($"후보 풀 부족으로 리그 팀 구성 실패! [{rule.desc}]");
            return null;
        }

        // 4. 검증
        ValidateResult(rule, result);

        return result;
    }

    // 우선 포함
    private void AddPriorityTeams(League_TeamData rule, List<string> result, List<string> priorityTeamIds, string playerTeamId)
    {
        if (priorityTeamIds == null) return;

        // 예외 처리 부분 (3.3 참고)
        if (rule.priorityTeamCount > 1)
        {
            // 이 과정에서 이미 플레이어는 상위 팀에 포함되어 있기 때문에 따로 플레이어 부분의 ID 추가 처리는 안함.
            for (int i = 0; i < rule.priorityTeamCount; i++)
            {
                var prevData = LeagueDataManager.Instance.LoadLeague(rule.prioritySourceLeagueId);
                if (prevData == null)
                {
                    Debug.LogError("이전 리그의 데이터가 없습니다!");
                    return;
                }

                // 이전 리그 상위 팀 추가 (정렬되어있는 기준)
                result.Add(prevData.standings[i].teamId);
            }
        }
        else
        {
            // priorityTeamCount만큼 우선 포함
            for (int i = 0; i < priorityTeamIds.Count; i++)
            {
                string teamId = priorityTeamIds[i];
                if (string.IsNullOrEmpty(teamId)) continue;
                if (result.Contains(teamId)) continue;

                result.Add(teamId);
            }

            // 플레이어 팀은 우선 포함 처리 (3.2 참고)
            if (!string.IsNullOrEmpty(playerTeamId) && !result.Contains(playerTeamId))
            {
                result.Add(playerTeamId);
            }

            if (rule.priorityTeamCount > 0 && result.Count < rule.priorityTeamCount)
            {
                Debug.LogWarning($"우선 포함 팀 수 부족! 필요: {rule.priorityTeamCount}, 현재: {result.Count}");
            }
        }
    }

    // 소속 구역 필터
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

            // 비트마스크 필터 적용
            var sectorMask = Parse(rule.candidateSectorList);
            if (!IsMatchedSector(sectorMask, team.Value.teamsector))
                continue;

            pool.Add(team.Value);
        }

        return pool;
    }

    // 티어별 추출
    private bool PickTeamsByTier(League_TeamData? rule, List<Rival_MasterData> candidatePool, List<string> result)
    {
        if (rule == null)
        {
            Debug.LogError("League_TeamData(rule)가 null입니다.");
            return false;
        }

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.D, rule.Value.selectionCountD))
            return false;

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.C, rule.Value.selectionCountC))
            return false;

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.B, rule.Value.selectionCountB))
            return false;

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.A, rule.Value.selectionCountA))
            return false;

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.S, rule.Value.selectionCountS))
            return false;

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.SS, rule.Value.selectionCountSS))
            return false;

        if (!PickRandomTeamsByTier(candidatePool, result, teamTier.SSS, rule.Value.selectionCountSSS))
            return false;

        return true;
    }

    // 특정 티어에서 count 만큼 랜덤으로 뽑기
    private bool PickRandomTeamsByTier(
    List<Rival_MasterData> candidatePool,
    List<string> result,
    teamTier targetTier,
    int count)
    {
        if (count <= 0) return true;
        if (candidatePool == null || result == null)
        {
            Debug.LogError("candidatePool 또는 result가 null입니다.");
            return false;
        }

        int remain = count;
        teamTier currentTier = targetTier;

        while (remain > 0)
        {
            var tierPool = new List<Rival_MasterData>();

            for (int i = 0; i < candidatePool.Count; i++)
            {
                if (candidatePool[i].teamTier.Equals(currentTier))
                    tierPool.Add(candidatePool[i]);
            }

            // 중복 방지용 셔플
            Shuffle(tierPool);

            int pickCount = Math.Min(remain, tierPool.Count);

            for (int i = 0; i < pickCount; i++)
            {
                var picked = tierPool[i];

                if (!result.Contains(picked.teamId))
                    result.Add(picked.teamId);

                candidatePool.Remove(picked);
            }

            remain -= pickCount;

            // 다 뽑았으면 성공
            if (remain <= 0)
                return true;

            // 부족하면 바로 아래 티어로 내려감
            if (!TryGetLowerTier(currentTier, out var lowerTier))
            {
                Debug.LogError(
                    $"후보 풀 부족으로 팀 구성 불가. 요청 티어: {targetTier}, 부족 수량: {remain}");
                return false;
            }

            Debug.LogWarning(
                $"{currentTier} 티어 팀이 부족하여 {lowerTier} 티어에서 {remain}팀 보충합니다.");

            currentTier = lowerTier;
        }

        return true;
    }

    // 아래 티어 찾기
    private bool TryGetLowerTier(teamTier currentTier, out teamTier lowerTier)
    {
        lowerTier = teamTier.None;

        switch (currentTier)
        {
            case teamTier.SSS:
                lowerTier = teamTier.SS;
                return true;
            case teamTier.SS:
                lowerTier = teamTier.S;
                return true;
            case teamTier.S:
                lowerTier = teamTier.A;
                return true;
            case teamTier.A:
                lowerTier = teamTier.B;
                return true;
            case teamTier.B:
                lowerTier = teamTier.C;
                return true;
            case teamTier.C:
                lowerTier = teamTier.D;
                return true;
            default:
                return false; // D 아래는 없음
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