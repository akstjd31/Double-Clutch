using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeeklyTrainingReportPopUp : MonoBehaviour
{
    [SerializeField] CharacterRow _characterRowPrefab;
    [SerializeField] Transform _characterRowParent;

    GenericObjectPool<CharacterRow> _characterRowPool;
    List<CharacterRow> _characterRowList = new List<CharacterRow>();
    private void Awake()
    {
        _characterRowPool = new GenericObjectPool<CharacterRow>(_characterRowPrefab, _characterRowParent);
    }
    public void Init(List<Student> students)
    {
        Debug.Log("ResultPopUp Init!");
        RefreshCharacterRowList(students);
    }

    public void RefreshCharacterRowList(List<Student> students)
    {
        foreach (var box in _characterRowList) //기존에 사용하던 박스들을 풀로 반납
        {
            _characterRowPool.Release(box);
        }
        _characterRowList.Clear(); //리스트도 클리어
        
        for (int i = 0; i < students.Count; i++)
        {
            CharacterRow newRow = _characterRowPool.Get(); //박스 채우기            
            newRow.Init(students[i]); //박스에 선수 정보 주입            

            _characterRowList.Add(newRow);
            Debug.Log("CharacterRow Initiate!");
        }
    }
}
