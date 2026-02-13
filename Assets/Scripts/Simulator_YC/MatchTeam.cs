using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchTeam
{
    private TeamSide _side;
    private string _teamName;
    private string _teamColorId; // 전술 ID 
    private List<MatchPlayer> _roster; // 현재 출전 중인 5명
    private int _score;

    public TeamSide Side => _side;
    public string TeamName => _teamName;
    public string TeamColorId => _teamColorId;
    public int Score => _score;
    public List<MatchPlayer> Roster => _roster;
    public MatchTeam(TeamSide side, string name, string tacticId)
    {
        _side = side;
        _teamName = name;
        _teamColorId = tacticId;
        _roster = new List<MatchPlayer>();
        _score = 0;
    }

    public void AddPlayer(MatchPlayer player)
    {
        if (_roster.Count < 5)
        {
            _roster.Add(player);
        }
        else
        {
            Debug.LogWarning($"[MatchTeam] Roster is full for {_teamName}");
        }
    }

    public void AddScore(int points)
    {
        _score += points;
    }

    /// <summary>
    /// 특정 포지션의 선수를 찾습니다 (없을 수도 있음)
    /// </summary>
    public MatchPlayer GetPlayerByPosition(Position pos)
    {
        return _roster.Find(p => p.MainPosition == pos);
    }
}
