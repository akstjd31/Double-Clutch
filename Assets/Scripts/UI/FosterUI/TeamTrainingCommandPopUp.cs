using System.Collections.Generic;
using UnityEngine;

public class TeamTrainingCommandPopUp : MonoBehaviour
{
    [SerializeField] Transform _trainingListParent; // 훈련 설정 버튼이 놓일 부모 오브젝트 위치
    [SerializeField] GameObject _trainingBoxPrefab; // 훈련 설정 버튼 프리팹

    Student _selectedStudent;

    private List<TrainingBox> _boxes = new List<TrainingBox>();

    private void Start() //박스는 일단 전부 만들기
    {
        MakeTrainingList();
    }

    public void Init(Student student) //누구의 훈련인지 설정
    {
        _selectedStudent = student;

        foreach (var box in _boxes)
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


        var trainingDB = FosterManager.Instance.Team_TrainingDB.DataList;
        foreach (var data in trainingDB)
        {
            CreateBox(new TeamTraining(data));
        }
        var restDB = FosterManager.Instance.Team_RestDB.DataList;
        foreach (var data in restDB)
        {
            CreateBox(new Team_Rest(data));
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
    public void OnPositionChangeButtonClick(Position position)
    {
        _selectedStudent.SetPosition(position);
    }
}
