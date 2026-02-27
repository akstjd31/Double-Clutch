using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


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

    private void Start()
    {
        _myStudents = _graduationManager.MyStudents;
        _graduationStudentList = _graduationManager.GraduationStudentList;

        //플레이어 보유 학생 리스트 가져오기
        //_myStudents = StudentManager.Instance.MyStudents;
        CreatCell();
        GuideText();
    }

    private void CreatCell()
    {
        for (int i = 0; i < _graduationStudentList.Count; i++)
        {
            GameObject nameBox = Instantiate(_nameBoxPrefab, _listObject.transform);
            var name = nameBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var honor = nameBox.transform.Find("Honor").GetComponent<TextMeshProUGUI>();
            var script = nameBox.GetComponent<GraduationNameBox>();
            //var color = nameBox.GetComponent<Color>();
            //컬러도 따로 설정 해야하는데... 새로 생성하는거보다는 활성화비활성화로 하는거 나을지도 
            //버튼을 생성할 때 학생 데이터 저장
            script.clickData = _graduationStudentList[i];
            script.graduationUI = _characterProfileDetail;
            name.text = _graduationStudentList[i].Name;
            //honor.text = _graduationStudentList[i].Honor.ToString();
        }
    }

    private void GuideText()
    {
        _graduationNumber.text = $"{_graduationStudentList.Count}명의 학생이 졸업 했습니다.";

        _totalHonorText.text = $"최종 획득 명성 +{_graduationManager.TotalHonor}";
    }
}
