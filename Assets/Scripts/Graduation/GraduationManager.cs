using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GraduationManager : MonoBehaviour
{
    [SerializeField] private bool _isGraduationSkip = false;

    [SerializeField] private PromotionPanel _promotionPanel;
    [SerializeField] private PassiveBox _passiveBox;

    [SerializeField] private List<Student> _myStudents;

    //졸업생 리스트
    [SerializeField] private List<Student> _graduationStudentList = new List<Student>();

    //진급학생 리스트
    [SerializeField] private List<int> _promotionStudentList = new List<int>();


    private int _turn;
    private int _totalHonor;


    public List<Student> GraduationStudentList => _graduationStudentList;
    public PromotionPanel PromotionPanel => _promotionPanel;
    public List<int> PromotionStudentList => _promotionStudentList;
    public List<Student> MyStudents => _myStudents;
    public int Turn { get { return _turn; } set { _turn = value; } }
    public int TotalHonor => _totalHonor;
    public bool IsGraduationSkip => _isGraduationSkip;

    private void Start()
    {
        _turn = 0;
        _myStudents = StudentManager.Instance.MyStudents;
        if( _myStudents == null )
        {
            Debug.Log("학생 리스트없음");
        }
        ListCreat();
        
        _turn = 0;
        //처음 학생 프로필 띄우기
        _promotionPanel.GetList();
        _promotionPanel.UpdateProfile();
    }

    private void ListCreat()
    {
        _totalHonor = 0;

        for (int i = 0; i < _myStudents.Count; i++)
        {
            //3학년이면 졸업생 리스트에 추가
            if (_myStudents[i].Grade == 3)
            {
                _graduationStudentList.Add(_myStudents[i]);
                Debug.Log(_myStudents[i].Name + "추가");
            }
            else
            {
                _promotionStudentList.Add(_myStudents[i].StudentId);
                _myStudents[i].SetGrade(_myStudents[i].Grade+1);
                Debug.Log($"{_myStudents[i].Name} : {_myStudents[i].Grade} 학년 진급생");
            }
        }

        if (_graduationStudentList.Count == 0)
        {
            _isGraduationSkip = true;
        }

        ReleaseStudent();
    }

    private void ReleaseStudent()
    {
        for (int i = 0; i < _graduationStudentList.Count; i++)
        {
            StudentManager.Instance.ReleaseStudent(_graduationStudentList[i]);
        }
        StudentManager.Instance.SaveGame();
    }

    public void NextScene()
    {
        _isGraduationSkip = false;
        _passiveBox.SelectSkillSave.Clear();

        //초기화 하기 전에 명예의전당에 전달
        _graduationStudentList.Clear();

        CalendarManager.Instance.NextTurn();
        GameManager.Instance.GoToLobby();
    }
}
