using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentCheatUI : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _scrollViewObj;
    [SerializeField] private GameObject _studentButtonObjPrefab;
    [SerializeField] private GameObject _statPanelObj;
    [SerializeField] private List<Button> _stdButtonList;
    

    private void OnEnable()
    {
        var stdMgr = StudentManager.Instance;
        if (stdMgr == null) return;
        if (_studentButtonObjPrefab == null) return;

        for (int i = 0; i < stdMgr.MyStudents.Count; i++)
        {
            var newObj = Instantiate(_studentButtonObjPrefab, _content);
            var stdName = StringManager.Instance.GetString(stdMgr.MyStudents[i].Name[0]) + 
                            StringManager.Instance.GetString(stdMgr.MyStudents[i].Name[1]) +
                            StringManager.Instance.GetString(stdMgr.MyStudents[i].Name[2]);

            int index = i;
            newObj.GetComponentInChildren<TextMeshProUGUI>().text = stdName;

            var btn = newObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnStudentButtonClick(stdMgr.MyStudents[index]));

            _stdButtonList.Add(btn);
        }
    }

    private void OnDisable()
    {
        if (_stdButtonList == null || _stdButtonList.Count < 1) return;
        
        foreach (var btn in _stdButtonList)
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    public void OnStudentButtonClick(Student std)
    {
        _scrollViewObj.SetActive(false);
        _statPanelObj.SetActive(true);

        if (_statPanelObj.TryGetComponent<DetailCheatUI>(out var statCheat))
        {
            statCheat.SetStudent(std);
        }
    }
}
