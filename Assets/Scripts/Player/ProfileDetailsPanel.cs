using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 역할 : 선수 프로필 패널에 부착하여 정보 텍스트 표시
/// </summary>
public class ProfileDetailsPanel : MonoBehaviour
{
    [SerializeField] Text _nameText;
    [SerializeField] Text _gradeText;
    
    [SerializeField] Text _attackText;
    [SerializeField] Text _defenseText;
    [SerializeField] Text _personalityText;
    [SerializeField] Text _traitText;

    [SerializeField] PassiveProfileBox _profileBox0;
    [SerializeField] PassiveProfileBox _profileBox1;
    [SerializeField] PassiveProfileBox _profileBox2;

    public void Init(Student student)
    {
        _nameText.text = student.Name;
        _gradeText.text = student.Grade.ToString() + "학년";
        _attackText.text = student.Attack.ToString();
        _defenseText.text = student.Defense.ToString();
        _personalityText.text = StringManager.Instance.GetString(student.PersonalityData.personalityName);
        _traitText.text = StringManager.Instance.GetString(student.TraitData.traitName);
        SetPassiveText(student);
    }

    private void SetPassiveText(Student student)
    {
        _profileBox0.Init(student.Passive[0]);        
        
        if (student.Grade >= 2)
        {
            _profileBox1.Init(student.Passive[1]);
        }

        if (student.Grade == 3)
        {
            _profileBox2.Init(student.Passive[2]);
        }
    }

}
