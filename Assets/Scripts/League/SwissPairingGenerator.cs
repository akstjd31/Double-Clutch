using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 문서 기준 Swiss pairing 생성기
/// 규칙:
/// 1. 동일 승수끼리 그룹 생성
/// 2. 상위 그룹부터 처리
/// 3. 그룹 내 정렬
///    - 승수 내림차순
///    - 직전 라운드 득실차 내림차순
///    - teamTier 내림차순
///    - teamId 오름차순
/// 4. 인접 pairing (1-2, 3-4, 5-6)
/// 5. 리매치 pair가 있으면
///    - 리매치가 아닌 pair는 고정
///    - 리매치 pair에 속한 팀만 셔플 반복
///    - 최대 시도 후에도 실패하면 리매치 허용
/// 6. 홀수 그룹은 최하위 팀을 하위 그룹으로 float
/// 7. 마지막 그룹까지 홀수면 BYE 처리
/// </summary>
public class SwissPairingGenerator : ILeaguePairingGenerator
{
    private const string PLAYER_TEAM_ID = "Player_Team";
    private const int DEFAULT_MAX_SHUFFLE_ATTEMPTS = 10;

    private readonly int _maxShuffleAttempts;
    private readonly System.Random _random;

    public SwissPairingGenerator(int maxShuffleAttempts = DEFAULT_MAX_SHUFFLE_ATTEMPTS)
    {
        _maxShuffleAttempts = Math.Max(1, maxShuffleAttempts);
        _random = new System.Random();
    }

    public List<LeagueMatchRecord> GenerateRoundMatches(LeagueSaveData saveData, int roundIndex)
    {
        var result = new List<LeagueMatchRecord>();
        if (saveData == null || saveData.teams == null || saveData.teams.Count < 2)
            return result;

        // 1. 동일 승수 그룹 생성
        var groups = BuildWinGroups(saveData, roundIndex);

        // 상위 그룹부터 순차 처리
        string floatedTeamId = null;

        for (int i = 0; i < groups.Count; i++)
        {
            var currentGroup = new List<string>();

            if (!string.IsNullOrEmpty(floatedTeamId))
            {
                currentGroup.Add(floatedTeamId);
                floatedTeamId = null;
            }

            currentGroup.AddRange(groups[i]);

            // 2, 3. 그룹 정렬 + 그룹 내 정렬
            SortGroup(currentGroup, saveData, roundIndex);

            // 4. 홀수 그룹이면 플로팅 또는 BYE
            if (currentGroup.Count % 2 != 0)
            {
                if (i < groups.Count - 1)
                {
                    floatedTeamId = ExtractFloater(currentGroup);
                }
                else
                {
                    string byeTeamId = SelectByeTeam(currentGroup, saveData);
                    currentGroup.Remove(byeTeamId);
                    result.Add(CreateByeMatch(roundIndex, byeTeamId));
                }
            }

            if (currentGroup.Count > 0)
            {
                var groupMatches = BuildMatchesForGroup(currentGroup, saveData, roundIndex);
                result.AddRange(groupMatches);
            }
        }

        return result;
    }

    /// <summary>
    /// 동일 승수 기준 그룹 생성
    /// 첫 라운드는 standings가 없을 수 있으므로 전체를 하나의 그룹으로 처리
    /// </summary>
    private List<List<string>> BuildWinGroups(LeagueSaveData saveData, int roundIndex)
    {
        if (saveData.standings == null || saveData.standings.Count == 0 || roundIndex == 0)
        {
            var firstGroup = saveData.teams
                .Select(t => t.teamId)
                .ToList();

            SortGroup(firstGroup, saveData, roundIndex);
            return new List<List<string>> { firstGroup };
        }

        return saveData.standings
            .GroupBy(s => s.win)
            .OrderByDescending(g => g.Key)
            .Select(g =>
            {
                var teamIds = g.Select(x => x.teamId).ToList();
                SortGroup(teamIds, saveData, roundIndex);
                return teamIds;
            })
            .ToList();
    }

