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
    [Header("���� ����â ���� UI")]
    [SerializeField] ProfileDetailsPanel _profileDetailsPanel;
    [SerializeField] PassiveExplainBox _passiveExplainBox;
    [SerializeField] Button _backBotton;

    [Header("���� ����â ���� UI")]
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
    private void Awake()
    {
        Instance = this;
    }


    public void OnCharacterBoxClick(Student student) //ĳ���� �ڽ� ��ư ��Ŭ������ ȣ��
    {
        _profileDetailsPanel.gameObject.SetActive(true);
        _profileDetailsPanel.Init(student);
    }

    public void OnPassiveBoxMouseOverStart(Player_PassiveData data) //�нú� ������ �ڽ��� OnPointerEnter���� ȣ��
    {        
        _passiveExplainBox.gameObject.SetActive(true);        
        _passiveExplainBox.Init(data);
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
        _startFosterButtonCount.text = $"���� ���� {currentCount} / {maxCount}";
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
}
