using TMPro;
using UnityEngine;

public class CharacterProfilePopUp : MonoBehaviour
{
    [SerializeField] CharacterBox _characterBox;
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
        _attack.text = _student.Attack.ToString();
        _defense.text = _student.Defense.ToString();
        _personality.text = StringManager.Instance.GetString(_student.PersonalityData.personalityName);
        _passive1.Init(_student.Passive[0]);
        _passive2.Init();
        _passive3.Init();

        if (_student.Passive.Count >= 2)
        {
            _passive2.Init(_student.Passive[1]);
        }
        if (_student.Passive.Count == 3)
        {
            _passive2.Init(_student.Passive[2]);
        }
    }
}
