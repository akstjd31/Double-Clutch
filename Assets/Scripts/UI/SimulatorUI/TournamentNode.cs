using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Constants;

public class TournamentNode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtTeamName;
    [SerializeField] private Outline _outline;

    [Header("Visual Elements")]
    [SerializeField] private CanvasGroup _canvasGroup; // 전체를 어둡게 만들기 위한 컴포넌트
    [SerializeField] private Image _outPipeImage;      // 다음 라운드로 이어지는 선(파이프) 이미지

    // 승/패 점수 표시용 텍스트 (프리팹의 '승 000-000' 텍스트 연결)
    [SerializeField] private TextMeshProUGUI _txtScore;

    public string TeamId { get; private set; }
    public int RoundIndex { get; private set; }

    // 이 슬롯이 현재 라운드인지 판별
    public void Init(string teamId, int roundIndex, bool isCurrentRound, bool isEliminatedInThisMatch, bool isWinnerOfThisMatch, string scoreText = "")
    {
        TeamId = teamId;
        RoundIndex = roundIndex;

        // 아예 할당되지 않은 빈 슬롯 (토너먼트 사이즈상 안 쓰는 노드들) -> 완전히 꺼버림
        if (string.IsNullOrEmpty(teamId))
        {
            gameObject.SetActive(false);
            return;
        }
        // 사용되는 노드이므로 활성화
        gameObject.SetActive(true);

        // 점수 텍스트 반영 로직
        if (_txtScore != null)
        {
            if (string.IsNullOrEmpty(scoreText))
            {
                // 아직 경기를 안 해서 점수가 없으면 텍스트 꺼버림
                _txtScore.gameObject.SetActive(false);
            }
            else
            {
                _txtScore.gameObject.SetActive(true);
                // 점수 형식은 깔끔하게 "75-63" 으로만 전달
                _txtScore.text = scoreText;
            }
        }

        // 아직 미정인 슬롯 ('?' 처리)
        if (teamId == "?")
        {
            _txtTeamName.text = "?";
            _txtTeamName.fontStyle = FontStyles.Normal;
            if (_outline != null) _outline.enabled = false;
            if (_outPipeImage != null) _outPipeImage.color = Color.white;
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            return;
        }

        // 팀 이름 렌더링
        _txtTeamName.text = GetTeamName(teamId);
        StringManager.Instance.ApplyFont(_txtTeamName);

        // 플레이어 팀인지 확인
        bool isMyTeam = (teamId == PrefKeys.PLAYER_TEAM_ID);

        // 내부 텍스트 볼드 처리 (전체 라운드 슬롯 대상)
        _txtTeamName.fontStyle = isMyTeam ? FontStyles.Bold : FontStyles.Normal;

        // 진 팀은 박스 전체를 어둡게 처리 (alpha = 0.4f)
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = isEliminatedInThisMatch ? 0.4f : 1f;
        }

        // 승리한 팀의 박스에서 나가는 선만 노란색으로 변경
        if (_outPipeImage != null)
        {
            _outPipeImage.color = isWinnerOfThisMatch ? new Color(1f, 0.84f, 0f) : Color.white;
        }
        // 슬롯 테두리 하이라이트 (현재 진행 중인 라운드의 내 팀 슬롯에만 한정)
        if (_outline != null)
        {
            _outline.enabled = (isMyTeam && isCurrentRound);
        }
    }

    private string GetTeamName(string teamId)
    {
        if (teamId == PrefKeys.PLAYER_TEAM_ID)
            return GameManager.Instance.SaveData.schoolName;

        var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(teamId);
        if (rivalData != null)
            return StringManager.Instance.GetString(rivalData.Value.teamNameKey);

        return teamId;
    }
}