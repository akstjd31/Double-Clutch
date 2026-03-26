using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MatchHistoryRow : MonoBehaviour
{
    // 예: "1라운드 승"
    [SerializeField] private TextMeshProUGUI _textRoundAndResult;

    // 로그 확인 버튼
    [SerializeField] private Button _btnLogCheck;

    [Header("Team & Score UI")]
    [SerializeField] private TextMeshProUGUI _textHomeScore;    // 우리 팀 점수 (예: 1234)
    [SerializeField] private TextMeshProUGUI _textHomeTeam;     // 우리 팀 이름 (예: 플레이어 고등학교)
    [SerializeField] private TextMeshProUGUI _textAwayTeam;     // 상대 팀 이름 (예: 라이벌 고등학교)
    [SerializeField] private TextMeshProUGUI _textAwayScore;    // 상대 팀 점수 (예: 4567)


    public void Init(int round, MatchResultRecord record, Action<int> onClickLog)
    {
        // 승/패 판별 (홈팀(유저) 점수 기준)
        string result = record.HomeScore >= record.AwayScore ? StringManager.Instance.GetString("UI_Match_승리") : StringManager.Instance.GetString("UI_Match_패배");

        // 텍스트 UI 적용
        if (_textRoundAndResult != null)
        {
            _textRoundAndResult.text = round+StringManager.Instance.GetString("UI_RoundBox_라운드")+" "+result;
            StringManager.Instance.ApplyFont(_textRoundAndResult);
        }

        if (_textHomeScore != null)
            _textHomeScore.text = record.HomeScore.ToString();

        if (_textHomeTeam != null)
            _textHomeTeam.text = record.HomeTeamName;

        if (_textAwayTeam != null)
            _textAwayTeam.text = record.AwayTeamName;

        if (_textAwayScore != null)
            _textAwayScore.text = record.AwayScore.ToString();

        // 이 줄의 로그 버튼을 누르면 자신의 라운드 번호를 들고 로그 패널을 열도록 연결
        _btnLogCheck.onClick.RemoveAllListeners();
        _btnLogCheck.onClick.AddListener(() => onClickLog?.Invoke(round));
        
    }
}
