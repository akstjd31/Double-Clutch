using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;
using System;

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
    [SerializeField] private GameObject _resultPanel;              // 결과 패널 전체
    [SerializeField] private TextMeshProUGUI _textResultHomeScore; // 아군 점수 (ex: 10)
    [SerializeField] private TextMeshProUGUI _textResultHomeName;  // 아군 이름 (ex: 플레고)
    [SerializeField] private TextMeshProUGUI _textResultAwayScore; // 적군 점수 (ex: 9)
    [SerializeField] private TextMeshProUGUI _textResultAwayName;  // 적군 이름 (ex: 라이벌고)
    [SerializeField] private TextMeshProUGUI _textResultTitle;     // 승패 텍스트 (ex: 승리 / 패배)
    [SerializeField] private TextMeshProUGUI _textResultReward;    // 지원금 텍스트 (ex: 지원금 : +49)

    [Header("Match Log")]
    [SerializeField] private TextMeshProUGUI _textLogMessage;

    [Header("Quarter End Popup")]
    [SerializeField] private GameObject _quarterEndPanel; // "2쿼터 종료" 팝업 전체

    [Header("Event Popup")]
    [SerializeField] private GameObject _eventPanel; // 하프타임 이벤트 패널 (유니티에서 연결)

    [Header("Cut-In Effect")]
    [SerializeField] private GameObject _cutInPanel;      // 컷인 전체 패널 (Canvas 내 Panel)
    [SerializeField] private Image _cutInImage;           // 컷인 이미지 출력부 (UI Image)
    [SerializeField] private TextMeshProUGUI _cutInText;  // 컷인 텍스트 (DUNK! 등)

    [Header("Settings UI")]
    [SerializeField] private GameObject _settingPanel;

    // 유니티 에디터에서 연결할 스프라이트들
    [SerializeField] private Sprite _spriteDunk;
    [SerializeField] private Sprite _spriteThreePoint;
    [SerializeField] private Sprite _spriteBuzzerBeater;

    // 로그 히스토리를 저장할 리스트와 최대 표시 줄 수
    [SerializeField] private int _maxLogLines = 5; // 한 화면에 보여줄 최대 로그 개수 (UI 크기에 맞춰 조절)
    private List<string> _logHistory = new List<string>();

    [Header("Playback Speed UI")]
    [SerializeField] private MatchReplayer _replayer; // 배속을 조절할 리플레이어 참조
    [SerializeField] private TextMeshProUGUI _textSpeedButton; // 버튼 위에 "1.0x"라고 표시될 텍스트

    // 기획서 기준 배속 단계
    private float[] _speedSteps = { 1.0f, 2.0f, 4.0f, 8.0f };
    private int _currentSpeedIndex = 0; // 현재 선택된 배속의 인덱스

    // 쿼터 종료 확인 버튼을 눌렀는지 체크하는 프로퍼티
    public bool IsQuarterEndConfirmed { get; private set; } = false;

    // 이벤트 선택 상태 확인용 프로퍼티
    public bool IsEventSelected { get; private set; } = false;
    public int SelectedEventIndex { get; private set; } = -1;

    // 확인 버튼을 눌렀을 때 실행할 함수를 담아둘 변수 추가
    private Action _onResultConfirmAction;

    // 점수판 갱신 (시간, 점수)
    public void UpdateScoreBoard(MatchState state)
    {
        // 쿼터 표시 수정 (4쿼터 이하는 1Q~4Q, 그 이상은 OT1, OT2...)
        if (state.CurrentQuarter <= 4)
        {
            _textQuarter.text = $"{state.CurrentQuarter}Q";
        }
        else
        {
            _textQuarter.text = $"OT{state.CurrentQuarter - 4}";
        }

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
        // 새 메시지를 리스트에 추가
        _logHistory.Add(message);

        // 최대 줄 수를 넘어가면 가장 오래된(위쪽) 로그 삭제
        if (_logHistory.Count > _maxLogLines)
        {
            _logHistory.RemoveAt(0);
        }

        // 텍스트 결합 (최신 로그는 흰색으로 강조, 이전 로그는 회색으로 처리)
        string combinedText = "";
        for (int i = 0; i < _logHistory.Count; i++)
        {
            if (i == _logHistory.Count - 1)
            {
                // 방금 들어온 최신 로그
                combinedText += $"<color=#FFFFFF>{_logHistory[i]}</color>";
            }
            else
            {
                // 이미 지나간 이전 로그들
                combinedText += $"<color=#888888>{_logHistory[i]}</color>\n";
            }
        }

        // UI 텍스트에 적용
        _textLogMessage.text = combinedText;
    }
    // 경기가 새로 시작될 때 로그 창을 깨끗하게 비워주는 함수
    public void ClearLog()
    {
        _logHistory.Clear();
        _textLogMessage.text = "";
    }

    // 컷인 연출 실행 함수
    public void ShowCutInEffect(string type, float speed = 1.0f)
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
        StartCoroutine(CoPlayCutInAnim(speed));
    }

    private IEnumerator CoPlayCutInAnim(float speed)
    {
        _cutInPanel.SetActive(true);
        _cutInPanel.transform.localScale = Vector3.zero;

        // 팍 하고 튀어나옴 (DOTween)
        _cutInPanel.transform.DOScale(1.2f, 0.2f / speed).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f / speed);

        // 원래 크기로 살짝 복귀
        _cutInPanel.transform.DOScale(1.0f, 0.1f / speed);

        // 1초 유지 (강조 시간)
        yield return new WaitForSeconds(1.0f / speed);

        // 사라짐
        _cutInPanel.transform.DOScale(0f, 0.2f / speed).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.2f / speed);

        _cutInPanel.SetActive(false);
    }


    // 경기 종료 시 호출할 함수
    public void ShowResultPopup(string homeName, int homeScore, string awayName, int awayScore, int rewardAmount = 0, Action onConfirm = null)
    {
        _onResultConfirmAction = onConfirm;
        // 패널 켜기
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(true);

            // 점수 및 팀 이름 세팅
            if (_textResultHomeName != null) _textResultHomeName.text = homeName;
            if (_textResultHomeScore != null) _textResultHomeScore.text = homeScore.ToString();

            if (_textResultAwayName != null) _textResultAwayName.text = awayName;
            if (_textResultAwayScore != null) _textResultAwayScore.text = awayScore.ToString();

            // 승패 판정 (동점은 연장전 로직상 발생하지 않음)
            if (_textResultTitle != null)
            {
                if (homeScore > awayScore)
                {
                    _textResultTitle.text = "승리";
                    _textResultTitle.color = new Color(1f, 0.8f, 0f); // 승리 시 텍스트 색상 (노란색/금색 계열)
                }
                else
                {
                    _textResultTitle.text = "패배";
                    _textResultTitle.color = Color.white; // 패배 시 기본 흰색
                }
            }

            // 지원금 세팅 (지원금 작업 완료 전까지는 0이나 임의의 값 출력)
            if (_textResultReward != null)
            {
                _textResultReward.text = $"지원금 : +{rewardAmount}";
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
    public void OnClickSpeedButton()
    {
        if (_replayer == null)
        {
            Debug.LogWarning("[MatchUIManager] MatchReplayer가 연결되지 않았습니다.");
            return;
        }

        // 다음 배속 단계로 넘어감 (마지막 단계면 다시 0번 인덱스로)
        _currentSpeedIndex++;
        if (_currentSpeedIndex >= _speedSteps.Length)
        {
            _currentSpeedIndex = 0;
        }

        // 새로운 배속 값 적용
        float newSpeed = _speedSteps[_currentSpeedIndex];
        _replayer.PlaybackSpeed = newSpeed;

        // 버튼 텍스트 갱신
        if (_textSpeedButton != null)
        {
            _textSpeedButton.text = $"{newSpeed:F1}x";
        }
    }
    // 스킵 시 진행 중이던 컷인 연출을 즉시 없애기 위한 헬퍼 함수
    public void ForceCloseCutIn()
    {
        if (_cutInPanel != null)
        {
            _cutInPanel.transform.DOKill(); // DOTween 애니메이션 즉시 중지
            _cutInPanel.SetActive(false);
        }
    }

    // 버튼의 OnClick에 연결할 스킵 버튼 전용 함수
    public void OnClickSkipButton()
    {
        if (_replayer != null)
        {
            _replayer.SkipReplay();
        }
    }
    // [게임메뉴] 버튼을 눌렀을 때 호출할 함수
    public void OnClickGameMenuButton()
    {
        if (_settingPanel != null)
        {
            _settingPanel.SetActive(true);

            // 세팅 창이 켜졌을 때 게임을 일시정지
            Time.timeScale = 0f; 
        }
    }

    // 세팅 창 바깥 어두운 배경을 터치했을 때 호출할 함수 (파란 화살표 부분)
    public void OnClickCloseSettingButton()
    {
        if (_settingPanel != null)
        {
            _settingPanel.SetActive(false);

            // 일시정지 해제
            Time.timeScale = 1f;
        }
    }
    // 2쿼터 종료 팝업 열기
    public void ShowQuarterEndPopup()
    {
        if (_quarterEndPanel != null)
        {
            _quarterEndPanel.SetActive(true);
            IsQuarterEndConfirmed = false; // 플래그 초기화
        }
        else
        {
            // 패널이 연결 안 되어 있으면 그냥 바로 넘어간 것으로 처리
            IsQuarterEndConfirmed = true;
        }
    }

    // [확인] 버튼을 눌렀을 때 호출될 함수
    public void OnClickQuarterEndConfirm()
    {
        IsQuarterEndConfirmed = true; // 확인 완료 플래그 켜기

        if (_quarterEndPanel != null)
        {
            _quarterEndPanel.SetActive(false); // 팝업 닫기
        }
    }
    // 버튼 클릭 함수
    public void OnClickResultConfirmButton()
    {
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }

        // ResultState에서 넘겨줬던 ReturnToLobby 함수를 여기서 실행
        _onResultConfirmAction?.Invoke();
    }
}
