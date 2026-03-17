using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SwissMatchRow : MonoBehaviour
{
    [SerializeField] private Image _bgImage; // 플레이어 팀 강조용 배경

    [Header("Home Team UI (Left)")]
    [SerializeField] private TextMeshProUGUI _txtHomeName;
    [SerializeField] private TextMeshProUGUI _txtHomeRecord; // 누적 승패
    [SerializeField] private TextMeshProUGUI _txtHomeScore;  // 해당 라운드 점수

    [Header("Away Team UI (Right)")]
    [SerializeField] private TextMeshProUGUI _txtAwayName;
    [SerializeField] private TextMeshProUGUI _txtAwayRecord;
    [SerializeField] private TextMeshProUGUI _txtAwayScore;

    public void Init(LeagueMatchRecord match, int viewRoundIndex)
    {
        string myTeamId = StudentManager.TEAM_ID;
        bool hasPlayer = (match.homeTeamId == myTeamId || match.awayTeamId == myTeamId);

        // 플레이어 팀이 포함된 매치라면 배경색 하이라이트
        if (_bgImage != null)
        {
            _bgImage.color = hasPlayer ? new Color(0.2f, 0.6f, 1f, 0.2f) : new Color(1f, 1f, 1f, 0f);
        }

        // 팀명 세팅
        _txtHomeName.text = GetTeamName(match.homeTeamId);
        _txtAwayName.text = GetTeamName(match.awayTeamId);

        if (hasPlayer)
        {
            if (match.homeTeamId == myTeamId) _txtHomeName.fontStyle = FontStyles.Bold;
            if (match.awayTeamId == myTeamId) _txtAwayName.fontStyle = FontStyles.Bold;
        }
        else
        {
            _txtHomeName.fontStyle = FontStyles.Normal;
            _txtAwayName.fontStyle = FontStyles.Normal;
        }

        // 직전 라운드(viewRoundIndex - 1)까지의 누적 승패 역산
        var homeRecord = GetCumulativeRecord(match.homeTeamId, viewRoundIndex);
        var awayRecord = GetCumulativeRecord(match.awayTeamId, viewRoundIndex);

        _txtHomeRecord.text = $"{homeRecord.win}승 {homeRecord.lose}패";
        _txtAwayRecord.text = $"{awayRecord.win}승 {awayRecord.lose}패";

        // 해당 탭(라운드)의 경기 득점 표시. 미진행시 "-"
        if (match.isPlayed)
        {
            _txtHomeScore.text = match.homeScore.ToString();
            _txtAwayScore.text = match.awayScore.ToString();
        }
        else
        {
            _txtHomeScore.text = "-";
            _txtAwayScore.text = "-";
        }
    }

    private string GetTeamName(string teamId)
    {
        if (teamId == StudentManager.TEAM_ID)
            return GameManager.Instance.SaveData.schoolName;

        var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(teamId);
        if (rivalData != null)
            return StringManager.Instance.GetString(rivalData.Value.teamNameKey);

        return teamId;
    }

    // 지정된 라운드 이전까지의 승패를 역산하여 반환
    private (int win, int lose) GetCumulativeRecord(string teamId, int upToRoundIndex)
    {
        int w = 0, l = 0;
        var league = LeagueManager.Instance.CurrentLeague;

        for (int i = 0; i < upToRoundIndex; i++)
        {
            var match = league.matchRecords.Find(m => m.roundIndex == i && (m.homeTeamId == teamId || m.awayTeamId == teamId));
            if (match != null && match.isPlayed)
            {
                int myScore = match.homeTeamId == teamId ? match.homeScore : match.awayScore;
                int opScore = match.homeTeamId == teamId ? match.awayScore : match.homeScore;

                if (myScore > opScore) w++;
                else if (myScore < opScore) l++;
            }
        }
        return (w, l);
    }
}
