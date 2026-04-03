using System.Collections.Generic;
using UnityEngine;

public class StudentSaveData : SaveBase
{
    public int lastIdCount; // StudentManager�� _idCount ����
    public List<Student> studentList; // �� �л� ��� ��ü ����
    public Team currentTeam;

    public List<Student> recruitCandidates = new List<Student>();
    public StudentSaveData(int idCount, List<Student> students, Team currentTeam, List<Student> candidates = null)
    {
        this.lastIdCount = idCount;
        this.studentList = students;
        this.currentTeam = currentTeam;
        this.recruitCandidates = candidates ?? new List<Student>();
    }

    // �⺻ ������ (JsonUtility �ε��)
    public StudentSaveData()
    {
        recruitCandidates = new List<Student>();
    }
}
