using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 육성 버튼을 누르면 나오는 TrainingPanel에 부착.
/// 선수 목록을 가져와 선수 수만큼 버튼 생성하는 역할
/// </summary>

public class TrainingPanel : MonoBehaviour
{
    [SerializeField] TrainingCharacterBox _trainingCharacterBoxPrefab;
    [SerializeField] Transform _trainingCharacterBoxParent;    

    GenericObjectPool<TrainingCharacterBox> _trainingBoxPool; //트레이닝 박스 오브젝트 풀

    List<TrainingCharacterBox> _boxList = new List<TrainingCharacterBox>(); //오브젝트 반납용 관리 리스트

    private void Awake()
    {
        _trainingBoxPool = new GenericObjectPool<TrainingCharacterBox>(_trainingCharacterBoxPrefab, _trainingCharacterBoxParent, 8, 20);
    }

    private void OnEnable()
    {
        RefreshPlayerList();
    }

    public void RefreshPlayerList()
    {        
        for (int i = 0; i < _boxList.Count; i++)
        {
            _trainingBoxPool.Release(_boxList[i]);
        }

        _boxList.Clear();
        
        var students = StudentManager.Instance.MyStudents;// 보유한 선수 수만큼 풀에서 가져와서 생성 (풀에 없으면 내장 오브젝트 풀이 자동 생성)
        for (int i = 0; i < students.Count; i++)
        {            
            TrainingCharacterBox newBox = _trainingBoxPool.Get(); //박스 채우기
            
            newBox.Init(students[i]); //박스에 선수 정보 주입            

            _boxList.Add(newBox);
        }
    }

    public void RefreshAllBoxesState()
    {
        for (int i = 0; i < _boxList.Count; i++)
        {
            if (_boxList[i] != null)
            {
                _boxList[i].SetStudentState();
            }
        }
    }
}
