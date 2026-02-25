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

            _boxList.Add(newBox);
        }
    }
}
