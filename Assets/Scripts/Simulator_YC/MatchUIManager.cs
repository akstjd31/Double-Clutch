using UnityEngine;
using TMPro; 
using DG.Tweening; 

public class MatchUIManager : MonoBehaviour
{
    [Header("Score Board")]
    [SerializeField] private TextMeshProUGUI _textQuarter;
    [SerializeField] private TextMeshProUGUI _textTime;
    [SerializeField] private TextMeshProUGUI _textHomeScore;
    [SerializeField] private TextMeshProUGUI _textAwayScore;
    [SerializeField] private TextMeshProUGUI _textHomeName;
    [SerializeField] private TextMeshProUGUI _textAwayName;

    [Header("Result Popup")]
    [SerializeField] private GameObject _resultPanel;       // 결과 패널 전체 (껏다 켰다 할 것)
    [SerializeField] private TextMeshProUGUI _finalScoreText; // 중앙 큰 점수

    [Header("Match Log")]
    [SerializeField] private TextMeshProUGUI _textLogMessage;

    [Header("Event Popup")]
    [SerializeField] private GameObject _eventPanel; // 하프타임 이벤트 패널 (유니티에서 연결)

    // 이벤트 선택 상태 확인용 프로퍼티
    public bool IsEventSelected { get; private set; } = false;
    public int SelectedEventIndex { get; private set; } = -1;

    // 점수판 갱신 (시간, 점수)
    public void UpdateScoreBoard(MatchState state)
    {
        //  쿼터 표시
        _textQuarter.text = $"{state.CurrentQuarter}Q";

        //  남은 시간 (초 -> 분:초 변환)
        int min = Mathf.FloorToInt(state.RemainTime / 60);
        int sec = Mathf.FloorToInt(state.RemainTime % 60);
        _textTime.text = $"{min:D2}:{sec:D2}";

        //  점수 표시
        _textHomeScore.text = state.HomeTeam.Score.ToString();
        _textAwayScore.text = state.AwayTeam.Score.ToString();

        //  팀 이름
        _textHomeName.text = state.HomeTeam.TeamName;
        _textAwayName.text = state.AwayTeam.TeamName;
    }

    // 중계 로그 표시 (타자기 효과 or 그냥 텍스트)
    public void UpdateLogText(string message)
    {
        _textLogMessage.text = message;

       
        _textLogMessage.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }
    // 경기 종료 시 호출할 함수
    public void ShowResultPopup(int homeScore, int awayScore)
    {
        // 패널 켜기
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(true);

            // 최종 점수 세팅
            if (_finalScoreText != null)
            {
                _finalScoreText.text = $"{homeScore} : {awayScore}";
            }

            // 등장 연출
            _resultPanel.transform.localScale = Vector3.zero;
            _resultPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
    }
    //  하프타임 패널 열기
    public void ShowHalfTimeEvent()
    {
        if (_eventPanel != null)
        {
            _eventPanel.SetActive(true);
            IsEventSelected = false; // 선택 대기 상태로 초기화
            SelectedEventIndex = -1;

            // 등장 연출
            _eventPanel.transform.localScale = Vector3.zero;
            _eventPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
    }
    // 버튼 클릭 시 호출할 함수 (유니티 버튼 OnClick에 연결)
    public void OnClickEventButton(int index)
    {
        SelectedEventIndex = index;
        IsEventSelected = true; // 선택 완료 플래그 켜기

        // 패널 닫기
        if (_eventPanel != null)
        {
            _eventPanel.SetActive(false);
        }
    }
}
