using System.Collections.Generic;
using Game.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// ?κ? ????? ???? UI ???? ???¸? ??????? ?? ????? ?????
/// ?????? ????? Init ???? ?л? ???? ???? ???
/// </summary>
public class StudentUIManager : MonoBehaviour
{
    public static StudentUIManager Instance;
    [Header("선수 관련 UI")]
    [SerializeField] ProfileDetailsPanel _profileDetailsPanel;
    [SerializeField] PassiveExplainBox _passiveExplainBox;
    [SerializeField] Button _backBotton;

    [Header("육성 관련 UI")]
    [SerializeField] TrainingPanel _trainingPanel;
    [SerializeField] IndividualTrainingCommandPopUp _individualTrainingCommandPopUp;
    [SerializeField] TeamTrainingCommandPopUp _teamTrainingCommandPopUp;

    [SerializeField] ConditionWarningPopUp _conditionWarningPopUp;
    [SerializeField] StateWarningPopUp_Individual _stateWarningPopUp_Individual;
    //[SerializeField] GameObject _stateWarningPopUp_Team;    
    [SerializeField] WeeklyTrainingReportPopUp _weeklyTrainingReportPopUp;
    [SerializeField] TrainingStartConfirmPopUp _trainingStartConfirmPopUp;
    [SerializeField] Button _startFosterButton;    
    [SerializeField] TextMeshProUGUI _startFosterButtonCount;
    [SerializeField] GameObject _costWarningPopUp;

    [Header("영입 & 방출 관련 UI")]
    [SerializeField] CharacterRecruitPanel _characterRecruitPanel;
    [SerializeField] RecruitWarningPopUp _recruitWarningPopUp;
    [SerializeField] RecruitConfirmPopUp _recruitConfirmPopUp;
    [SerializeField] CharacterOutPanel _characterOutPanel;
    [SerializeField] OutWarningPopUp _outWarningPopUp;
    [SerializeField] GameObject _cantOutWarningPopUp;
    [SerializeField] OutConfirmPopUp _outConfirmPopUp;
    [SerializeField] CharacterProfilePopUp _characterProfilePopUp;
    [SerializeField] PassiveExplainBox _passiveProfileBox;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (CalendarManager.Instance == null) return;

        var cal = CalendarManager.Instance.GetCalendar();
        
