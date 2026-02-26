using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConditionWarningPopUp : MonoBehaviour
{
    [SerializeField] Transform _warningListParent;
    [SerializeField] Problem _problemPrefab;
    [SerializeField] Button _confirmButton;
    [SerializeField] TextMeshProUGUI _cost;

    GenericObjectPool<Problem> _problemPool;
    List<Problem> _problemList = new List<Problem>();

    private void Awake()
    {
        _problemPool = new GenericObjectPool<Problem>(_problemPrefab, _warningListParent);
    }

    private void Start()
    {
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(() => FosterManager.Instance.StartFoster());
    }

    public void Init(List<Student> targets, int cost)
    {        
        MakeWarningList(targets);
        SetCostText(cost);
    }

    public void Init(int cost)
    {
        SetCostText(cost);
    }


    private void MakeWarningList(List<Student> students)
    {
        foreach (var box in _problemList) //기존에 사용하던 경고창들을 풀로 반납
        {
            _problemPool.Release(box);
        }
        _problemList.Clear();

        foreach (Student target in students) //넘겨받은 학생 중 문제 컨디션 0 다시 체크 후 경고문구 생성
        {
            if (target.State != StudentState.None) //부상, 과로 학생용(팀 훈련 시)
            {
                CreateWarning(target.Name, target.State);
            }
            else if (target.Condition <= 0) //컨디션 0 학생(범용)
            {
                CreateWarning(target.Name);
            }
        }        
    }

    private void SetCostText(int cost)
    {
        _cost.text = $"비용 : {cost}G";
    }

    private void CreateWarning(string name)
    {
        Problem problem = _problemPool.Get();
        problem.Init(name);
    }
    private void CreateWarning(string name, StudentState state)
    {
        Problem problem = _problemPool.Get();
        problem.Init(name, state);
    }

    private void OnDestroy()
    {
        _confirmButton.onClick.RemoveAllListeners();
    }
}
