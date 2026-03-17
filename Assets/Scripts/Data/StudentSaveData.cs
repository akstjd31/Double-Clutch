using System.Collections.Generic;
using UnityEngine;

public class StudentSaveData : SaveBase
{
    public int lastIdCount; // StudentManager�� _idCount ����
    public List<Student> studentList; // �� �л� ��� ��ü ����
    public Team currentTeam;

    public StudentSaveData(int idCount, List<Student> students, Team currentTeam)
    {
        this.lastIdCount = idCount;
        this.studentList = students;
        this.currentTeam = currentTeam;
    }

    // �⺻ ������ (JsonUtility �ε��)
    public StudentSaveData()
    {

    }
}
