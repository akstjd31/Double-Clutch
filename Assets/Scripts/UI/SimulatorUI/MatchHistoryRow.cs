using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MatchHistoryRow : MonoBehaviour
{
    // 예: "1라운드 승"
    [SerializeField] private TextMeshProUGUI _textRoundAndResult;

    // 예: "(10 : 8) 플레이어 팀 vs 라이벌 고교"
    [SerializeField] private TextMeshProUGUI _textScoreAndTeam;

    // 로그 확인 버튼
    [SerializeField] private Button _btnLogCheck;

    public void Init(int round, MatchResultRecord record, Action<int> onClickLog)
    {
        // 승/패 판별 (홈팀(유저) 점수 기준)
        string result = record.HomeScore >= record.AwayScore ? "승" : "패";

        // 텍스트 UI 적용
        if (_textRoundAndResult != null)
            _textRoundAndResult.text = $"{round}라운드 {result}";

        if (_textScoreAndTeam != null)
            _textScoreAndTeam.text = $"({record.HomeScore} : {record.AwayScore}) {record.HomeTeamName} vs {record.AwayTeamName}";

        // 이 줄의 로그 버튼을 누르면 자신의 라운드 번호를 들고 로그 패널을 열도록 연결
        _btnLogCheck.onClick.RemoveAllListeners();
        _btnLogCheck.onClick.AddListener(() => onClickLog?.Invoke(round));
    }
}