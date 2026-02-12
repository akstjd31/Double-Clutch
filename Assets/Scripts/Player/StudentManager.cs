using System.Collections.Generic;
using UnityEngine;

public class StudentManager : MonoBehaviour
{
    public static StudentManager Instance { get; private set; }
    [SerializeField] StudentFactory _studentFactory; //선수 생성용 팩토리

    [SerializeField] private List<Student> _myStudents = new List<Student>(); //선수 목록
    private void Awake()
    {
        Instance = this;
    }
    public void PickNewStudent()
    {
        Student newStudent = _studentFactory.MakeRandomStudent();
        _myStudents.Add(newStudent);

        Debug.Log($"[신규 영입] {newStudent.Name} ({newStudent.Grade}학년)이 입학했습니다.");

        // 데이터 확인을 위해 즉시 상세 정보 출력
        PrintStudentDetails(newStudent);
    }

    // 전체 학생 목록 가져오기
    public List<Student> GetAllStudents() => _myStudents;

    // 특정 학생 이름으로 찾기
    public Student FindStudentByName(string name)
    {
        return _myStudents.Find(s => s.Name == name);
    }

    // 학생 상세 정보 로그 출력 (디버그용)
    public void PrintStudentDetails(Student s)
    {
        string passiveNames = string.Join(", ", s.Passive.ConvertAll(p => p.skillName));
        string statInfo = "";

        // 1번 방식(GetStatValue)을 사용한다고 가정
        foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
        {
            statInfo += $"{type}: {s.GetCurrentStat(type)} ";
        }

        Debug.Log($"이름: {s.Name} | 학년: {s.Grade}\n" +
                  $"패시브: [{passiveNames}]\n" +
                  $"스탯: {statInfo}");
    }


}
