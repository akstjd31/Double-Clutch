using UnityEngine;
using TMPro;
using System.Collections;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _calendarText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _honorText;

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

        if (CalendarManager.Instance == null) return;

        UpdateCalendarText(CalendarManager.Instance.GetCalendar());
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
}
