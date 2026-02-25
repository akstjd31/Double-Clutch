using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ���� : ���� ������ �гο� �����Ͽ� ���� �ؽ�Ʈ ǥ��
/// </summary>
public class ProfileDetailsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _gradeText;
    
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _defenseText;
    [SerializeField] TextMeshProUGUI _personalityText;
    [SerializeField] TextMeshProUGUI _traitText;

    [SerializeField] PassiveProfileBox _profileBox0;
    [SerializeField] PassiveProfileBox _profileBox1;
    [SerializeField] PassiveProfileBox _profileBox2;

    public void Init(Student student)
    {
        _nameText.text = student.Name;
        _gradeText.text = student.Grade.ToString() + "�г�";
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
