using TMPro;
using UnityEngine;

public class CharacterProfilePopUp : MonoBehaviour
{
    [SerializeField] CharacterBox _characterBox;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _grade;
    [SerializeField] TextMeshProUGUI _attack;
    [SerializeField] TextMeshProUGUI _defense;
    [SerializeField] TextMeshProUGUI _personality;
    [SerializeField] CharacterPassiveProfileRow _passive1;
    [SerializeField] CharacterPassiveProfileRow _passive2;
    [SerializeField] CharacterPassiveProfileRow _passive3;

    Student _student;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += SetText;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= SetText;
    }

    public Student GetStudent() => _student;

    public void Init(Student target)
    {
        _student = target;
        SetText();
    }

    public void SetText()
    {
        if (_student == null) return;
        _characterBox.Init(_student);

        SetName(_student);
        _attack.text = _student.Attack.ToString();
        StringManager.Instance.ApplyFont(_attack);
        _defense.text = _student.Defense.ToString();
        StringManager.Instance.ApplyFont(_defense);
        StringManager.Instance.GetString(_student.PersonalityData.personalityName, _personality);
        StringManager.Instance.ApplyFont(_personality);
        _passive1.Init(_student.Passive[0]);
        _passive2.Init();
        _passive3.Init();

        if (_student.Passive.Count >= 2)
        {
            _passive2.Init(_student.Passive[1]);
        }
        if (_student.Passive.Count == 3)
        {
            _passive3.Init(_student.Passive[2]);
        }
        SetGrade(_student.Grade);
        StringManager.Instance.ApplyFont(_grade);
    }

    public void SetName(Student student)
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(student.Name[0]) + manager.GetString(student.Name[1]) + manager.GetString(student.Name[2]);

        _name.text = name;      
        StringManager.Instance.ApplyFont(_name);
    }

    public void SetGrade(int grade)
    {
        switch (grade)
        {
            case 1: _grade.text = StringManager.Instance.GetString("UI_Player_1학년"); break;
            case 2: _grade.text = StringManager.Instance.GetString("UI_Player_2학년"); break;
            case 3: _grade.text = StringManager.Instance.GetString("UI_Player_3학년"); break;
        }
    }
}
