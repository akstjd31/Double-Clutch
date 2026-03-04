using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// �κ� ȭ���� ���� UI Ȱ��ȭ ���¸� �����ϴ� �� �̱��� �Ŵ���
/// Ȱ��ȭ�� ���ÿ� Init ȣ��� �л� ���� ���� �ʿ�
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

    public void OnCharacterBoxClick(Student student) //ĳ���� �ڽ� ��ư ��Ŭ������ ȣ��
    {
        _profileDetailsPanel.gameObject.SetActive(true);
        _profileDetailsPanel.Init(student);
    }

    public void OnPassiveBoxMouseOverStart(Player_PassiveData? data) //�нú� ������ �ڽ��� OnPointerEnter���� ȣ��
    {        
        if (!data.HasValue)
        {
            _passiveExplainBox.gameObject.SetActive(false);
            return;
        }

        _passiveExplainBox.gameObject.SetActive(true);        
        _passiveExplainBox.Init(data.Value);
    }

    public void OnPassiveBoxMouseOverEnd() //�нú� ������ �ڽ� OnPointerExit���� ȣ��
    {
        _passiveExplainBox.gameObject.SetActive(false);        
    }

    public void OnTrainingButtonClick() //�κ� ȭ���� ������ư ��Ŭ������ ȣ��
    {
        // �׽�Ʈ�� ���� �ӽ� �ּ�
        _trainingPanel.gameObject.SetActive(true);

        FosterManager.Instance.UpdateScheduleState();

        // �ӽ÷� ���� ��ġ(MatchPrep)�� �ٷ� �Ѿ�� ����
        // GameManager.Instance.ChangeState<MatchPrepState>();
    }

    public void OnTrainingCharacterBoxClick(Student target) //�÷��̾� �ڽ� ��Ŭ������ ȣ��
    {
        _individualTrainingCommandPopUp.gameObject.SetActive(true);
        _individualTrainingCommandPopUp.Init(target);
    }

    public void OnTrainingBoxClick()
    {
        _individualTrainingCommandPopUp.gameObject.SetActive(false);
        _teamTrainingCommandPopUp.gameObject.SetActive(false);
    }

    public void OnTrainingReserved()
    {
        _trainingPanel.RefreshAllBoxesState();
    }

    public void OnTeamTrainingButtonClick() //�ܼ� Ȱ��ȭ�� �ν����� ����� �ص� OK
    {
        _teamTrainingCommandPopUp.gameObject.SetActive(true);
    }

    public void OpenStateWarningPopUp_Individual(Student target) //�Ƿ�, �λ� �������� �Ʒ� �Ҵ�� �˾� ȣ���(���� �Ʒ� ����)
    {
        _stateWarningPopUp_Individual.gameObject.SetActive(true);
        _stateWarningPopUp_Individual.Init(target);
    }

    //public void OpenStateWarningPopUp_Team()//�Ƿ�, �λ� �������� �Ʒ� �Ҵ�� �˾� ȣ���(�� �Ʒ� ����)
    //{
    //    _stateWarningPopUp_Team.gameObject.SetActive(true);
    //}

    public void RefreshStartFosterButton(bool isInteractable, int currentCount, int maxCount)
    {
        _startFosterButton.interactable = isInteractable;
        _startFosterButtonCount.text = $"육성 시작 {currentCount} / {maxCount}";
    }

    public void OpenConditionWarningPopUp(List<Student> targets, int cost)
    {
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
        _costWarningPopUp.SetActive(true);
    }

    public void OnConfirmButtonClick()
    {
        FosterManager.Instance.StartFoster();
    }

    public void OpenWeeklyTrainingReportPopUp(List<Student> students)
    {
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
        _outWarningPopUp.gameObject.SetActive(true);
        _outWarningPopUp.Init(number);
    }

    public void OpenCantOutWarningPopUp()
    {
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
    
}