    /// <summary>
    /// 그룹 내 정렬
    /// 1순위: 승수
    /// 2순위: 직전 라운드 득실차
    /// 3순위: teamTier
    /// 4순위: teamId 오름차순
    /// </summary>
    private void SortGroup(List<string> group, LeagueSaveData saveData, int roundIndex)
    {
        group.Sort((a, b) =>
        {
            int winA = GetWin(saveData, a);
            int winB = GetWin(saveData, b);
            int compare = winB.CompareTo(winA);
            if (compare != 0) return compare;

            int lastRoundDiffA = GetLastRoundDiff(saveData, a, roundIndex);
            int lastRoundDiffB = GetLastRoundDiff(saveData, b, roundIndex);
            compare = lastRoundDiffB.CompareTo(lastRoundDiffA);
            if (compare != 0) return compare;

            int tierA = GetTeamTierValue(saveData, a);
            int tierB = GetTeamTierValue(saveData, b);
            compare = tierB.CompareTo(tierA);
            if (compare != 0) return compare;

            return string.Compare(a, b, StringComparison.Ordinal);
        });
    }

    /// <summary>
    /// 홀수 그룹일 때 하위 그룹으로 내릴 팀 선택
    /// 문서 기준으로 "그룹 내 정렬 완료 후" 최하위 팀을 float
    /// </summary>
    private string ExtractFloater(List<string> currentGroup)
    {
        if (currentGroup == null || currentGroup.Count == 0)
            return null;

        string floater = currentGroup[currentGroup.Count - 1];
        currentGroup.RemoveAt(currentGroup.Count - 1);
        return floater;
    }

    /// <summary>
    /// 마지막 그룹까지 홀수면 BYE 처리
    /// 가능하면 기존에 BYE를 받지 않은 팀 중 최하위 팀 선택
    /// </summary>
    private string SelectByeTeam(List<string> currentGroup, LeagueSaveData saveData)
    {
        for (int i = currentGroup.Count - 1; i >= 0; i--)
        {
            if (!HasBye(saveData, currentGroup[i]))
                return currentGroup[i];
        }

        return currentGroup[currentGroup.Count - 1];
    }

    /// <summary>
    /// 그룹 내 인접 페어링 후 리매치 처리
    /// - 리매치가 아닌 pair는 고정
    /// - 리매치 pair에 속한 팀만 셔플 반복
    /// - 최대 시도 후 실패 시 리매치 허용
    /// </summary>
    private List<LeagueMatchRecord> BuildMatchesForGroup(List<string> orderedGroup, LeagueSaveData saveData, int roundIndex)
    {
        var lockedPairs = new List<PairingPair>();
        var unresolvedTeamIds = new List<string>(orderedGroup);

        int attempt = 0;

        while (unresolvedTeamIds.Count > 0)
        {
            var sequentialPairs = BuildSequentialPairs(unresolvedTeamIds);

            var stillUnresolved = new List<string>();
            bool anyNewLocked = false;

            foreach (var pair in sequentialPairs)
            {
                if (HasPlayed(saveData, pair.TeamA, pair.TeamB))
                {
                    stillUnresolved.Add(pair.TeamA);
                    stillUnresolved.Add(pair.TeamB);
                }
                else
                {
                    lockedPairs.Add(pair);
                    anyNewLocked = true;
                }
            }

            if (stillUnresolved.Count == 0)
                break;

            // 더 이상 해결된 pair가 안 생기면 셔플 시도 카운트 증가
            if (!anyNewLocked)
            {
                attempt++;
                if (attempt > _maxShuffleAttempts)
                {
                    // fallback: 남은 pair는 리매치 허용
                    var fallbackPairs = BuildSequentialPairs(stillUnresolved);
                    lockedPairs.AddRange(fallbackPairs);
                    break;
                }
            }

            Shuffle(stillUnresolved);
            unresolvedTeamIds = stillUnresolved;
        }

        // 원래 그룹 순서를 최대한 유지하도록 정렬
        lockedPairs = lockedPairs
            .OrderBy(p => orderedGroup.IndexOf(p.TeamA))
            .ToList();

        return lockedPairs
            .Select(p => CreateNormalMatch(roundIndex, p.TeamA, p.TeamB))
            .ToList();
    }

