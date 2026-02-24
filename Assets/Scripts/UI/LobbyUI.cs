using UnityEngine;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Text _calendarText;
    [SerializeField] private Text _moneyText;

    private void OnEnable()
    {
        if (CalendarManager.Instance != null)
            CalendarManager.Instance.OnWeekChanged += UpdateCalendarText;

        if (GameManager.Instance != null)
            GameManager.Instance.OnMoneyChanged += UpdateMoneyText;
    }

    // Update is called once per frame
    private void Start()
    {
        UpdateCalendarText(CalendarManager.Instance.GetCalendar());
    }

    private void OnDisable()
    {
        if (CalendarManager.Instance != null)
            CalendarManager.Instance.OnWeekChanged -= UpdateCalendarText;
            
        if (GameManager.Instance != null)
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyText;
    }

    public void UpdateCalendarText(Calendar calendar)
    {
        _calendarText.text = $"{calendar.year}년차 {calendar.month}월 {calendar.week}주";
    }

    public void UpdateMoneyText(int money)
    {
        _moneyText.text = $"{money.ToString("N0")}G";
    }
}
