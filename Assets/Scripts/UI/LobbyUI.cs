using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _schoolText;
    [SerializeField] private TextMeshProUGUI _coachText;
    [SerializeField] private TextMeshProUGUI _calendarText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _honorText;
    [SerializeField] private Button _trainingButton;
    [SerializeField] private Button _matchButton;

    [Header("Setting")]
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private Toggle _koreanToggle;
    [SerializeField] private Toggle _englishToggle;
    [SerializeField] private Toggle _japanToggle;
    private void OnEnable()
    {
        if (CalendarManager.Instance != null)
        {
            CalendarManager.Instance.OnWeekChanged += UpdateCalendarText;
            CalendarManager.Instance.OnWeekChanged += SetButtonActivate;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDataChanged += UpdateMoneyText;
            GameManager.Instance.OnDataChanged += UpdateHonorText;
            GameManager.Instance.OnDataChanged += UpdateProfileText;
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (GameManager.Instance == null) return;
        UpdateMoneyText();
        UpdateHonorText();
        UpdateProfileText();

        var calMgr = CalendarManager.Instance;
        if (calMgr == null) return;

        UpdateCalendarText(calMgr.GetCalendar());
        SetButtonActivate(calMgr.GetCalendar());
    }

    private void OnDisable()
    {
        if (CalendarManager.Instance != null)
        {
            CalendarManager.Instance.OnWeekChanged -= UpdateCalendarText;
            CalendarManager.Instance.OnWeekChanged -= SetButtonActivate;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDataChanged -= UpdateMoneyText;
            GameManager.Instance.OnDataChanged -= UpdateHonorText;
            GameManager.Instance.OnDataChanged -= UpdateProfileText;
        }
    }

    // ───── Setting ─────

    public void OnMenuButtonClick()
    {
        _settingPanel.SetActive(!_settingPanel.activeSelf);
    }

    public void OnKoreanToggleChanged(bool isOn)
    {
        if (isOn) StringManager.Instance.SetLanguage(Language.Ko);
    }

    public void OnEnglishToggleChanged(bool isOn)
    {
        if (isOn) StringManager.Instance.SetLanguage(Language.En);
    }

    public void OnJapanToggleChanged(bool isOn)
    {
        if (isOn) StringManager.Instance.SetLanguage(Language.Ja);
    }

    // ───── Calendar / Money / Honor ─────

    public void UpdateCalendarText(Calendar calendar)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.SaveData == null) return;

        _calendarText.text = $"{GameManager.Instance.SaveData.year}년차 {calendar.month}월 {calendar.week}주";
    }

    public void UpdateMoneyText()
    {
        _moneyText.text = $"{GameManager.Instance.SaveData.money.ToString("N0")}G";
    }

    public void UpdateHonorText()
    {
        _honorText.text = GameManager.Instance.SaveData.honor.ToString("N0");
    }

    public void UpdateProfileText()
    {
        var data = GameManager.Instance.SaveData;
        _schoolText.text = data.schoolName;
        _coachText.text = data.coachName;
    }

    public void OnClickConfirmButton()
    {
        if (CalendarManager.Instance == null) return;

        CalendarManager.Instance.IsEndPhase = true;
        CalendarManager.Instance.NextTurn();

        if (GameManager.Instance == null) return;
    }

    public void SetButtonActivate(Calendar calendar)
    {
        var type = CalendarManager.Instance.CurrentGetPhaseType();
        if (type.Equals(phaseType.League))
        {
            _matchButton.gameObject.SetActive(true);
            _trainingButton.gameObject.SetActive(false);
        }
        else if (type.Equals(phaseType.Training) || type.Equals(phaseType.Event))
        {
            _matchButton.gameObject.SetActive(false);
            _trainingButton.gameObject.SetActive(true);
        }
    }
}
