using UnityEngine;
using UnityEngine.UI;

public class CharacterOutPanel : MonoBehaviour
{
    [SerializeField] CharacterRecruitBox[] characterRecruitBoxList = new CharacterRecruitBox[5];
    [SerializeField] Button _outConfirmButton;

    int _selectCount = 0;

    public void Init()
    {
        for (int i = 0; i < characterRecruitBoxList.Length; i++)
        {
            characterRecruitBoxList[i].Init(StudentManager.Instance.MakeRandomStudent());
        }
        //_outConfirmButton.onClick.AddListener(ConfirmRecruit);
    }
}
