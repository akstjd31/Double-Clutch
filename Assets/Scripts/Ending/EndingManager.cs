using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] EndingEventDataReader _endingEventDataReader;
    [SerializeField] ScriptDataReader _endingScriptDataReader;    
    [SerializeField] EndingRollDataReader _endingRollDataReader;

    [SerializeField] EndingUIController _uiController;
    [SerializeField] EndingRollController _endingRollController;

    [SerializeField] Button _nextButton;
    public static EndingManager Instance;

    private int _currentIndex = 0;
    private ScriptData _currentScript;

    private void Awake()
    {
        Instance = this;
        
    }

    public void PlayEndingEvent()
    {
        _currentScript = _endingScriptDataReader.DataList[_currentIndex];

        CheckFade();
        _uiController.SetBackgroundImage(_currentScript.background);
        _uiController.SetCharacterImage(_currentScript.portrait);
        _uiController.SetText(_currentScript.name, _currentScript.text);
    }    

    public void OnNextButtonClick()
    {
        if (_uiController.IsFading) //페이드 연출 중에는 버튼 눌러도 반응 x (어차피 Fade 패널로 눌리지도 않음)
        {
            return;
        }

        if (_endingScriptDataReader.DataList[_currentIndex].scriptType == 1)
        {
            //이벤트 종료 후 엔딩롤로 전환
            return;
        }
        _currentIndex++;        
        PlayEndingEvent();
    }

    private void CheckFade()
    {
        switch (_currentScript.startEffectId)
        {
            case "Fade_In": 
                _uiController.FadeIn();
                break;
            case "Fade_Out":
                _uiController.FadeOut();
                break;
            default:
                Debug.Log("Script 테이블의 startEffectId 작성 양식이 Fade_In , Fade_Out 으로 되어 있는지 확인하세요.");
                break;
        }        
    }

    private List<Student> GetEveryStudentList()
    {
        List<GraduationStudent> graduationList = GraduationAlbumManager.Instance.SaveData.graduationStudentList;
        List<Student> studentList = new List<Student>();
        for (int i = 0; i < graduationList.Count; i++)
        {
            List<Student> students = graduationList[0].studentList;

            for (int j = 0; j < students.Count; j++)
            {
                studentList.Add(students[j]);
            }
        }
        for (int i = 0; i < StudentManager.Instance.MyStudents.Count; i++)
        {
            studentList.Add(StudentManager.Instance.MyStudents[i]);
        }        
        studentList.Sort((a, b) => b.StudentId.CompareTo(a.StudentId));
        return studentList;
    }
}
