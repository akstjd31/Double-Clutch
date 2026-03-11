using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IndividualTrainingCommandPopUp : MonoBehaviour
{
    [SerializeField] Transform _trainingListParent;
    [SerializeField] TrainingBox _trainingBoxPrefab;
    [SerializeField] TextMeshProUGUI _nameText;

    private GenericObjectPool<TrainingBox> _pool;
    private List<TrainingBox> _boxList = new List<TrainingBox>();
    private Student _selectedStudent;

    private void Awake()
    {        
        _pool = new GenericObjectPool<TrainingBox>(_trainingBoxPrefab, _trainingListParent);
    }
  
    public void Init(Student student)
    {
        _selectedStudent = student;

        // 기존 사용하던 박스 반납
        foreach (var box in _boxList)
        {
            _pool.Release(box);
        }
        _boxList.Clear();

        // 개인 훈련 데이터 생성 및 배치
        var trainingDB = FosterManager.Instance.IndividualTrainingDB.DataList;
        for (int i = 0; i < trainingDB.Count; i++)
        {
            CreateBox(new IndividualTraining(trainingDB[i]));
        }

        // 개인 휴식 데이터 생성 및 배치
        var restDB = FosterManager.Instance.IndividualRestDB.DataList;
        for (int i = 0; i < restDB.Count; i++)
        {
            CreateBox(new IndividualRest(restDB[i]));
        }

        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_selectedStudent.Name[0]) + manager.GetString(_selectedStudent.Name[1]) + manager.GetString(_selectedStudent.Name[2]);
        _nameText.text = name + " 육성 커맨드";
    }

    private void CreateBox(ITraining command)
    {
        // 풀에서 박스 생성
        TrainingBox box = _pool.Get();
        box.transform.SetAsLastSibling();

        // 데이터 주입 (학생 설정 후 Init 호출)
        box.Init(command);
        box.SetStudent(_selectedStudent);        

        _boxList.Add(box);
    }
        
    //선수 개인 훈련 시 포지션 변경 버튼에 각각 연결
    public void OnCClick() => ChangePosition(Position.C);
    public void OnPFClick() => ChangePosition(Position.PF);
    public void OnPGClick() => ChangePosition(Position.PG);
    public void OnSFClick() => ChangePosition(Position.SF);
    public void OnSGClick() => ChangePosition(Position.SG);
    private void ChangePosition(Position position)
    {
        _selectedStudent.SetPosition(position);
    }
}