using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using DG.Tweening;
using System.Collections;

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

    [Header("Cut-In Effect")]
    [SerializeField] private GameObject _cutInPanel;      // 컷인 전체 패널 (Canvas 내 Panel)
    [SerializeField] private Image _cutInImage;           // 컷인 이미지 출력부 (UI Image)
    [SerializeField] private TextMeshProUGUI _cutInText;  // 컷인 텍스트 (DUNK! 등)

    // 유니티 에디터에서 연결할 스프라이트들
    [SerializeField] private Sprite _spriteDunk;
    [SerializeField] private Sprite _spriteThreePoint;
    [SerializeField] private Sprite _spriteBuzzerBeater;


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

    // 컷인 연출 실행 함수
    public void ShowCutInEffect(string type)
    {
        Debug.Log($">>> 컷인 함수 호출됨! 타입: {type} / 패널연결여부: {(_cutInPanel != null)}");
        if (_cutInPanel == null) return;

        Sprite targetSprite = null;
        string targetText = "";

        switch (type)
        {
            case "DUNK":
                targetSprite = _spriteDunk;
                targetText = "SLAM DUNK!";
                break;
            case "3PT":
                targetSprite = _spriteThreePoint;
                targetText = "3 POINT!";
                break;
            case "BUZZER":
                targetSprite = _spriteBuzzerBeater;
                targetText = "BUZZER BEATER!";
                break;
            default:
                return; // 해당 없으면 무시
        }

        // 이미지/텍스트 세팅
        if (_cutInImage != null && targetSprite != null)
            _cutInImage.sprite = targetSprite;

        if (_cutInText != null)
            _cutInText.text = targetText;

        // 연출 시작 (코루틴)
        StartCoroutine(CoPlayCutInAnim());
    }

    private IEnumerator CoPlayCutInAnim()
    {
        _cutInPanel.SetActive(true);
        _cutInPanel.transform.localScale = Vector3.zero;

        // 팍 하고 튀어나옴 (DOTween)
        _cutInPanel.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f);

        // 원래 크기로 살짝 복귀
        _cutInPanel.transform.DOScale(1.0f, 0.1f);

        // 1초 유지 (강조 시간)
        yield return new WaitForSeconds(1.0f);

        // 사라짐
        _cutInPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.2f);

        _cutInPanel.SetActive(false);
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
