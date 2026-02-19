using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 역할: 선수 영입 및 관리
/// </summary>
public class StudentManager : MonoBehaviour 
{
    int _idCount = 0; //선수 영입 시 부여할 고유 id 카운터(저장/로드 필요)
    int _recruitLimit = 5; //선수 영입 최대치
    public bool CanRecruit => _recruitLimit > _myStudents.Count;
    public static StudentManager Instance { get; private set; }
    [SerializeField] StudentFactory _studentFactory; //선수 생성용 팩토리

    [SerializeField] private List<Student> _myStudents = new List<Student>(); //선수 목록
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MakeTestStudents(4);
    }

    public void MakeTestStudents(int n) //선수를 랜덤하게 n명 채워넣는 매서드(테스트용)
    {
        for (int i = 0; i < n; i++)
        {
            RecruitNewStudent(_studentFactory.MakeRandomStudent());
        }
    }    

    public void RecruitNewStudent(Student newStudent)
    {
        if (!CanRecruit)
        {
            Debug.Log("영입 최대치로 인한 영입 불가!");
            return;
        }
        _myStudents.Add(newStudent);

        newStudent.SetStudentId(_idCount++);

        //여기 Ui 갱신 로직 넣기
    }

    // 전체 학생 목록 가져오기
    public List<Student> GetAllStudents() => _myStudents;

    // id로 학생 조회
    public Student FindStudentById(int id)
    {
        return _myStudents.Find(s => s.StudentId == id);
    }



}
