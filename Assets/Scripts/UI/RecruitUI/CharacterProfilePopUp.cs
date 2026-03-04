using TMPro;
using UnityEngine;

public class CharacterProfilePopUp : MonoBehaviour
{
    [SerializeField] CharacterBox _characterBox;
    [SerializeField] TextMeshProUGUI _attack;
    [SerializeField] TextMeshProUGUI _defense;
    [SerializeField] TextMeshProUGUI _personality;
    [SerializeField] TextMeshProUGUI _passive1;
    [SerializeField] TextMeshProUGUI _passive2;
    [SerializeField] TextMeshProUGUI _passive3;

    Student _student;

    public Student GetStudent()
    {
        return _student;
    }

    public void Init(Student target)
    {
        _student = target;
        SetText();
    }

    public void SetText()
    {
        _characterBox.Init(_student);
        _attack.text = _student.Attack.ToString();
        _defense.text = _student.Defense.ToString();
        _personality.text = StringManager.Instance.GetString(_student.PersonalityData.personalityName);
        _passive1.text = StringManager.Instance.GetString(_student.Passive[0].skillName);
        _passive2.text = "비어 있음";
        _passive3.text = "비어 있음";

        if (_student.Passive.Count >= 2)
        {
            _passive2.text = StringManager.Instance.GetString(_student.Passive[1].skillName);
        }
        if (_student.Passive.Count == 3)
        {
            _passive2.text = StringManager.Instance.GetString(_student.Passive[2].skillName);
        }
    }
}
