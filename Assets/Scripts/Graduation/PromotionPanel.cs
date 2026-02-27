using TMPro;
using UnityEngine;

public class PromotionPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;

    [SerializeField] private TextMeshProUGUI _guideBoxName;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _gradeUp;

    [SerializeField] private TextMeshProUGUI[] _passiveNameText = new TextMeshProUGUI[3];

    private void OnEnable()
    {
        for (int i = 0; i < _passiveNameText.Length; i++)
        {
            _passiveNameText[i].text = "";
        }
    }
    public void Profile(Student Student)
    {
        _guideBoxName.text = $"{Student.Name} 학생이 진급 하였습니다.\r\n패시브 스킬을 선택해주세요!";
        _name.text = Student.Name;
        _gradeUp.text = $"{Student.Grade} → {Student.Grade + 1}";

        for(int i = 0; i < Student.PassiveId.Count; i++)
        {
            _passiveNameText[i].text = Student.PassiveId[i];
        }
    }

}
