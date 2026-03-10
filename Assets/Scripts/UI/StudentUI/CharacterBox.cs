using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Button _selectButton;

    Student _target;

    public Student Target => _target;

    public void Init(Student student)
    {        
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(student.Name[0]) + manager.GetString(student.Name[1]) + manager.GetString(student.Name[2]);
        _nameText.text = name;                     
        _target = student;                
    }

    public Button GetSelectButton() => _selectButton;
}
