using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRecruitBox : MonoBehaviour
{
    [SerializeField] Image _characterImage;
    [SerializeField] TextMeshProUGUI _positionText;
    [SerializeField] TextMeshProUGUI _gradeText;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _defenseText;    
    [SerializeField] Toggle _toggleButton;
    [SerializeField] Outline _outLine;

    private Student _student;
    private bool isSelected = false;
    public bool IsSelected => isSelected;

    private void Start()
    {
        _toggleButton.onValueChanged.AddListener(ChangeToggleState);
    }

    public void Init(Student target)
    {
        _student = target;
        SetText();
    }

    public Student GetStudent()
    {
        return _student;
    }

    private void SetText()
    {
        _positionText.text = _student.Position.ToString();
        _gradeText.text = _student.Grade.ToString() + "ÇÐ³â";
        _nameText.text = _student.Name;
        _attackText.text = _student.Attack.ToString();
        _defenseText.text = _student.Defense.ToString();
    }

    private void ChangeToggleState(bool isOn)
    {
        if (isOn)
        {
            isSelected = true;
            _outLine.enabled = true;
        }
        else
        {
            isSelected = false;
            _outLine.enabled = false;
        }
    }

    private void OnDestroy()
    {
        _toggleButton.onValueChanged?.RemoveAllListeners();
    }
}
