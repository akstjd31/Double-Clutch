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

    public int SimulatedScore;

    public int ReboundCount { get; set; }
    public int Try2pt { get; set; }
    public int Succ2pt { get; set; }
    public int Try3pt { get; set; }
    public int Succ3pt { get; set; }

    public TeamSide Side => _side;
    public string TeamName => _teamName;
    public string TeamColorId => _teamColorId;
    public int Score => _score;
    public List<MatchPlayer> Roster => _roster;
    public List<PlayerSynergyData> ActiveSynergies { get; private set; } = new List<PlayerSynergyData>();
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
    public void EvaluateSynergies(List<PlayerSynergyData> synergyDb)
    {
        ActiveSynergies.Clear();

        // 현재 출전 중인 5명의 특성 개수를 카운트
        Dictionary<string, int> traitCounts = new Dictionary<string, int>();
        foreach (var player in _roster)
        {
            if (!string.IsNullOrEmpty(player.TraitId))
            {
                if (!traitCounts.ContainsKey(player.TraitId))
                    traitCounts[player.TraitId] = 0;
                traitCounts[player.TraitId]++;
            }
        }

        // 시너지 DB를 순회하며 발동 조건이 맞는지 체크
        foreach (var syn in synergyDb)
        {
            bool isActivated = false;

            // 같은 특성 2개를 요구하는 시너지인 경우
            if (syn.traitId1 == syn.traitId2)
            {
                if (traitCounts.ContainsKey(syn.traitId1) && traitCounts[syn.traitId1] >= 2)
                    isActivated = true;
            }
            // 서로 다른 특성 2개를 요구하는 시너지인 경우 (ex: 스피드 + 테크닉)
            else
            {
                if (traitCounts.ContainsKey(syn.traitId1) && traitCounts.ContainsKey(syn.traitId2))
                    isActivated = true;
            }

            if (isActivated)
            {
                ActiveSynergies.Add(syn);
                Debug.Log($"[시너지 발동] {_teamName} 팀에 '{StringManager.Instance.GetString(syn.synergyName)}' 시너지 발동! (효과: {syn.effectType} {syn.effectValue})");
            }
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
