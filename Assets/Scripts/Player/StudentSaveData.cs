using System.Collections.Generic;
using UnityEngine;

public class StudentSaveData : SaveBase
{
    public int lastIdCount; // StudentManager의 _idCount 저장
    public List<Student> studentList; // 내 학생 목록 전체 저장

    public StudentSaveData(int idCount, List<Student> students)
    {
        this.lastIdCount = idCount;
        this.studentList = students;
    }

    // 기본 생성자 (JsonUtility 로드용)
    public StudentSaveData()
    {

    }
}
