using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TotalRankRow : MonoBehaviour
{
    [SerializeField] private Image _bgImage; // 내 팀 하이라이트용 배경 (선택 사항)
    [SerializeField] private TextMeshProUGUI _textRank;      // 순위
    [SerializeField] private TextMeshProUGUI _textTeamName;  // 팀명
    [SerializeField] private TextMeshProUGUI _textPlayed;    // 경기 수
    [SerializeField] private TextMeshProUGUI _textWin;       // 승리
    [SerializeField] private TextMeshProUGUI _textLose;      // 패배
    [SerializeField] private TextMeshProUGUI _textPoints;    // 승점
    [SerializeField] private TextMeshProUGUI _textGoalDiff;  // 득실차

    // 데이터 주입 함수
    public void Init(LeagueStandingData data, bool isMyTeam)
    {
        _textRank.text = data.rank.ToString();
        _textPlayed.text = data.played.ToString();
        _textWin.text = data.win.ToString();
        _textLose.text = data.lose.ToString();
        _textPoints.text = data.points.ToString();
        _textGoalDiff.text = data.goalDiff.ToString();

        // 내 팀인지 판별하여 이름과 배경색 처리
        if (isMyTeam)
        {
            // GameManager에서 유저가 지은 학교 이름 가져오기
            _textTeamName.text = GameManager.Instance.SaveData.schoolName;

            // 내 팀이면 배경색을 약간 푸른빛이나 노란빛으로 강조
            if (_bgImage != null) _bgImage.color = new Color(0.2f, 0.6f, 1f, 0.5f);
        }
        else
        {
            //  NPC 팀이면 라이벌 마스터 데이터에서 번역된 이름 가져오기
            var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(data.teamId);
            if (rivalData != null)
            {
                _textTeamName.text = StringManager.Instance.GetString(rivalData.Value.teamNameKey);
            }
            else
            {
                _textTeamName.text = data.teamId; // 데이터를 못 찾았을 경우 대비
            }

            // 일반 팀 배경색 투명하게
            if (_bgImage != null) _bgImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
