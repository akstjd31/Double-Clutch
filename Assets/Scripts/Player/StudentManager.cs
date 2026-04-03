using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Constants;
/// <summary>
/// ????: ???? ???? ?? ????
/// </summary>


public class StudentManager : Singleton<StudentManager> 
{

    int _idCount = 0; //???? ???? ?? ?��??? ???? id ?????(????/?��? ???)
    public const int BasicRecruitLimit = 5;
    int _recruitLimit = 5; //???? ???? ????
    public int RecruitLimit => GetRecruitLimit();
    public bool IsStable => GetRecruitLimit() == _myStudents.Count;
    
    [SerializeField] StudentFactory _studentFactory; //???? ?????? ????
    [SerializeField] private List<Student> _myStudents = new List<Student>(); //???? ???
    [SerializeField] Team _currentTeam;
    public List<Student> MyStudents => _myStudents;
    public Team CurrentTeam => _currentTeam;
    public int GetRecruitLimit()
    {
        return _recruitLimit + InfraManager.Instance.GetInfraEffectValueByEffectType(infraEffectType.AddRoster);
    }

    protected override void Awake()
    {
        base.Awake();
        // Instance = this;
    }

    private void Start()
    {
        if (_studentFactory != null) 
            _studentFactory.InitDatas();

        //if (_myStudents.Count == 0)
        //{
        //    MakeTestStudents(3);
        //}
        
        LoadGame();
    }
    
    public void InitStudent(Student student)
    {
        _studentFactory.JustInitStudent(student);
    }

    public void InitStudenList(List<Student> studentList)
    {
        _studentFactory.InitStudentList(studentList);
    }
    public void SetCurrentTeam(List<Student> players)
    {
        _currentTeam = new Team(PrefKeys.PLAYER_TEAM_ID, true);
        for (int i = 0; i < players.Count; i++)
        {
            _currentTeam.SetMember(i, players[i]);
        }        

        Debug.Log("팀 생성 완료!");
    }


    public List<Student> MakeRandomTeam(int n) // n?????? ?????? ???? ??????? ?????? ???
    {
        List<Student> newTeam = new List<Student>();
        for (int i = 0; i < n; i++)
        {
            newTeam.Add(_studentFactory.MakeRandomStudent());
        }
        return newTeam;
    }

    public Student MakeRandomStudent()
    {
        return _studentFactory.MakeRandomStudent();
    }

    public void MakeTestStudents(int n) //?????? ??????? n?? ?????? ?????(??????)
    {
        for (int i = 0; i < n; i++)
        {
            RecruitNewStudent(_studentFactory.MakeRandomStudent());
        }
    }    

    public void RecruitNewStudent(Student newStudent)
    {        
        _myStudents.Add(newStudent);

        newStudent.SetStudentId(_idCount++);

        //SaveGame();
    }

    public void ReleaseStudent(Student target)
    {
        _myStudents.Remove(target);
        Debug.Log($"{target.Name} 선수가 팀을 떠났습니다.");

        //SaveGame();
    }

    // ??? ?��? ??? ????????
    public List<Student> GetAllStudents() => _myStudents;

    // id?? ?��? ???
    public Student FindStudentById(int id)
    {
        return _myStudents.Find(s => s.StudentId == id);
    }

    public void SaveGame()
    {
        // 1. ?????? ??????? ??? ??????.
        StudentSaveData saveData = new StudentSaveData(_idCount, _myStudents, _currentTeam);

        // 2. ??????? ???? ????????.
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.Save(FilePath.STUDENT_PATH, saveData);
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.STUDENT_PATH, out var data))
        {
            // 1. ???? ????
            _idCount = data.lastIdCount;
            _myStudents = data.studentList;
            _currentTeam = data.currentTeam;

            // 2. ???? ???? ?��?? ?��????? ScriptableObject(SO) ?????? ????????!
            // ?????? ??? ??? DB?? ????? ??? Init ????? ????.
            foreach (var student in _myStudents)
            {
                _studentFactory.InitStudent(student);
            }

            Debug.Log("���� �ε� �Ϸ�!");
            // UI ???? ???? ??? ???
        }
    }

    public void OnInfraUpdated()
    {
        foreach (var student in _myStudents)
        {
            student.OnInfraUpdated();
        }
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        SaveGame();
    }

    public StudentFactory GetFactory() => _studentFactory;
}
