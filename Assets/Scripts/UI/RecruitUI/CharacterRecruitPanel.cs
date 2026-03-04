using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRecruitPanel : MonoBehaviour
{
    [SerializeField] CharacterRecruitBox[] characterRecruitBoxList = new CharacterRecruitBox[5];
    [SerializeField] Button _recruitConfirmButton; //영입하기 버튼
    List<Student> _selectedStudents = new List<Student>(); // 영입 선택된 선수 목록
    int _selectCount = 0;

    private void OnEnable()
    {
        Init();
    }

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
        _selectedStudents.Clear();
        foreach (var box in characterRecruitBoxList)//선택된 영입후보 수 계산
        {
            if (box.IsSelected)
            {
                _selectedStudents.Add(box.GetStudent());
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
        foreach (var student in _selectedStudents)
        {
            StudentManager.Instance.RecruitNewStudent(student);//일단 모두 영입
        }
        if (!StudentManager.Instance.IsStable)
        {
            //방출 창 팝업 호출
            StudentUIManager.Instance.OpenCharacterOutPanel();
        }

        this.gameObject.SetActive(false);
    }
    
}
