using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogHistoryPanel : MonoBehaviour
{
    [SerializeField] private Button _btnClose;
    [SerializeField] private Button _btnPrev;
    [SerializeField] private Button _btnNext;

    [Header("로그 출력용 텍스트")]
    [SerializeField] private TextMeshProUGUI _textLogContent;

    [Header("텍스트 UI 연결")]
    [SerializeField] private TextMeshProUGUI _textLeagueName; // {LeagueName}
    [SerializeField] private TextMeshProUGUI _textRoundTitle; // {0}라운드 경기 로그
    [SerializeField] private TextMeshProUGUI _textQuarter;    // {0}쿼터

    private List<MatchLogData> _currentMatchLogs;
    private int _currentRound = 1;
    private int _currentQuarter = 1;
    private int _maxQuarter = 4;

    private void Awake()
    {
        if (_btnClose != null) _btnClose.onClick.AddListener(() => gameObject.SetActive(false));
        if (_btnPrev != null) _btnPrev.onClick.AddListener(OnClickPrevQuarter);
        if (_btnNext != null) _btnNext.onClick.AddListener(OnClickNextQuarter);
    }

    // 리그 결산 패널에서 이 함수를 호출해 전체 로그를 전달받습니다.
    public void OpenPanel(int round)
    {
        gameObject.SetActive(true);
        _currentRound = round;

        // 리그 레코드 매니저에서 해당 라운드의 전체 로그를 꺼내옵니다.
        var record = LeagueRecordManager.Instance.GetMatchRecord(_currentRound);
        if (record != null && record.FullLogs != null)
        {
            _currentMatchLogs = record.FullLogs;
            // 연장전을 갔을 수도 있으므로, 마지막 로그의 쿼터 번호를 최대 쿼터로 잡습니다.
            _maxQuarter = _currentMatchLogs.Count > 0 ? _currentMatchLogs[_currentMatchLogs.Count - 1].Quarter : 1;
        }
        else
        {
            _currentMatchLogs = new List<MatchLogData>();
            _maxQuarter = 1;
        }

        _currentQuarter = 1; // 창을 열면 항상 1쿼터부터 보여줌
        UpdateLogView();
    }

    private void OnClickPrevQuarter()
    {
        if (_currentQuarter > 1) { _currentQuarter--; UpdateLogView(); }
    }

    private void OnClickNextQuarter()
    {
        if (_currentQuarter < _maxQuarter) { _currentQuarter++; UpdateLogView(); }
    }

    private void UpdateLogView()
    {
        if (_btnPrev != null) _btnPrev.interactable = (_currentQuarter > 1);
        if (_btnNext != null) _btnNext.interactable = (_currentQuarter < _maxQuarter);

        if (_textLeagueName != null) _textLeagueName.text = "테스트 리그";
        if (_textRoundTitle != null) _textRoundTitle.text = $"{_currentRound}";
        if (_textQuarter != null) _textQuarter.text = $"{_currentQuarter}";

        if (_textLogContent == null) return;

        if (_currentMatchLogs == null || _currentMatchLogs.Count == 0)
        {
            _textLogContent.text = "로그가 없습니다.";
            return;
        }

        var quarterLogs = _currentMatchLogs.Where(log => log.Quarter == _currentQuarter).ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var log in quarterLogs)
        {
            string fullText = log.LogText;

            // 텍스트가 아예 비어있거나 공백뿐이라면 아무것도 하지 않고 다음 로그로
            if (string.IsNullOrWhiteSpace(fullText))
            {
                continue;
            }

            if (fullText.Length >= 5) // null이나 empty 체크는 위에서 끝났으므로 길이만 확인
            {
                string time = fullText.Substring(0, 5);
                string message = fullText.Length > 6 ? fullText.Substring(6).Trim() : "";

                sb.AppendLine($"{time}<pos=100>{message}");
            }
            else
            {
                // 길이가 짧은 예외 데이터는 그대로 줄바꿈해서 출력
                sb.AppendLine(fullText);
            }
        }

        _textLogContent.text = sb.ToString();
    }

}
