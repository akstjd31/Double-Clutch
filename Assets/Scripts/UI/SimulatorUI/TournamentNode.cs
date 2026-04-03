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


    [SerializeField] private Image _verticalPipeImage; // 위/아래로 꺾이는 세로선 
    [SerializeField] private Image _inPipeImage;       // 노드에서 오른쪽으로 뻗는 짧은 가로선
    [SerializeField] private Image _outPipeImage;      // 다음 라운드로 이어지는 겹치는 가로선

    // 승/패 점수 표시용 텍스트 (프리팹의 '승 000-000' 텍스트 연결)
    [SerializeField] private TextMeshProUGUI _txtScore;

    public string TeamId { get; private set; }
    public int RoundIndex { get; private set; }


    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void OnDisable()
    {
        StringManager.OnLanguageChanged -=Refresh;
    }
    public void DisableNode()
    {
        gameObject.SetActive(false);
        if (_verticalPipeImage != null) _verticalPipeImage.gameObject.SetActive(false);
        if (_inPipeImage != null) _inPipeImage.gameObject.SetActive(false);
        if (_outPipeImage != null) _outPipeImage.gameObject.SetActive(false);
    }

    // 이 슬롯이 현재 라운드인지 판별
    public void Init(string teamId, int roundIndex, bool isCurrentRound, bool isEliminatedInThisMatch, bool isWinnerOfThisMatch, bool isAdvancedToHere = false, string scoreText = "")
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

        // 경기 준비 중인 애들 선 다시 켜주기
        if (_verticalPipeImage != null) _verticalPipeImage.gameObject.SetActive(true);
        if (_inPipeImage != null) _inPipeImage.gameObject.SetActive(true);

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
        Color highlightColor = new Color(1f, 0.84f, 0f); // 진출 노란색
        Color activeColor = Color.white;                 // 매칭됨 / 경기 진행 중 (흰색)
        Color inactiveColor = Color.black;               // 기본 미정 / 패배 (검은색)
        // 아직 미정인 슬롯 ('?' 처리)
        if (teamId == "?")
        {
            _txtTeamName.text = "?";
            _txtTeamName.fontStyle = FontStyles.Normal;
            if (_outline != null) _outline.enabled = false;
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            if (_inPipeImage != null) _inPipeImage.color = inactiveColor;
            if (_verticalPipeImage != null) _verticalPipeImage.color = inactiveColor;
            if (_outPipeImage != null) _outPipeImage.color = inactiveColor;

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

        // 슬롯 테두리 하이라이트 (현재 진행 중인 라운드의 내 팀 슬롯에만 한정)
        if (_outline != null)
        {
            _outline.enabled = (isMyTeam && isCurrentRound);
        }

        var league = LeagueManager.Instance.CurrentLeague;
        int currentLeagueRound = league != null ? (league.isFinished ? league.currentRoundIndex - 1 : league.currentRoundIndex) : 0;

        bool isFutureRound = (roundIndex > currentLeagueRound);

        //  내 박스에서 뻗어나가는 짧은 가로선과 반쪽짜리 세로선 (안 겹침) -> 이겼으면 노랑
        Color myColor;
        if (isWinnerOfThisMatch) myColor = highlightColor;       // 이겼으면 노란색
        else if (isEliminatedInThisMatch) myColor = inactiveColor; // 졌으면 검은색 (비활성화 느낌)
        else if (isFutureRound) myColor = inactiveColor;
        else myColor = activeColor;


        if (_verticalPipeImage != null) _verticalPipeImage.color = myColor;
        if (_inPipeImage != null) _inPipeImage.color = myColor;

        //  다음 노드로 향하는 긴 가로선 (_outPipeImage)
        if (_outPipeImage != null)
        {
            if (isWinnerOfThisMatch)
            {
                // 이겼으면 켜고 노란색 칠하기
                _outPipeImage.gameObject.SetActive(true);
                _outPipeImage.color = highlightColor;
            }
            else if (!isEliminatedInThisMatch && !isWinnerOfThisMatch)
            {
                // 아직 경기 전이면 켜고 하얀색 유지
                _outPipeImage.gameObject.SetActive(true);
                _outPipeImage.color = isFutureRound ? inactiveColor : activeColor;
            }
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

    private void Refresh()
    {
        _txtTeamName.text = GetTeamName(TeamId);
        StringManager.Instance.ApplyFont(_txtTeamName);
    }
}