    /// <summary>
    /// 그룹 내 인접 순서대로 1-2, 3-4, 5-6 페어 생성
    /// </summary>
    private List<PairingPair> BuildSequentialPairs(List<string> teamIds)
    {
        var result = new List<PairingPair>();

        for (int i = 0; i < teamIds.Count; i += 2)
        {
            if (i + 1 >= teamIds.Count)
                break;

            result.Add(new PairingPair(teamIds[i], teamIds[i + 1]));
        }

        return result;
    }

    /// <summary>
    /// unresolved 팀들만 셔플
    /// </summary>
    private void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private bool HasPlayed(LeagueSaveData saveData, string teamA, string teamB)
    {
        if (saveData.matchRecords == null) return false;

        foreach (var match in saveData.matchRecords)
        {
            bool sameMatch =
                (match.homeTeamId == teamA && match.awayTeamId == teamB) ||
                (match.homeTeamId == teamB && match.awayTeamId == teamA);

            if (sameMatch)
                return true;
        }

        return false;
    }

    private bool HasBye(LeagueSaveData saveData, string teamId)
    {
        if (saveData.matchRecords == null) return false;

        foreach (var match in saveData.matchRecords)
        {
            if (match.specialNote != "BYE") continue;
            if (match.homeTeamId == teamId)
                return true;
        }

        return false;
    }

    private int GetWin(LeagueSaveData saveData, string teamId)
    {
        var standing = saveData.standings?.Find(s => s.teamId == teamId);
        return standing != null ? standing.win : 0;
    }

    /// <summary>
    /// 직전 라운드 득실차
    /// BYE는 득실차 0으로 처리
    /// </summary>
    private int GetLastRoundDiff(LeagueSaveData saveData, string teamId, int roundIndex)
    {
        int targetRound = roundIndex - 1;
        if (targetRound < 0 || saveData.matchRecords == null)
            return 0;

        foreach (var match in saveData.matchRecords)
        {
            if (match.roundIndex != targetRound) continue;
            if (!match.isPlayed) continue;

            if (match.specialNote == "BYE")
            {
                if (match.homeTeamId == teamId)
                    return 0;
                continue;
            }

            if (match.homeTeamId == teamId)
                return match.homeScore - match.awayScore;

            if (match.awayTeamId == teamId)
                return match.awayScore - match.homeScore;
        }

        return 0;
    }

    /// <summary>
    /// teamTier 값 조회
    /// 플레이어 팀은 League_Level_Table.playerTeamTier 사용
    /// </summary>
    private int GetTeamTierValue(LeagueSaveData saveData, string teamId)
    {
        if (teamId == PLAYER_TEAM_ID)
        {
            var masterData = LeagueDataManager.Instance.GetMasterDataById(saveData.leagueId);
            if (masterData == null)
                return int.MinValue;

            string leagueLevelId = masterData.Value.leagueLevelId;

            var levelData = LeagueDataManager.Instance.GetLeagueLevelDataById(leagueLevelId);
            if (levelData == null)
                return int.MinValue;

            return (int)levelData.Value.playerTeamTier;
        }

        var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(teamId);
        if (rivalData == null)
            return int.MinValue;

        return (int)rivalData.Value.teamTier;
    }

    private LeagueMatchRecord CreateNormalMatch(int roundIndex, string homeTeamId, string awayTeamId)
    {
        return new LeagueMatchRecord
        {
            roundIndex = roundIndex,
            homeTeamId = homeTeamId,
            awayTeamId = awayTeamId,
            isPlayed = false,
            homeScore = 0,
            awayScore = 0,
            specialNote = string.Empty,
            replayLogKey = string.Empty
        };
    }

    private LeagueMatchRecord CreateByeMatch(int roundIndex, string teamId)
    {
        return new LeagueMatchRecord
        {
            roundIndex = roundIndex,
            homeTeamId = teamId,
            awayTeamId = string.Empty,
            isPlayed = true,
            homeScore = 1,
            awayScore = 0,
            specialNote = "BYE",
            replayLogKey = string.Empty
        };
    }

    private class PairingPair
    {
        public string TeamA;
        public string TeamB;

        public PairingPair(string teamA, string teamB)
        {
            TeamA = teamA;
            TeamB = teamB;
        }
    }
}