using UnityEngine;
using UnityEngine.UI;

public class CharacterRecruitPanel : MonoBehaviour
{
    [SerializeField] CharacterRecruitBox[] characterRecruitBoxList = new CharacterRecruitBox[5];
    [SerializeField] Button _recruitConfirmButton;

    int _selectCount = 0;    

    public void Init()
    {
        for (int i = 0; i < characterRecruitBoxList.Length; i++)
        {
            characterRecruitBoxList[i].Init(StudentManager.Instance.MakeRandomStudent());
        }
        _recruitConfirmButton.onClick.AddListener(ConfirmRecruit);
    }

    private void ConfirmRecruit() //영입하기 버튼 온클릭에서 호출
    {
        _selectCount = 0;
        for (int i = 0; i < characterRecruitBoxList.Length; i++) //선택된 영입후보 수 계산
        {            
            if (characterRecruitBoxList[i].IsSelected)
            {
                _selectCount++;                
            }
        }

        int totalCount = StudentManager.Instance.MyStudents.Count + _selectCount;

        if (totalCount < StudentManager.Instance.RecruitLimit) //영입 후 선수 수(현재 선수 + 선택한 영입 후보)가 최대 선수 보유치에 못미칠 것으로 예상되면 추가영입 경고 팝업 호출
        {
            StudentUIManager.Instance.OpenRecruitWarningPopUp(StudentManager.Instance.RecruitLimit - totalCount);
        }
        else //영입 후 선수(현재 선수 + 선택한 영입 후보)가 최대 보유치 이상이면 영입 확인 팝업 호출
        {
            StudentUIManager.Instance.OpenRecruitConfirmPopUp(_selectCount);
        }
    }

    public void OnConfirmButtonClick() //*중요* 영입 확인 팝업의 확인 버튼에서는 UI매니저 말고 여기를 호출
    {
        for (int i = 0; i < characterRecruitBoxList.Length; i++)
        {
            if (characterRecruitBoxList[i].IsSelected)
            {
                StudentManager.Instance.RecruitNewStudent(characterRecruitBoxList[i].GetStudent()); //일단 모두 영입
            }
        }

        if (!StudentManager.Instance.IsStable)
        {
            //방출 창 팝업 호출
        }
    }

    
}
