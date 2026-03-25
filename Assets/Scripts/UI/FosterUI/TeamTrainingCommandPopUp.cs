using System.Collections.Generic;
using UnityEngine;

public class TeamTrainingCommandPopUp : MonoBehaviour
{
    [SerializeField] Transform _trainingListParent; // �Ʒ� ���� ��ư�� ���� �θ� ������Ʈ ��ġ
    [SerializeField] TrainingBox _trainingBoxPrefab; // �Ʒ� ���� ��ư ������

    GenericObjectPool<TrainingBox> _teamTrainingPool;
    private List<TrainingBox> _boxList = new List<TrainingBox>();
    private TrainingPanel _trainingPanel;

    private void Awake()
    {
        _teamTrainingPool = new GenericObjectPool<TrainingBox>(_trainingBoxPrefab, _trainingListParent, 4, 6);
        _trainingPanel = GameObject.FindAnyObjectByType<TrainingPanel>();
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init() //�� �Ʒ��� ������ ��ü �����̹Ƿ� Ÿ�� ���� x
    {
        RefreshTrainingList();
    }

    public void RefreshTrainingList()
    {
        foreach (var box in _boxList) //���� �ڽ� ���? ����
        {
            _teamTrainingPool.Release(box);
        }
        _boxList.Clear();


        var trainingDB = FosterManager.Instance.Team_TrainingDB.DataList;
        for (int i = 0; i < trainingDB.Count; i++)
        {
            CreateBox(new TeamTraining(trainingDB[i]));
        }
        

        var restDB = FosterManager.Instance.Team_RestDB.DataList;        
        for (int i = 0; i < restDB.Count; i++)
        {
            CreateBox(new Team_Rest(restDB[i]));
        }

    }
    private void CreateBox(ITraining command)
    {        
        TrainingBox box = _teamTrainingPool.Get();
        box.transform.SetAsLastSibling();
        box.Init(command, _trainingPanel);        

        _boxList.Add(box);
    }
    
}
