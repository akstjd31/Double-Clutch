using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 역할: 선수 영입 및 관리
/// </summary>


public class StudentManager : MonoBehaviour 
{
    private const string SAVE_FILE = "StudentSave.json";

    int _idCount = 0; //선수 영입 시 부여할 고유 id 카운터(저장/로드 필요)
    int _recruitLimit = 5; //선수 영입 최대치
    public bool CanRecruit => _recruitLimit > _myStudents.Count;
    public static StudentManager Instance { get; private set; }
    [SerializeField] StudentFactory _studentFactory; //선수 생성용 팩토리

    [SerializeField] private List<Student> _myStudents = new List<Student>(); //선수 목록
    public List<Student> MyStudents => _myStudents;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadGame();

        if (_myStudents.Count == 0)
        {
            MakeTestStudents(5);
        }
        
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

    public void SaveGame()
    {
        // 1. 저장할 데이터를 팩에 담습니다.
        StudentSaveData saveData = new StudentSaveData(_idCount, _myStudents);

        // 2. 매니저를 통해 저장합니다.
        SaveLoadManager.Instance.Save(SAVE_FILE, saveData);
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance.TryLoad<StudentSaveData>(SAVE_FILE, out var data))
        {
            // 1. 변수 복구
            _idCount = data.lastIdCount;
            _myStudents = data.studentList;

            // 2. ★매우 중요★ 로드된 학생들은 ScriptableObject(SO) 연결이 끊겨있음!
            // 팩토리가 들고 있는 DB를 이용해 다시 Init 해줘야 합니다.
            foreach (var student in _myStudents)
            {
                _studentFactory.InitStudent(student);
            }

            Debug.Log("게임 로드 완료!");
            // UI 갱신 로직 호출 필요
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

}
