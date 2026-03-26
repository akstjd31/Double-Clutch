using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TotalRankRow : MonoBehaviour
{
    [SerializeField] private Image _bgImage; // 내 팀 하이라이트용 배경 (선택 사항)
    [Header("Rank UI")]
    [SerializeField] private TextMeshProUGUI _textRank;      // 순위 (예: 1위)
    [SerializeField] private TextMeshProUGUI _textTeamName;  // 팀명
    [SerializeField] private TextMeshProUGUI _textRecord;    // 누적 승패 (예: 10승 2패)
    [SerializeField] private TextMeshProUGUI _textGoalDiff;  // 득실차 (예: 1234)

    // 데이터 주입 함수
    public void Init(LeagueStandingData data, bool isMyTeam)
    {
        if (_textRank != null) 
        {
            _textRank.text = data.rank+StringManager.Instance.GetString("UI_Simulator_위");
            StringManager.Instance.ApplyFont(_textRank);
        }

        if (_textRecord != null) 
        {
            _textRecord.text = StringManager.Instance.GetFormattedString("UI_Matchlog_승패", data.win, data.lose);
            StringManager.Instance.ApplyFont( _textRecord);
        } 

        if (_textGoalDiff != null) _textGoalDiff.text = data.goalDiff.ToString();

        // 내 팀인지 판별하여 이름과 배경색, 폰트 굵기 처리
        if (isMyTeam)
        {
            if (_textTeamName != null)
            {
                _textTeamName.text = GameManager.Instance.SaveData.schoolName;
                _textTeamName.fontStyle = FontStyles.Bold;
            }
            if (_textRank != null) _textRank.fontStyle = FontStyles.Bold;

            // 내 팀이면 배경색을 약간 푸른빛으로 강조
            if (_bgImage != null) _bgImage.color = new Color(0.2f, 0.6f, 1f, 0.5f);
        }
        else
        {
            // NPC 팀이면 라이벌 마스터 데이터에서 번역된 이름 가져오기
            if (_textTeamName != null)
            {
                var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(data.teamId);
                if (rivalData != null)
                {
                    _textTeamName.text = StringManager.Instance.GetString(rivalData.Value.teamNameKey);
                    StringManager.Instance.ApplyFont(_textTeamName);
                }
                else
                {
                    _textTeamName.text = data.teamId; // 데이터를 못 찾았을 경우 대비
                }
                _textTeamName.fontStyle = FontStyles.Normal;
            }

            if (_textRank != null) _textRank.fontStyle = FontStyles.Normal;

            // 일반 팀 배경색 투명하게
            if (_bgImage != null) _bgImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
