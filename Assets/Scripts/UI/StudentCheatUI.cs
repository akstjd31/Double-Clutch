using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentCheatUI : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _scrollViewObj;
    [SerializeField] private GameObject _studentButtonObjPrefab;
    [SerializeField] private GameObject _statPanelObj;

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

            newObj.GetComponentInChildren<TextMeshProUGUI>().text = stdName;
            newObj.GetComponent<Button>().onClick.AddListener(OnStudentButtonClick);
        }
    }

    public void OnStudentButtonClick()
    {
        _scrollViewObj.SetActive(false);
        _statPanelObj.SetActive(true);
    }
}
