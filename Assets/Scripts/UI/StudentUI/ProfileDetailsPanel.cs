using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileDetailsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _gradeText;
    [SerializeField] TMP_Dropdown _positionDropdown;
    [SerializeField] Slider _conditionSlider;

    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _defenseText;
    [SerializeField] TextMeshProUGUI _personalityText;
    [SerializeField] TextMeshProUGUI _traitText;

    [SerializeField] PassiveProfileBox _profileBox0;
    [SerializeField] PassiveProfileBox _profileBox1;
    [SerializeField] PassiveProfileBox _profileBox2;

    Student _student;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    public void Init(Student student)
    {
        Debug.Log("Profile Details Panel Init!");
        _student = student;
        _positionDropdown.value = PositionIntoValue(student.Position);
        _nameText.text = student.Name;
        _gradeText.text = student.Grade.ToString() + "학년";
        _attackText.text = student.Attack.ToString();
        _defenseText.text = student.Defense.ToString();
        _conditionSlider.value = NormalizeConditionValue(student.Condition);
        SetPassiveText(student);
        Refresh();
    }

    private void Refresh()
    {
        if (_student == null) return;
        _personalityText.text = StringManager.Instance.GetString(_student.PersonalityData.personalityName);
        _traitText.text = StringManager.Instance.GetString(_student.TraitData.traitName);
    }

    private void SetPassiveText(Student student)
    {
        _profileBox0.Init(student.Passive[0]);
        _profileBox1.Init();
        _profileBox2.Init();

        if (student.Grade >= 2)
        {
            _profileBox1.Init(student.Passive[1]);
        }

        if (student.Grade == 3)
        {
            _profileBox2.Init(student.Passive[2]);
        }
    }

    public void OnPositionChanged(int value)
    {
        _student.SetPosition(ValueIntoPosition(value));
    }

    private int PositionIntoValue(Position position)
    {
        switch (position)
        {
            case Position.C: return 0;
            case Position.SF: return 1;
            case Position.PF: return 2;
            case Position.SG: return 3;
            case Position.PG: return 4;
            default: return 0;
        }
    }

    private Position ValueIntoPosition(int value)
    {
        switch (value)
        {
            case 0: return Position.C;
            case 1: return Position.SF;
            case 2: return Position.PF;
            case 3: return Position.SG;
            case 4: return Position.PG;
            default: return Position.C;
        }
    }

    public float NormalizeConditionValue(int condition)
    {
        return (float)condition / 100;
    }
}
