using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PassiveSkillSelectPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;
    [SerializeField] private PromotionPanel _promotionPanel;
    [SerializeField] private PassiveBox _passiveBox;

    [SerializeField] private GameObject _guideBox;
    [SerializeField] private GameObject _afterGuideBox;
    [SerializeField] private GameObject _warningBox;

    //private List<Student> _promotionStudentList;

    private void Start()
    {
        //_promotionStudentList = _graduationManager.PromotionStudentList;
    }
    public void OnClickOKButton()
    {
        Student student = null;

        for (int i = 0; i < _graduationManager.MyStudents.Count; i++)
        {
            if (_graduationManager.MyStudents[i].StudentId ==
                _graduationManager.PromotionStudentList[_graduationManager.Turn])
            {
                student = _graduationManager.MyStudents[i];
                break;
            }
        }

        if (_graduationManager.PromotionPanel.IsSkillChoise == true)
        {
            gameObject.SetActive(false);

            //선택한 스킬 추가
            student.SetPassive(_passiveBox.SelectSkill);
            _graduationManager.PromotionPanel.UpdateProfile();

            _graduationManager.Turn++;

            if (_graduationManager.Turn < _graduationManager.PromotionStudentList.Count)
            {
                var NextstudentID = _graduationManager.PromotionStudentList[_graduationManager.Turn];

                for (int i = 0; i < _graduationManager.MyStudents.Count; i++)
                {
                    if (NextstudentID == _graduationManager.MyStudents[i].StudentId)
                    {
                        Student _currentStudent = _graduationManager.MyStudents[i];
                        Debug.Log($"순서: {_currentStudent.Name} 학생");
                    }
                }
            }

            //_guideBox.SetActive(false);
            _afterGuideBox.SetActive(true);
            _graduationManager.PromotionPanel.IsSkillChoise = false;
        }
        //스킬 선택 안됐으면 팝업
        else if(_graduationManager.PromotionPanel.IsSkillChoise == false)
        {
            _warningBox.SetActive(true);
        }
    }
}
