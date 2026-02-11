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

    [Header("Match Log")]
    [SerializeField] private TextMeshProUGUI _textLogMessage;

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
}