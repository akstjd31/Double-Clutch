using System.Collections.Generic;

/// <summary>
/// 리그 런타임 클래스 (진행 정도를 알 수 있음)
/// </summary>
public class LeagueRuntime
{
    public LeagueSaveData SaveData { get; private set; }

    public LeagueRuntime (LeagueSaveData saveData)
    {
        SaveData = saveData;
    }

    public bool isFinished => SaveData.isFinished;
    public int CurrentRound => SaveData.currentRoundIndex;
}