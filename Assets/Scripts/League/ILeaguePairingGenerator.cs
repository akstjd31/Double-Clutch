using System.Collections.Generic;

/// <summary>
/// 스위스 or 토너먼트
/// </summary>
public interface ILeaguePairingGenerator
{
    List<LeagueMatchRecord> GenerateRoundMatches(LeagueSaveData saveData, int roundIndex);
}