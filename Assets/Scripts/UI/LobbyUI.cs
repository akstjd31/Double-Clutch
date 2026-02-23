using UnityEngine;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    
    [SerializeField] private GameObject _tutorialObj;
    [SerializeField] private Text _calendarText;
    [SerializeField] private Text _moneyText;

    private void Awake()
    {
        if (GameManager.Instance == null) return;
        if (CalendarManager.Instance == null) return;

        CalendarManager.Instance.OnWeekChanged += UpdateCalendarText;
        GameManager.Instance.OnMoneyChanged += UpdateMoneyText;
    }

    // Update is called once per frame
    private void Start()
    {
        _tutorialObj.SetActive(IsFirstRun());

        UpdateCalendarText(CalendarManager.Instance.GetCalendar());
    }

    private void OnDestroy()
    {
        if (CalendarManager.Instance == null) return;
        if (GameManager.Instance == null) return;
        

        CalendarManager.Instance.OnWeekChanged -= UpdateCalendarText;
        GameManager.Instance.OnMoneyChanged -= UpdateMoneyText;
    }

    // 처음 실행하는건지?
    private bool IsFirstRun() => PlayerPrefs.GetInt(PrefKeys.KEY_FIRST_RUN_DONE, 0) == 0;

    public void UpdateCalendarText(Calendar calendar)
    {
        _calendarText.text = $"{calendar.year}년차 {calendar.month}월 {calendar.week}주";
    }

    public void UpdateMoneyText(int money)
    {
        _moneyText.text = $"{money.ToString("N0")}G";
    }
}
