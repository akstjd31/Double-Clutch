using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionWarningPopUp : MonoBehaviour
{
    [SerializeField] Transform _warningListParent;
    [SerializeField] GameObject _problemPrefab;
    [SerializeField] Button _confirmButton;
    [SerializeField] TextMeshProUGUI _cost;

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
        ClearWarningList();        
        foreach (Student target in students)
        {            
            if (target.Condition <= 0)
            {
                CreateWarning(target.Name);
            }
        }        
    }

    private void SetCostText(int cost)
    {
        _cost.text = $"ºñ¿ë : {cost}G";
    }

    private void CreateWarning(string name)
    {
        GameObject go = Instantiate(_problemPrefab, _warningListParent);        
        if (go.TryGetComponent(out Problem problem))
        {
            problem.Init(name);
        }
    }


    private void ClearWarningList()
    {
        foreach (Transform child in _warningListParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        _confirmButton.onClick.RemoveAllListeners();
    }
}
