using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SwissMatchRow : MonoBehaviour
{
    [SerializeField] private Image _bgImage; // 플레이어 팀 강조용 배경

    [Header("Home Team UI")]
    [SerializeField] private TextMeshProUGUI _txtRank;
    [SerializeField] private TextMeshProUGUI _txtTeamName; // 팀명
    [SerializeField] private TextMeshProUGUI _txtRecord;   // 누적 승패 
    [SerializeField] private TextMeshProUGUI _txtScore;  // 해당 라운드 점수


    public void Init(string teamId, int rank, int win, int lose, string scoreStr, bool isPlayerTeam)
    {
        // 플레이어 팀일 경우 배경색 강조
        if (_bgImage != null)
        {
            _bgImage.color = isPlayerTeam ? new Color(0.2f, 0.6f, 1f, 0.2f) : new Color(1f, 1f, 1f, 0f);
        }

        // 순위 표기
        if (_txtRank != null)
            _txtRank.text = rank > 0 ? $"{rank}위" : "-위";

        // 팀명 및 볼드 처리
        if (_txtTeamName != null)
            _txtTeamName.text = GetTeamName(teamId);

        // 플레이어 팀 폰트 강조
        if (isPlayerTeam)
        {
            _txtRank.fontStyle = FontStyles.Bold;
            _txtTeamName.fontStyle = FontStyles.Bold;
        }
        else
        {
            _txtRank.fontStyle = FontStyles.Normal;
            _txtTeamName.fontStyle = FontStyles.Normal;
        }

        // 누적 승패 및 점수 표기
        if (_txtRecord != null) _txtRecord.text = $"{win}승 {lose}패";
        if (_txtScore != null) _txtScore.text = scoreStr;
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
}

