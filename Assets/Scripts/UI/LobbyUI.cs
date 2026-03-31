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
    [SerializeField] private SwissBoardPanel _swissBoardPanel; // 대진표 연결용
    [SerializeField] private TournamentBoardPanel _tournamentBoardPanel;

    [Header("Setting")]
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private Toggle _koreanToggle;
    [SerializeField] private Toggle _englishToggle;
    [SerializeField] private Toggle _japanToggle;


    [SerializeField] private Button _testButton;
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
        StringManager.OnLanguageChanged += RefreshCalendarText;
    }

    private void Start()
    {
        CheckAndRecoverLeagueDesync();
        Init();
        // 매치 버튼에 이벤트 연결
        if (_matchButton != null)
        {
            _matchButton.onClick.RemoveAllListeners();
            _matchButton.onClick.AddListener(OnClickMatchButton);
        }

        PlaySound();

        var gm = GameManager.Instance;
        if (gm == null) return;
        int weekId = gm.SaveData.weekId;

        if (weekId == 8)
        {
            gm.GoToGraduation();
        }
        else if (weekId == 1)
        {
            CalendarManager.Instance.CheckEnding(weekId);
        }
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.StopSound();
    }


    private void PlaySound()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound(SoundName.BGM_LOBBY_01);
    }

    // 매치 버튼을 눌렀을 때 실행될 함수
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
                // 스위스 패널이 켜져있다면 확실하게 꺼줍니다!
                if (_swissBoardPanel != null) _swissBoardPanel.gameObject.SetActive(false);
                if (_tournamentBoardPanel != null) _tournamentBoardPanel.OpenPanel(null, btnText);
                else if (!cannotPlay) GameManager.Instance.ChangeState<MatchPrepState>();
            }
            else
            {
                // 토너먼트 패널이 켜져있다면 확실하게 꺼줍니다!
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

        // 앨범 데이터 유무로 앨범 버튼 활성화 유무 결정
        if (GraduationAlbumManager.Instance == null) return;
        if (GraduationAlbumManager.Instance.HasData())
            _graduationAlbumButton.interactable = true;
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
        StringManager.OnLanguageChanged -= RefreshCalendarText;
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
        _calendarText.text = StringManager.Instance.GetFormattedString("UI_Calendar_달력",GameManager.Instance.SaveData.year, calendar.month, calendar.week);
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

        if (GameManager.Instance == null) return;
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

    //  결산창 강제종료 복구 로직 
    private void CheckAndRecoverLeagueDesync()
    {
        var calMgr = CalendarManager.Instance;
        if (calMgr == null) return;

        // 달력은 '리그'인데, 실제 리그 데이터는 '종료'된 모순 상태인지 확인
        if (calMgr.CurrentGetPhaseType() == phaseType.League)
        {
            var currentLeague = LeagueManager.Instance.CurrentLeague;
            if (currentLeague != null && (currentLeague.isFinished || currentLeague.isPlayerEliminated))
            {
                Debug.LogWarning("[LobbyUI] 결산 전 강제종료 감지. 누락된 주차 넘김 및 페널티 정산을 마저 수행합니다.");

                // ResultState에서 못하고 꺼진 '출전 페널티' 마저 적용
                ApplyLeagueEndConditionDropToAllPlayers();

                // 임시 로그 및 포지션 초기화
                // 저장 타이밍으로 인한 버그로 생각되는 부분이 있어 주석 처리
                //if (LeagueRecordManager.Instance != null)
                //    LeagueRecordManager.Instance.ClearLeagueRecords();

                if (StudentManager.Instance != null && StudentManager.Instance.MyStudents != null)
                {
                    foreach (var student in StudentManager.Instance.MyStudents)
                        student.SetMatchPosition(Position.None);
                }

                // 데이터 저장 (매칭 UI 초기화)
                var emptyData = new StudentSaveData();
                if (SaveLoadManager.Instance != null)
                    SaveLoadManager.Instance.Save<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, emptyData);

                // 달력을 다음 주로 넘기기
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
                    if (id < 10000) participatedIds.Add(id); // 용병 제외
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
