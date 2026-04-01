using System.Collections;
using System.Collections.Generic;
using Game.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _schoolText;
    [SerializeField] private TextMeshProUGUI _coachText;
    [SerializeField] private TextMeshProUGUI _calendarText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _honorText;
    [SerializeField] private Button _graduationAlbumButton;
    [SerializeField] private Button _trainingButton;
    [SerializeField] private Button _matchButton;
    [SerializeField] private SwissBoardPanel _swissBoardPanel;
    [SerializeField] private TournamentBoardPanel _tournamentBoardPanel;

    [Header("Setting")]
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private Toggle _koreanToggle;
    [SerializeField] private Toggle _englishToggle;
    [SerializeField] private Toggle _japanToggle;

    [SerializeField] private Button _testButton;

    private bool _isInitialized;

    private void Awake()
    {
        InitOnce();
    }

    private void Start()
    {
        HandleLobbyEnter();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.StopSound();
    }

    private void InitOnce()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        if (_matchButton != null)
        {
            _matchButton.onClick.RemoveAllListeners();
            _matchButton.onClick.AddListener(OnClickMatchButton);
        }
    }

    private void SubscribeEvents()
    {
        if (CalendarManager.Instance != null)
        {
            CalendarManager.Instance.OnWeekChanged -= UpdateCalendarText;
            CalendarManager.Instance.OnWeekChanged -= SetButtonActivate;
            CalendarManager.Instance.OnWeekChanged += UpdateCalendarText;
            CalendarManager.Instance.OnWeekChanged += SetButtonActivate;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDataChanged -= UpdateMoneyText;
            GameManager.Instance.OnDataChanged -= UpdateHonorText;
            GameManager.Instance.OnDataChanged -= UpdateProfileText;
            GameManager.Instance.OnDataChanged += UpdateMoneyText;
            GameManager.Instance.OnDataChanged += UpdateHonorText;
            GameManager.Instance.OnDataChanged += UpdateProfileText;
        }

        StringManager.OnLanguageChanged -= RefreshCalendarText;
        StringManager.OnLanguageChanged += RefreshCalendarText;
    }

    private void UnsubscribeEvents()
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

        StringManager.OnLanguageChanged -= RefreshCalendarText;
    }

    /// <summary>
    /// 로비가 활성화될 때마다 실행
    /// 중간에 껐다 켜지는 상황까지 포함
    /// </summary>
    private void HandleLobbyEnter()
    {
        var gm = GameManager.Instance;
        var calMgr = CalendarManager.Instance;

        if (gm == null || gm.SaveData == null) return;
        if (calMgr == null) return;

        CheckAndRecoverLeagueDesync();
        Init();
        PlaySound();

        int weekId = gm.SaveData.weekId;

        if (weekId == 1)
        {
            bool moved = calMgr.TryHandleWeek1LobbyFlow();

            // 엔딩씬으로 갔다면 여기서 끝
            if (gm.SaveData.hasEnding)
                return;

            if (moved)
                weekId = gm.SaveData.weekId;
        }

        if (weekId == 8)
        {
            gm.SetGraduationPending(true);
            gm.GoToGraduation();
            return;
        }
    }

    private void PlaySound()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound(SoundName.BGM_LOBBY_01);
    }

    public void OnClickMatchButton()
    {
        var currentLeague = LeagueManager.Instance.CurrentLeague;
        if (currentLeague != null)
        {
            bool cannotPlay = currentLeague.isFinished || currentLeague.isPlayerEliminated;
            string btnText = cannotPlay
                ? StringManager.Instance.GetString("UI_Popup_닫기")
                : StringManager.Instance.GetString("UI_Matchlog_경기준비");

            if (currentLeague.leagueType == "Tournament")
            {
                if (_swissBoardPanel != null) _swissBoardPanel.gameObject.SetActive(false);
                if (_tournamentBoardPanel != null) _tournamentBoardPanel.OpenPanel(null, btnText);
                else if (!cannotPlay) GameManager.Instance.ChangeState<MatchPrepState>();
            }
            else
            {
                if (_tournamentBoardPanel != null) _tournamentBoardPanel.gameObject.SetActive(false);
                if (_swissBoardPanel != null) _swissBoardPanel.OpenPanel(null, btnText);
                else if (!cannotPlay) GameManager.Instance.ChangeState<MatchPrepState>();
            }
        }
        else
        {
            GameManager.Instance.ChangeState<MatchPrepState>();
        }
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

        if (GraduationAlbumManager.Instance == null) return;
        _graduationAlbumButton.interactable = GraduationAlbumManager.Instance.HasData();
    }

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

    public void UpdateCalendarText(Calendar calendar)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.SaveData == null) return;

        _calendarText.text = StringManager.Instance.GetFormattedString(
            "UI_Calendar_달력",
            GameManager.Instance.SaveData.year,
            calendar.month,
            calendar.week
        );

        StringManager.Instance.ApplyFont(_calendarText);
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
    }

    public void PlayConfirmSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
    }

    public void PlayCancelSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_CANCEL);
    }

    public void PlayAlbumInSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_ALBUM_IN);
    }

    public void PlayWarningSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.UI_WARNING_01);
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

    private void RefreshCalendarText()
    {
        if (CalendarManager.Instance != null)
            UpdateCalendarText(CalendarManager.Instance.GetCalendar());
    }

    private void CheckAndRecoverLeagueDesync()
    {
        var calMgr = CalendarManager.Instance;
        if (calMgr == null) return;

        if (calMgr.CurrentGetPhaseType() == phaseType.League)
        {
            var currentLeague = LeagueManager.Instance.CurrentLeague;
            if (currentLeague != null && (currentLeague.isFinished || currentLeague.isPlayerEliminated))
            {
                Debug.LogWarning("[LobbyUI] 결산 전 강제종료 감지. 누락된 주차 넘김 및 페널티 정산을 마저 수행합니다.");

                ApplyLeagueEndConditionDropToAllPlayers();

                if (StudentManager.Instance != null && StudentManager.Instance.MyStudents != null)
                {
                    foreach (var student in StudentManager.Instance.MyStudents)
                        student.SetMatchPosition(Position.None);
                }

                var emptyData = new StudentSaveData();
                if (SaveLoadManager.Instance != null)
                    SaveLoadManager.Instance.Save<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, emptyData);

                calMgr.IsEndPhase = true;
                calMgr.NextTurn();
            }
        }
    }

    private void ApplyLeagueEndConditionDropToAllPlayers()
    {
        if (LeagueRecordManager.Instance == null || StudentManager.Instance == null) return;

        var currentLeague = LeagueManager.Instance.CurrentLeague;
        int totalRounds = currentLeague != null ? currentLeague.currentRoundIndex + 1 : 4;

        HashSet<int> participatedIds = new HashSet<int>();

        for (int i = 1; i <= totalRounds; i++)
        {
            var record = LeagueRecordManager.Instance.GetMatchRecord(i);
            if (record != null && record.HomePlayerIds != null)
            {
                foreach (int id in record.HomePlayerIds)
                {
                    if (id < 10000) participatedIds.Add(id);
                }
            }
        }

        foreach (int id in participatedIds)
        {
            Student realStudent = StudentManager.Instance.FindStudentById(id);
            if (realStudent != null)
            {
                if (realStudent.Condition <= 0 && realStudent.State == StudentState.None)
                {
                    int rate = UnityEngine.Random.Range(0, 100);
                    if (rate < 60)
                    {
                        realStudent.ChangeState(StudentState.OverWorked);
                        Debug.Log($"[리그 종료 후유증 복구] {realStudent.Name[0]} 학생이 과로 상태가 되었습니다.");
                    }
                    else
                    {
                        realStudent.ChangeState(StudentState.Injured);
                        Debug.Log($"[리그 종료 후유증 복구] {realStudent.Name[0]} 학생이 부상 상태가 되었습니다.");
                    }
                }

                realStudent.ChangeCondition(-30);
            }
        }

        StudentManager.Instance.SaveGame();
    }
}