        if (cal.month == 3 && cal.week == 1)
            OpenRecruitPanel();
    }

    public void OnCharacterBoxClick(CharacterBox box) //ĳ???? ??? ??? ????????? ???
    {
        PlayConfirmSound();
        _profileDetailsPanel.gameObject.SetActive(true);
        _profileDetailsPanel.Init(box.Target);        
    }

    public void OnPassiveBoxMouseOverStart(Player_PassiveData? data) //?н?? ?????? ????? OnPointerEnter???? ???
    {        
        if (!data.HasValue)
        {
            _passiveExplainBox.gameObject.SetActive(false);
            return;
        }
        if (string.IsNullOrEmpty(data.Value.skillId))
        {
            _passiveExplainBox.gameObject.SetActive(false);
            return;
        }
        _passiveExplainBox.gameObject.SetActive(true);        
        _passiveExplainBox.Init(data.Value);
    }

    public void OnPassiveBoxMouseOverEnd() //?н?? ?????? ??? OnPointerExit???? ???
    {
        _passiveExplainBox.gameObject.SetActive(false);        
    }

    public void OnTrainingButtonClick() //?κ? ????? ??????? ????????? ???
    {
        // ?????? ???? ??? ???
        _trainingPanel.gameObject.SetActive(true);

        FosterManager.Instance.UpdateScheduleState();

        // ??÷? ???? ???(MatchPrep)?? ??? ????? ????
        // GameManager.Instance.ChangeState<MatchPrepState>();
    }

    public void OnTrainingCharacterBoxClick(Student target) //?÷???? ??? ????????? ???
    {
        _individualTrainingCommandPopUp.gameObject.SetActive(true);
        _individualTrainingCommandPopUp.Init(target);
    }

    public void OnTrainingBoxClick()
    {
        PlayConfirmSound();
        _individualTrainingCommandPopUp.gameObject.SetActive(false);
        _teamTrainingCommandPopUp.gameObject.SetActive(false);
    }

    public void OnTrainingReserved()
    {
        _trainingPanel.RefreshAllBoxesState();
    }

    public void OnTeamTrainingButtonClick() //??? ?????? ?ν????? ????? ??? OK
    {
        PlayConfirmSound();
        _teamTrainingCommandPopUp.gameObject.SetActive(true);
    }

    public void OpenStateWarningPopUp_Individual(Student target) //???, ?λ? ???????? ??? ???? ??? ????(???? ??? ????)
    {
        _stateWarningPopUp_Individual.gameObject.SetActive(true);
        _stateWarningPopUp_Individual.Init(target);
    }

    //public void OpenStateWarningPopUp_Team()//???, ?λ? ???????? ??? ???? ??? ????(?? ??? ????)
    //{
    //    _stateWarningPopUp_Team.gameObject.SetActive(true);
    //}

    public void RefreshStartFosterButton(bool isInteractable, int currentCount, int maxCount)
    {
        _startFosterButton.interactable = isInteractable;
        _startFosterButtonCount.text = $"{currentCount} / {maxCount}";
    }

    public void OpenConditionWarningPopUp(List<Student> targets, int cost)
    {
        PlayWarningSound();
        _conditionWarningPopUp.gameObject.SetActive(true);
        _conditionWarningPopUp.Init(targets, cost);
    }    

    public void OpenTrainingStartConfirmPopUp(int cost)
    {
        _trainingStartConfirmPopUp.gameObject.SetActive(true);
        _trainingStartConfirmPopUp.Init(cost);        
    }

    public void OpenCostWarningPopUp()
    {
        PlayWarningSound();
        _costWarningPopUp.SetActive(true);
    }

    public void OnConfirmButtonClick()
    {
        PlayConfirmSound();
        FosterManager.Instance.StartFoster();
    }

    public void OpenWeeklyTrainingReportPopUp(List<Student> students)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_REPORT);
            
        _weeklyTrainingReportPopUp.gameObject.SetActive(true);
        _weeklyTrainingReportPopUp.Init(students);
    }

    public void OpenRecruitPanel() //영입 시작하려면 이거 호출!
    {
        _characterRecruitPanel.gameObject.SetActive(true);
        //_characterRecruitPanel.Init();
    }

    public void OpenRecruitWarningPopUp(int number)
    {
        _recruitWarningPopUp.gameObject.SetActive(true);
        _recruitWarningPopUp.Init(number);
    }

    public void OpenRecruitConfirmPopUp(int number)
    {
        _recruitConfirmPopUp.gameObject.SetActive(true);
        _recruitConfirmPopUp.Init(number);
    }

    public void OpenCharacterOutPanel()
    {
        _characterOutPanel.gameObject.SetActive(true);
        _characterOutPanel.Init();
    }

    public void OpenOutWarningPopUp(int number)
    {
        PlayWarningSound();
        _outWarningPopUp.gameObject.SetActive(true);
        _outWarningPopUp.Init(number);
    }

    public void OpenCantOutWarningPopUp()
    {
        PlayWarningSound();
        _cantOutWarningPopUp.gameObject.SetActive(true);        
    }
    public void OpenOutConfirmPopUp(int number)
    {
        _outConfirmPopUp.gameObject.SetActive(true);
        _outConfirmPopUp.Init(number);
    }
    
    public void OpenProfilePopUp(Student student)// 2초 누르면 호출될 함수
    {
        if (_characterProfilePopUp != null)
        {
            _characterProfilePopUp.gameObject.SetActive(true);
            _characterProfilePopUp.Init(student);
        }
    }

    public void OnPassiveProfilePopUpStart(Player_PassiveData data)
    {
        if (string.IsNullOrEmpty(data.skillId))
        {
            _passiveExplainBox.gameObject.SetActive(false);
            return;
        }
        _passiveProfileBox.gameObject.SetActive(true);
        _passiveProfileBox.Init(data);
    }

    public void OnPassiveProfilePopUpEnd()
    {
        _passiveProfileBox.gameObject.SetActive(false);
    }

    private void PlayWarningSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.UI_WARNING_01);
    }

    private void PlayConfirmSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
    }
}
