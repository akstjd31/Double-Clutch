using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private const string KEY_FIRST_RUN_DONE = "FIRST_RUN_DONE";
    [SerializeField] private GameObject _tutorialObj;
    [SerializeField] private Text calendarText;

    private void Awake()
    {
        if (CalendarManager.Instance == null) return;

        CalendarManager.Instance.OnWeekChanged += UpdateCalendarText;
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

        CalendarManager.Instance.OnWeekChanged -= UpdateCalendarText;
    }

    // 처음 실행하는건지?
    private bool IsFirstRun() => PlayerPrefs.GetInt(KEY_FIRST_RUN_DONE, 0) == 0;

    public void UpdateCalendarText(Calendar calendar)
    {
        calendarText.text = $"{calendar.year}년차 {calendar.month}월 {calendar.week}주";
    }
}
