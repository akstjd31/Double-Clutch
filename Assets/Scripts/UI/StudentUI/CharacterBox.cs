using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _gradeText;
    [SerializeField] Button _selectButton;

    Student _target;

    public Student Target => _target;

    public void Init(Student student)
    {        
        _nameText.text = student.Name;        
        _gradeText.text = student.Grade.ToString();                
        _target = student;                
    }

    public Button GetSelectButton() => _selectButton;
}
