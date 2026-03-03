using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PassiveSkillSelectPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;
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
        if (_graduationManager.PromotionPanel.IsSkillChoise == true)
        {
            gameObject.SetActive(false);
            _graduationManager.Turn++;

            if (_graduationManager.Turn < _graduationManager.PromotionStudentList.Count)
            {
                var student = _graduationManager.PromotionStudentList[_graduationManager.Turn];
                Debug.Log($"순서: {student.Name} 학생");
            }

            //_guideBox.SetActive(false);
            _afterGuideBox.SetActive(true);
            _graduationManager.PromotionPanel.IsSkillChoise = false;
        }
        //스킬 선택 안됐으면 팝업
        else
        {
            _warningBox.SetActive(true);
        }
    }
}
