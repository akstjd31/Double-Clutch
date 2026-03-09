using System.Collections.Generic;
using UnityEngine;

public class StudentSaveData : SaveBase
{
    public int lastIdCount; // StudentManager�� _idCount ����
    public List<Student> studentList; // �� �л� ��� ��ü ����

    public StudentSaveData(int idCount, List<Student> students)
    {
        this.lastIdCount = idCount;
        this.studentList = students;
    }

    // �⺻ ������ (JsonUtility �ε��)
    public StudentSaveData()
    {

    }
}
