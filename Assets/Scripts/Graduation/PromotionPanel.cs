using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromotionPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;
    [SerializeField] private GameObject _passiveSkillSelectPanel;
    [SerializeField] private PassiveBox _passiveBox;

    [SerializeField] private GameObject _beforeGuideBox;
    [SerializeField] private GameObject _afterChoice;

    [SerializeField] private TextMeshProUGUI _guideBoxName;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _gradeUp;

    [SerializeField] private TextMeshProUGUI[] _passiveNameText = new TextMeshProUGUI[3];

    private List<Student> _promotionStudentList;

    private bool _isSkillChoise = false;

    public bool IsSkillChoise { get { return _isSkillChoise; } set { _isSkillChoise = value; } }

    private void OnEnable()
    {
        for (int i = 0; i < _passiveNameText.Length; i++)
        {
            _passiveNameText[i].text = "";
        }
    }

    public void GetList()
    {
        _promotionStudentList = _graduationManager.PromotionStudentList;
    }

    public void UpdateProfile()
    {
        if (_graduationManager.Turn >= _promotionStudentList.Count)
        {
            return;
        }
        var student = _promotionStudentList[_graduationManager.Turn];
        Debug.Log($"순서: {student.Name} 학생");

        if (_isSkillChoise == false)
        {
            _guideBoxName.text = $"{student.Name} 학생이 진급 하였습니다.\r\n패시브 스킬을 선택해주세요!";
        }
        else if (_isSkillChoise == true)
        {
            _afterChoice.SetActive(true);
        }

        _name.text = student.Name;
        _gradeUp.text = $"{student.Grade}학년 → {student.Grade + 1}학년";

        for (int i = 0; i < student.PassiveId.Count; i++)
        {
            _passiveNameText[i].text = student.PassiveId[i];
        }

        _isSkillChoise = false;
        Debug.Log($"스킬 선택 상태{_isSkillChoise}/{IsSkillChoise}");
    }

    public void OnClickNextButton()
    {
        if (_promotionStudentList.Count == 0)
        {
            Debug.Log("진급 학생 없음");
            _graduationManager.NextScene();
            return;
        }

        Debug.Log($"{_graduationManager.Turn}=={_promotionStudentList.Count}");
        if (_graduationManager.Turn == _promotionStudentList.Count)
        {
            Debug.Log("메인 씬으로 넘어가야 함.");
            _graduationManager.NextScene();
            return;
        }

        var student = _promotionStudentList[_graduationManager.Turn];
        _passiveSkillSelectPanel.SetActive(true);
        _passiveBox.GetSkill(student);
        //스킬 선택 상태 초기화
        _isSkillChoise = false;
        Debug.Log($"스킬 선택 상태{_isSkillChoise}/{IsSkillChoise}");

        Debug.Log($"남은 학생 수 {_graduationManager.Turn + 1}/{_promotionStudentList.Count}");

    }

    public void OnClickAfterChoice()
    {
        _afterChoice.SetActive(false);
    }

    public void OnClickNextStudent()
    {
        UpdateProfile();
        _afterChoice.SetActive(false);

        if (_graduationManager.Turn == _promotionStudentList.Count)
        {
            _beforeGuideBox.SetActive(false);
        }

    }
}
