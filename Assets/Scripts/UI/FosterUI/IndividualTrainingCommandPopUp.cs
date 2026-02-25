using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TrainingComamndPopUp에 부착하기
/// 개인 훈련 목록 전부 생성하기
/// </summary>
public class IndividualTrainingCommandPopUp : MonoBehaviour
{
    [SerializeField] Transform _trainingListParent;
    [SerializeField] GameObject _trainingBoxPrefab;

    Student _selectedStudent;

    private List<TrainingBox> _boxes = new List<TrainingBox>();

    private void Start() //박스는 일단 전부 만들기
    {
        MakeTrainingList();
    }

    public void Init(Student student) //누구의 훈련인지 설정
    {        
        _selectedStudent = student;

        foreach(var box in _boxes)
        {
            box.SetStudent(_selectedStudent);
        }
    }

    private void MakeTrainingList()
    {
        foreach (var box in _boxes) //기존 박스 목록 리셋
        {
            Destroy(box.gameObject); 
        }
        _boxes.Clear();
        

        var trainingDB = FosterManager.Instance.IndividualTrainingDB.DataList;
        foreach (var data in trainingDB)
        {
            CreateBox(new IndividualTraining(data));
        }
        var restDB = FosterManager.Instance.IndividualRestDB.DataList;
        foreach (var data in restDB)
        {
            CreateBox(new IndividualRest(data));
        }

    }
    private void CreateBox(ITraining command)
    {
        GameObject go = Instantiate(_trainingBoxPrefab, _trainingListParent);
        TrainingBox box = go.GetComponent<TrainingBox>();

        if (box != null)
        {
            box.SetStudent(_selectedStudent);
            box.Init(command);            
        }

        _boxes.Add(box);
    }
}