using System.Collections.Generic;
using TMPro;
using UnityEngine;




public class GraduationListPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;

    [SerializeField] private TextMeshProUGUI _graduationNumber;
    [SerializeField] private TextMeshProUGUI _totalHonorText;

    [SerializeField] private GameObject _nameBoxPrefab;
    [SerializeField] private GameObject _characterProfileDetail;
    [SerializeField] private GameObject _listObject;


    List<Student> _myStudents;
    List<Student> _graduationStudentList;
    List<GameObject> _graduationNameBoxList = new List<GameObject>();

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
        

        //플레이어 보유 학생 리스트 가져오기
        //_myStudents = StudentManager.Instance.MyStudents;
        
        //GuideText();
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    private void Start()
    {
        _myStudents = _graduationManager.MyStudents;
        _graduationStudentList = _graduationManager.GraduationStudentList;
        CreatCell();
        GuideText();
    }

    private void CreatCell()
    {
        StringManager manager = StringManager.Instance;
        _graduationNameBoxList.Clear();
        for (int i = 0; i < _graduationStudentList.Count; i++)
        {
            GameObject nameBox = Instantiate(_nameBoxPrefab, _listObject.transform);
            var name = nameBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var honor = nameBox.transform.Find("Honor").GetComponent<TextMeshProUGUI>();
            var script = nameBox.GetComponent<GraduationNameBox>();
            _graduationNameBoxList.Add(nameBox);
            //var color = nameBox.GetComponent<Color>();
            //버튼을 생성할 때 학생 데이터 저장
            script.clickData = _graduationStudentList[i];
            script.graduationUI = _characterProfileDetail;
            name.text = manager.GetString(_graduationStudentList[i].Name[0]) + manager.GetString(_graduationStudentList[i].Name[1]) + manager.GetString(_graduationStudentList[i].Name[2]);
            honor.text = _graduationStudentList[i].TotalFame.ToString();

            manager.ApplyFont(name);                        
        }
    }

    private void RefreshCell()
    {
        StringManager manager = StringManager.Instance;
        for (int i = 0; i < _graduationNameBoxList.Count; i++)
        {
            GameObject nameBox = _graduationNameBoxList[i];
            var name = nameBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var honor = nameBox.transform.Find("Honor").GetComponent<TextMeshProUGUI>();
            var script = nameBox.GetComponent<GraduationNameBox>();
            //var color = nameBox.GetComponent<Color>();
            //버튼을 생성할 때 학생 데이터 저장
            script.clickData = _graduationStudentList[i];
            script.graduationUI = _characterProfileDetail;
            name.text = manager.GetString(_graduationStudentList[i].Name[0]) + manager.GetString(_graduationStudentList[i].Name[1]) + manager.GetString(_graduationStudentList[i].Name[2]);
            honor.text = _graduationStudentList[i].TotalFame.ToString();

            manager.ApplyFont(name);
        }
    }

    private void GuideText()
    {
        Debug.Log($"졸업자 수: {_graduationStudentList.Count}");
        Debug.Log($"명예 총 수: {_graduationManager.TotalHonor}");
        string getGraduationNum = StringManager.Instance.GetString("UI_Graduation_졸업팝업");

        Debug.Log(getGraduationNum);        
        getGraduationNum = getGraduationNum.Replace("{0}", $"{_graduationStudentList.Count}");
        _graduationNumber.text = getGraduationNum;

        string getHonor = StringManager.Instance.GetString("UI_Graduation_졸업팝업2");
        
        getHonor = getHonor.Replace("{N}", $"{_graduationManager.TotalHonor}");

        _totalHonorText.text = getHonor;
    }

    private void Refresh()
    {
        RefreshCell();
        GuideText();
    }

}
