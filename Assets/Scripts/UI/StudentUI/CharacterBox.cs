using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterBox : MonoBehaviour
{
    [SerializeField] Image _studentImage;
    //[SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Button _selectButton;
    [SerializeField] Image _position;
    [SerializeField] Image _trait;

    Student _target;

    public Student Target => _target;

    public void Init(Student student)
    {        
        //StringManager manager = StringManager.Instance;
        //string name = manager.GetString(student.Name[0]) + manager.GetString(student.Name[1]) + manager.GetString(student.Name[2]);

        //_nameText.text = name;
        //manager.ApplyFont(_nameText);
        _target = student;

        SpriteManager spriteManager = SpriteManager.Instance;
        _studentImage.sprite = spriteManager.GetSprite(student.VisualData.playerImageResource);
        _position.sprite = spriteManager.GetPositionSprite(_target.Position);
        _trait.sprite = spriteManager.GetSprite(_target.TraitData.traitResource);
    }

    public Button GetSelectButton() => _selectButton;
}
