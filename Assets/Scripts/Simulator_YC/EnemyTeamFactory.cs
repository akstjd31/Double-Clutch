using System.Collections.Generic;
using UnityEngine;

public class EnemyTeamFactory : MonoBehaviour
{
    public static EnemyTeamFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    public MatchTeam ConvertToTeam(TeamSide side, Team team)
    {
        if (team == null || team.Members == null || team.Members[0] == null)
        {
            Debug.LogError($"[EnemyTeamFactory] {side} 팀 데이터가 Null입니다! 에러 방지용 임시 팀을 생성합니다.");
            team = new Team(side == TeamSide.Home ? PrefKeys.PLAYER_TEAM_ID : "Dummy_Team", side == TeamSide.Home);

            for (int i = 0; i < 5; i++)
            {
                Student dummy = new Student();
                dummy.SetName($"더미{i + 1}", "", "");
                dummy.SetPosition((Position)(i + 1));
                dummy.SetMatchPosition((Position)(i + 1));

                List<Stat> dStats = new List<Stat> {
                    new Stat(potential.Stat2pt, 10, 99, 1),
                    new Stat(potential.Stat3pt, 10, 99, 1),
                    new Stat(potential.StatPass, 10, 99, 1),
                    new Stat(potential.StatBlock, 10, 99, 1),
                    new Stat(potential.StatSteal, 10, 99, 1),
                    new Stat(potential.StatRebound, 10, 99, 1)
                };
                dummy.SetStat(dStats);
                dummy.OnStatChanged();
                team.SetMember(i, dummy);
            }
        }
        string archetypeId = team.Team_ArchetypeData.HasValue ? team.Team_ArchetypeData.Value.teamArchetypeId : string.Empty;

        // 팀 이름이 혹시 비어있으면 임시 이름 부여
        string teamName = string.IsNullOrEmpty(team.TeamNameKey) ?
                          (side == TeamSide.Home ? GameManager.Instance.SaveData.schoolName : "더미 봇 팀") :
                          team.TeamNameKey;

        MatchTeam matchTeam = new MatchTeam(side, teamName, archetypeId);

        // 용병이나 적군을 위한 임시 ID 시작 번호
        int startId = side == TeamSide.Home ? 10000 : 20000;
        Student[] members = team.Members;

        for (int i = 0; i < members.Length; i++)
        {
            if (members[i] == null) continue;

            Position finalPos = members[i].MatchPosition != Position.None ? members[i].MatchPosition : members[i].Position;

            // 정식 학생(ID 0 이상)이면 고유 ID 사용, 용병(ID -1)이나 적군이면 임시 ID 부여
            int matchPlayerId = (members[i].StudentId >= 0) ? members[i].StudentId : (startId + i);

            matchTeam.AddPlayer(ConvertStudentToMatchPlayer(members[i], matchPlayerId, finalPos));
        }

        return matchTeam;
    }

    public MatchPlayer ConvertStudentToMatchPlayer(Student s, int id, Position pos)
    {
        Dictionary<MatchStatType, int> stats = new Dictionary<MatchStatType, int>();

        stats.Add(MatchStatType.TwoPoint, s.GetCurrentStat(potential.Stat2pt));
        stats.Add(MatchStatType.ThreePoint, s.GetCurrentStat(potential.Stat3pt));
        stats.Add(MatchStatType.Pass, s.GetCurrentStat(potential.StatPass));
        stats.Add(MatchStatType.Steal, s.GetCurrentStat(potential.StatSteal));
        stats.Add(MatchStatType.Block, s.GetCurrentStat(potential.StatBlock));
        stats.Add(MatchStatType.Rebound, s.GetCurrentStat(potential.StatRebound));

        string actualVisualKey = s.VisualData.portraitResource;
        if (string.IsNullOrEmpty(actualVisualKey)) actualVisualKey = "Default_Player_Sprite";
        MatchPlayer matchPlayer = new MatchPlayer(id, s.Name, pos, stats, actualVisualKey, s.Passive);
        matchPlayer.TraitId = s.TraitId;
        matchPlayer.CutIn1 = s.VisualData.playerCutInResourceId01;
        matchPlayer.CutIn2 = s.VisualData.playerCutInResourceId02;
        matchPlayer.CutIn3 = s.VisualData.playerCutInResourceId03;
        return matchPlayer;
    }
}
