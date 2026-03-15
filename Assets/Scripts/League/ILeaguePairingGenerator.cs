using System.Collections.Generic;

public interface ILeaguePairingGenerator
{
    List<LeagueMatchRecord> GenerateRoundMatches(LeagueSaveData saveData, int roundIndex);
}