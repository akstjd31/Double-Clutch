using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ????: ???? ???? ?? ????
/// </summary>


public class StudentManager : Singleton<StudentManager> 
{
    private const string SAVE_FILE = "StudentSave.json";

    int _idCount = 0; //???? ???? ?? ?��??? ???? id ?????(????/?��? ???)
    int _recruitLimit = 5; //???? ???? ????
    public int RecruitLimit => GetRecruitLimit();
    public bool IsStable => GetRecruitLimit() == _myStudents.Count;
    // public static StudentManager Instance { get; private set; }
    [SerializeField] StudentFactory _studentFactory; //???? ?????? ????
    [SerializeField] private List<Student> _myStudents = new List<Student>(); //???? ???
    public List<Student> MyStudents => _myStudents;

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

        SaveGame();
    }

    public void ReleaseStudent(Student target)
    {
        _myStudents.Remove(target);
        Debug.Log($"{target.Name} 선수가 팀을 떠났습니다.");

        SaveGame();
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
        StudentSaveData saveData = new StudentSaveData(_idCount, _myStudents);

        // 2. ??????? ???? ????????.
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.Save(SAVE_FILE, saveData);
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance.TryLoad<StudentSaveData>(SAVE_FILE, out var data))
        {
            // 1. ???? ????
            _idCount = data.lastIdCount;
            _myStudents = data.studentList;

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

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        SaveGame();
    }
}
