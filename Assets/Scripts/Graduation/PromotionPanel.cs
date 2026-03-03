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

    private List<int> _promotionStudentList;

    private bool _isSkillChoise = false;
    Student _currentStudent;

    public bool IsSkillChoise { get { return _isSkillChoise; } set { _isSkillChoise = value; } }

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

        int studentNum = _promotionStudentList[_graduationManager.Turn];

        for (int i = 0; i < _graduationManager.MyStudents.Count; i++)
        {
            if(studentNum == _graduationManager.MyStudents[i].StudentId)
            {
                _currentStudent = _graduationManager.MyStudents[i];
            }
        }
        Debug.Log($"순서: {_currentStudent.Name} 학생");

        if (_isSkillChoise == false)
        {
            _guideBoxName.text = $"{_currentStudent.Name} 학생이 진급 하였습니다.\r\n패시브 스킬을 선택해주세요!";
        }
        else if (_isSkillChoise == true)
        {
            _afterChoice.SetActive(true);
        }

        _name.text = _currentStudent.Name;
        _gradeUp.text = $"{_currentStudent.Grade}학년 → {_currentStudent.Grade + 1}학년";

        for (int i = 0; i < 3; i++)
        {
            if(i < _currentStudent.PassiveId.Count)
            {
                _passiveNameText[i].text = _currentStudent.PassiveId[i];
            }
            else
            {
                _passiveNameText[i].text = "";
            }
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
        _passiveBox.GetSkillList(_currentStudent);
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
