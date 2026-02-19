using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 역할 : 개별 캐릭터 박스에 부착하여 선수 한 명의 정보를 간략하게 표시. 버튼 클릭시 프로필 패널 활성화.
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

    private void OnDisable()
    {
        _selectButton.onClick.RemoveAllListeners();
    }
}
