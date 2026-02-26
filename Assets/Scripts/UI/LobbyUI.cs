using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _calendarText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _honorText;
    [SerializeField] private Button _trainingButton;
    [SerializeField] private Button _matchButton;


    private void OnEnable()
    {
        if (CalendarManager.Instance != null)
            CalendarManager.Instance.OnWeekChanged += UpdateCalendarText;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDataChanged += UpdateMoneyText;
            GameManager.Instance.OnDataChanged += UpdateHonorText;
        }
    }

    // Update is called once per frame
    private void Start()
    {
        // 데이터 로드하면서 값 받아오기
        var gameMgr = GameManager.Instance;
        if (gameMgr == null) return;

        UpdateMoneyText();
        UpdateHonorText();

        var calMgr = CalendarManager.Instance;
        if (calMgr == null) return;

        UpdateCalendarText(calMgr.GetCalendar());
        SetButtonActivate(calMgr.GetPhaseType(gameMgr.SaveData.weekId));
    }

    private void OnDisable()
    {
        if (CalendarManager.Instance != null)
            CalendarManager.Instance.OnWeekChanged -= UpdateCalendarText;
            
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDataChanged -= UpdateMoneyText;
            GameManager.Instance.OnDataChanged -= UpdateHonorText;
        }
    }

    public void UpdateCalendarText(Calendar calendar)
    {
        _calendarText.text = $"{calendar.year}년차 {calendar.month}월 {calendar.week}주";
    }

    public void UpdateMoneyText()
    {
        _moneyText.text = $"{GameManager.Instance.SaveData.money.ToString("N0")}G";
    }

    public void UpdateHonorText()
    {
        _honorText.text = GameManager.Instance.SaveData.honor.ToString("N0");
    }

    public void OnClickConfirmButton()
    {
        if (CalendarManager.Instance == null) return;
        
        CalendarManager.Instance.IsEndPhase = true;
        CalendarManager.Instance.NextTurn();
    }

    // 육성 or 경기 시작 버튼
    public void SetButtonActivate(phaseType type)
    {
        if (type.Equals(phaseType.League))
        {
            _matchButton.gameObject.SetActive(true);
            _trainingButton.gameObject.SetActive(false);
        }
        else    //else if (type.Equals(phaseType.Training))
        {
            _matchButton.gameObject.SetActive(false);
            _trainingButton.gameObject.SetActive(true);
        }
    }
}
