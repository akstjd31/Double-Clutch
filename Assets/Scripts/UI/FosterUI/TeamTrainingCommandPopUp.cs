using System.Collections.Generic;
using UnityEngine;

public class TeamTrainingCommandPopUp : MonoBehaviour
{
    [SerializeField] Transform _trainingListParent; // 훈련 설정 버튼이 놓일 부모 오브젝트 위치
    [SerializeField] TrainingBox _trainingBoxPrefab; // 훈련 설정 버튼 프리팹

    GenericObjectPool<TrainingBox> _teamTrainingPool;
    private List<TrainingBox> _boxList = new List<TrainingBox>();

    private void Awake()
    {
        _teamTrainingPool = new GenericObjectPool<TrainingBox>(_trainingBoxPrefab, _trainingListParent, 4, 6);
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init() //팀 훈련은 어차피 전체 적용이므로 타겟 세팅 x
    {
        RefreshTrainingList();
    }

    public void RefreshTrainingList()
    {
        foreach (var box in _boxList) //기존 박스 목록 리셋
        {
            _teamTrainingPool.Release(box);
        }
        _boxList.Clear();


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
        TrainingBox box = _teamTrainingPool.Get();
        box.Init(command);        

        _boxList.Add(box);
    }
    
}
