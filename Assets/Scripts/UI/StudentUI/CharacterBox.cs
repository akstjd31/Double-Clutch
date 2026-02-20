using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ���� : ���� ĳ���� �ڽ��� �����Ͽ� ���� �� ���� ������ �����ϰ� ǥ��. ��ư Ŭ���� ������ �г� Ȱ��ȭ.
/// </summary>
public class CharacterBox : MonoBehaviour
{
    [SerializeField] Text _nameText;
    [SerializeField] Text _gradeText;
    [SerializeField] Button _selectButton;

    public void Init(Student student)
    {        
        _nameText.text = student.Name;
        _gradeText.text = student.Grade.ToString();        
        _selectButton.onClick?.RemoveAllListeners();
        Student target = student;
        _selectButton.onClick.AddListener(() => StudentUIManager.Instance.OnCharacterBoxClick(target));
    }

    private void OnDestroy()
    {
        _selectButton.onClick.RemoveAllListeners();
    }

    public Button GetSelectButton() => _selectButton;
}
