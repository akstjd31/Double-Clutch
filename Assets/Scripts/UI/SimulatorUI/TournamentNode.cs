using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentNode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtTeamName;
    [SerializeField] private Outline _outline;

    [Header("Visual Elements")]
    [SerializeField] private CanvasGroup _canvasGroup; // 전체를 어둡게 만들기 위한 컴포넌트
    [SerializeField] private Image _outPipeImage;      // 다음 라운드로 이어지는 선(파이프) 이미지

    public string TeamId { get; private set; }
    public int RoundIndex { get; private set; }

    // 이 슬롯이 현재 라운드인지 판별
    public void Init(string teamId, int roundIndex, bool isCurrentRound, bool isEliminated, bool isWinnerOfThisMatch)
    {
        TeamId = teamId;
        RoundIndex = roundIndex;

        // 빈 슬롯일 경우
        if (string.IsNullOrEmpty(teamId))
        {
            _txtTeamName.text = "";
            if (_outline != null) _outline.enabled = false;
            if (_outPipeImage != null) _outPipeImage.color = Color.white; 
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            return;
        }

        // 팀 이름 렌더링
        _txtTeamName.text = GetTeamName(teamId);

        // 플레이어 팀인지 확인
        bool isMyTeam = (teamId == StudentManager.TEAM_ID);

        // 내부 텍스트 볼드 처리 (전체 라운드 슬롯 대상)
        _txtTeamName.fontStyle = isMyTeam ? FontStyles.Bold : FontStyles.Normal;

        // 슬롯 테두리 하이라이트 (현재 진행 중인 라운드의 내 팀 슬롯에만 한정)
        if (_outline != null)
        {
            _outline.enabled = (isMyTeam && isCurrentRound);
        }
        // 탈락팀 전체 슬롯 어둡게 처리 (알파값 조절)
        if (_canvasGroup != null)
        {
            // 누적 1패라도 있으면 투명도를 40%로 낮춰 어둡게 보이게 함
            _canvasGroup.alpha = isEliminated ? 0.4f : 1f;
        }

        //  승리 파이프 노란색 하이라이트
        if (_outPipeImage != null)
        {
            // 이 매치에서 이긴 팀이라면 파이프를 노란색으로, 아니면 기본 흰색/회색으로
            _outPipeImage.color = isWinnerOfThisMatch ? new Color(1f, 0.84f, 0f) : Color.white;
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
}