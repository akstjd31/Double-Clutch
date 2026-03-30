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

    [Header("페이드 인/아웃 연출 소요시간")]
    [SerializeField] float _fadeTime = 0.5f;
    [Header("엔딩 크레딧 소요 시간")]
    [SerializeField] float _rollSpeed = 15f;
    [Header("AndYou 이미지 송출 시간")]
    [SerializeField] float _andYouTime = 3f;
    
    public float FadeTime => _fadeTime;
    public float RollSpeed => _rollSpeed;
    public float AndYouTime => _andYouTime;

    public static EndingManager Instance;

    private int _currentIndex = 0;
    private ScriptData _currentScript;
    private List<Student> _studentRollIndex = new List<Student>();
    public List<Student> StudentRollIndex => _studentRollIndex;
    

    private void Awake()
    {
        Instance = this;        
    }

    private void Start()
    {
        PlayEndingEvent();
    }

    public void PlayEndingEvent()
    {
        _currentScript = _endingScriptDataReader.DataList[_currentIndex];

        CheckFade();
        _uiController.SetBackgroundImage(_currentScript.background);
        _uiController.SetCharacterImage(_currentScript.portrait);
        _uiController.SetCharacterSpeaking(_currentScript.speaking);
        _uiController.SetText(_currentScript.name, _currentScript.text);
        _uiController.PlayBGM(_currentScript.bgm);
        _uiController.PlaySFX(_currentScript.sfx);
    }

    private void PlayEndingRoll()
    {
        _endingRollController.Init(GetEveryStudentList(), _endingRollDataReader.DataList);        

        _endingRollController.StartRoll(_rollSpeed);
    }

    public void PlayAndYou()
    {
        _uiController.PlayAndYouSequence();
    }

    public void OnNextButtonClick()
    {
        if (_currentIndex == -1)
        {
            return;
        }        
        if (_uiController.IsFading) //페이드 연출 중에는 버튼 눌러도 반응 x (어차피 Fade 패널로 눌리지도 않음)
        {
            return;
        }

        if (_endingScriptDataReader.DataList[_currentIndex].scriptType == 1)
        {
            _currentIndex = -1;
            PlayEndingRoll();
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
                break;
        }        
    }

    private Dictionary<Student, int> GetEveryStudentList() //학생, 기수로 이루어진 딕셔너리 반환
    {
        _studentRollIndex.Clear();
        List<GraduationStudent> graduationList = new List<GraduationStudent>();        
        if (GraduationAlbumManager.Instance != null && GraduationAlbumManager.Instance.SaveData != null)
        {
            graduationList.AddRange(GraduationAlbumManager.Instance.SaveData.graduationStudentList);
        }

        Dictionary<Student, int> studentDict = new Dictionary<Student, int>();
        for (int i = 0; i < graduationList.Count; i++) //졸업생은 학생 / 기수 매칭
        {
            List<Student> students = graduationList[i].studentList;

            for (int j = 0; j < students.Count; j++)
            {
                studentDict.Add(students[j], graduationList[i].graduatingClass);
                _studentRollIndex.Add(students[j]);
            }
        } //재학생은 기수 -1로 매칭
        for (int i = 0; i < StudentManager.Instance.MyStudents.Count; i++)
        {
            studentDict.Add(StudentManager.Instance.MyStudents[i], -1);
            _studentRollIndex.Add(StudentManager.Instance.MyStudents[i]);
        }
        _studentRollIndex.Sort((a, b) => a.StudentId.CompareTo(b.StudentId));
        return studentDict;
    }
}
