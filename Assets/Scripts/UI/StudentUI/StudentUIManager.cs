using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 로비 화면의 선수 UI 활성화 상태를 관리하는 비 싱글톤 매니저
/// 활성화와 동시에 Init 호출로 학생 정보 갱신 필요
/// </summary>
public class StudentUIManager : MonoBehaviour
{
    public static StudentUIManager Instance;
    [SerializeField] ProfileDetailsPanel _profileDetailsPanel;
    [SerializeField] PassiveExplainBox _passiveExplainBox;
    [SerializeField] Button _backBotton;

    [SerializeField] TrainingPanel _trainingPanel;
    [SerializeField] IndividualTrainingCommandPopUp _trainingCommandPopUp;

    [SerializeField] ConditionWarningPopUp _conditionWarningPopUp;
    [SerializeField] PositionWarningPopUp _positionWarningPopUp;
    [SerializeField] WeeklyTrainingReportPopUp _weeklyTrainingReportPopUp;
    [SerializeField] TrainingStartConfirmPopUp _trainingStartConfirmPopUp;

    [SerializeField] GameObject _costWarningPopUp;
    private void Awake()
    {
        Instance = this;
    }


    public void OnCharacterBoxClick(Student student) //캐릭터 박스 버튼 온클릭에서 호출
    {
        _profileDetailsPanel.gameObject.SetActive(true);
        _profileDetailsPanel.Init(student);
    }

    public void OnPassiveBoxMouseOverStart(Player_PassiveData data) //패시브 프로필 박스의 OnPointerEnter에서 호출
    {        
        _passiveExplainBox.gameObject.SetActive(true);        
        _passiveExplainBox.Init(data);
    }

    public void OnPassiveBoxMouseOverEnd() //패시브 프로필 박스 OnPointerExit에서 호출
    {
        _passiveExplainBox.gameObject.SetActive(false);        
    }

    public void OnTrainingButtonClick() //로비 화면의 육성버튼 온클릭에서 호출
    {
        _trainingPanel.gameObject.SetActive(true);
    }

    public void OnTrainingCharacterBoxClick(Student target) //플레이어 박스 온클릭에서 호출
    {
        _trainingCommandPopUp.gameObject.SetActive(true);
        _trainingCommandPopUp.Init(target);
    }

    public void OpenPositionWarningPopUp(Student target) //TrainingBox 온클릭에서 호출. PositionWarningBox를 띄우는 역할.
    {
        _positionWarningPopUp.gameObject.SetActive(true);
        _positionWarningPopUp.Init(target);
    }

    public void OpenConditionWarningPopUp(List<Student> targets, int cost)
    {
        _conditionWarningPopUp.gameObject.SetActive(true);
        _conditionWarningPopUp.Init(targets, cost);
    }    

    public void OpenTrainingStartConfirmPopUp(int cost)
    {
        //_trainingStartConfirmPopUp.Init(cost);
        _conditionWarningPopUp.Init(cost);
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
