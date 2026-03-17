using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SwissRoundTab : MonoBehaviour
{
    [SerializeField] private Button _tabButton;
    [SerializeField] private Image _bgImage;
    [SerializeField] private TextMeshProUGUI _txtRound;
    [SerializeField] private GameObject _redDotObj; // 레드도트 아이콘

    private int _myRoundIndex;
    private Action<int> _onTabClicked;

    public void Init(int roundIndex, int currentLeagueRound, bool isFinished, Action<int> onClickAction)
    {
        _myRoundIndex = roundIndex;
        _onTabClicked = onClickAction;
        _txtRound.text = $"{roundIndex + 1}라운드";

        // 버튼 클릭 이벤트 연결
        _tabButton.onClick.RemoveAllListeners();
        _tabButton.onClick.AddListener(() => _onTabClicked?.Invoke(_myRoundIndex));

        // 미래의 라운드 (아직 진행되지않은 라운드)
        if (!isFinished && roundIndex > currentLeagueRound)
        {
            _tabButton.interactable = false;
            _bgImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // 어둡게 (비활성화)
            _txtRound.color = Color.gray;
            _redDotObj.SetActive(false);
        }
        // 현재 또는 과거 라운드
        else
        {
            _tabButton.interactable = true;
            _txtRound.color = Color.black;

            bool isViewed = PlayerPrefs.GetInt($"RedDot_{LeagueManager.Instance.CurrentLeague.leagueId}_{roundIndex}", 0) == 1;

            // 아직 안 봤고, 플레이가 끝난 과거 라운드라면 레드도트 켜기
            if (!isViewed && roundIndex < currentLeagueRound)
            {
                _redDotObj.SetActive(true);
            }
            else
            {
                _redDotObj.SetActive(false);
            }
        }
    }

    // 탭이 선택되었을 때의 시각적 강조
    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            _bgImage.color = new Color(1f, 0.9f, 0.2f, 1f); // 노란색 강조
            _txtRound.fontStyle = FontStyles.Bold;

            // 탭을 눌렀으므로 레드도트 지우기 및 저장
            if (_redDotObj.activeSelf)
            {
                _redDotObj.SetActive(false);
                PlayerPrefs.SetInt($"RedDot_{LeagueManager.Instance.CurrentLeague.leagueId}_{_myRoundIndex}", 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            // 선택되지 않은 접근 가능한 탭의 기본 색상
            if (_tabButton.interactable)
            {
                _bgImage.color = Color.white;
                _txtRound.fontStyle = FontStyles.Normal;
            }
        }
    }
}
