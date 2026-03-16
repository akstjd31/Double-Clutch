using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterBox : MonoBehaviour
{
    [SerializeField] Image _studentImage;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Button _selectButton;
    [SerializeField] TextMeshProUGUI _position;
    [SerializeField] TextMeshProUGUI _state;

    Student _target;

    public Student Target => _target;

    public void Init(Student student)
    {        
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(student.Name[0]) + manager.GetString(student.Name[1]) + manager.GetString(student.Name[2]);
        _nameText.text = name;                     
        _target = student;
        _studentImage.sprite = SpriteManager.Instance.GetSprite(student.VisualData.playerImageResource);
        _position.text = student.Position.ToString();
        _state.text = student.Condition.ToString();
    }

    public Button GetSelectButton() => _selectButton;
}
