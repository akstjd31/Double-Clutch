using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterOutPanel : MonoBehaviour
{    
    [SerializeField] Button _outConfirmButton;
    [SerializeField] CharacterRecruitBox _characterRecruitBoxPrefab;
    [SerializeField] Transform _characterBoxContainer;
    GenericObjectPool<CharacterRecruitBox> _characterRecruitBoxPool;
    List<CharacterRecruitBox> _boxList = new List<CharacterRecruitBox>();

    List<Student> _outList = new List<Student>();

    int _selectCount = 0;

    private void Awake()
    {
        _characterRecruitBoxPool = new GenericObjectPool<CharacterRecruitBox>(_characterRecruitBoxPrefab, _characterBoxContainer, 8, 12);
    }

    public void Init()
    {
        RefreshBoxList();

        _outConfirmButton.onClick.AddListener(ConfirmOut);
    }

    public void RefreshBoxList()
    {
        foreach(var box in _boxList)
        {
            _characterRecruitBoxPool.Release(box);
        }
        _boxList.Clear();

        for (int i = 0; i < StudentManager.Instance.MyStudents.Count; i++)
        {
            CharacterRecruitBox newBox = _characterRecruitBoxPool.Get();
            newBox.Init(StudentManager.Instance.MyStudents[i]);
            _boxList.Add(newBox);
        }
    }

    private void ConfirmOut()
    {
        _selectCount = 0;
        _outList.Clear();
        foreach (var box in _boxList) //방출 후 남을 학생 수 계산 및 방출 선택 학생 기억.
        {
            if (box.IsSelected)
            {
                _outList.Add(box.GetStudent());
                _selectCount++;
            }
        }
        int totalCount = StudentManager.Instance.MyStudents.Count - _selectCount;
        if (totalCount > StudentManager.Instance.RecruitLimit) //방출 진행 후에도 최대 보유치보다 많을 것으로 예상되면
        {
            //추가 방출 알림 팝업 호출
            StudentUIManager.Instance.OpenOutWarningPopUp(totalCount - StudentManager.Instance.RecruitLimit);
        }
        else if (totalCount < StudentManager.Instance.RecruitLimit)
        {
            //방출 불가 알림 팝업 호출
            StudentUIManager.Instance.OpenCantOutWarningPopUp();
        }
        else //최대보유치와 방출 후 학생 수가 딱 맞아야 방출 확인 팝업 호출
        {
            StudentUIManager.Instance.OpenOutConfirmPopUp(_selectCount);
        }

    }

    // 버튼 클릭 시 방출
    public void OnConfirmOutButtonClick()
    {
        foreach (var target in _outList)
        {
            StudentManager.Instance.ReleaseStudent(target);
        }
        StudentManager.Instance.SaveGame();
        this.gameObject.SetActive(false);

        CalendarManager.Instance.NextTurn();
    }

    private void OnDestroy()
    {
        _outConfirmButton?.onClick?.RemoveAllListeners();
    }
}
