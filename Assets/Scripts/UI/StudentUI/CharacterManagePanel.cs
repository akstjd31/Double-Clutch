using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManagePanel : MonoBehaviour
{
    [SerializeField] CharacterBox _characterBoxPrefab;
    [SerializeField] Transform _characterBoxParent;
    [SerializeField] GameObject _backButtonObj;    

    GenericObjectPool<CharacterBox> _characterBoxPool;

    List<CharacterBox> _boxList = new List<CharacterBox>();

    CharacterBox _currentActiveBox;

    private void Awake()
    {
        _characterBoxPool = new GenericObjectPool<CharacterBox>(_characterBoxPrefab, _characterBoxParent);
    }

    private void OnEnable()
    {
        RefreshPlayerList();
    }

    public void RefreshPlayerList()
    {
        foreach (var box in _boxList) //기존에 사용하던 박스들을 풀로 반납
        {
            _characterBoxPool.Release(box);
        }
        _boxList.Clear();

        var students = StudentManager.Instance.MyStudents;// 보유한 선수 수만큼 풀에서 가져와서 생성 (풀에 없으면 내장 오브젝트 풀이 자동 생성)
        for (int i = 0; i < students.Count; i++)
        {
            CharacterBox newBox = _characterBoxPool.Get(); //박스 채우기

            newBox.Init(students[i]); //박스에 선수 정보 주입

            var btn = newBox.GetSelectButton(); // 뒤로가기 버튼 설정
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { _backButtonObj.SetActive(false); });
            btn.onClick.AddListener(() => StudentUIManager.Instance.OnCharacterBoxClick(newBox));
            btn.onClick.AddListener(() => ChangeCurrentActiveBox(newBox));

            _boxList.Add(newBox);
        }
    }

    private void ChangeCurrentActiveBox(CharacterBox box)
    {
        _currentActiveBox = box;
    }

    public void OnNextButtonClick()
    {
        MoveSelection(1);
    }

    public void OnPrevButtonClick()
    {
        MoveSelection(-1);
    }

    private void MoveSelection(int direction)
    {
        if (_boxList.Count <= 1) return; // 박스가 하나 이하면 움직일 필요 없음

        // 1. 현재 박스의 인덱스 찾기
        int currentIndex = _boxList.IndexOf(_currentActiveBox);

        // 2. 다음 인덱스 계산 (리스트 범위를 벗어나지 않게 순환 처리)
        int nextIndex = currentIndex + direction;

        if (nextIndex >= _boxList.Count)
        {
            nextIndex = 0; // 마지막에서 다음 누르면 처음으로
        }
        if (nextIndex < 0)
        {
            nextIndex = _boxList.Count - 1; // 처음에서 이전 누르면 마지막으로
        }

        // 3. 대상 박스 가져오기
        CharacterBox targetBox = _boxList[nextIndex];

        // 4. 클릭했을 때와 동일한 로직 실행
        ExecuteBoxClick(targetBox);
    }
    private void ExecuteBoxClick(CharacterBox box)
    {
        // OnCharacterBoxClick의 로직을 그대로 수행
        _backButtonObj.SetActive(false); // 질문하신 코드의 리스너 로직 반영
        StudentUIManager.Instance.OnCharacterBoxClick(box);
        ChangeCurrentActiveBox(box);
    }
}
