using System.Collections.Generic;
using UnityEngine;

public class GraduationStudentSaveData : SaveBase
{
    public int graduatingClass;         // 기수
    public List<Student> studentList;   // 해당 기수에 졸업한 학생 리스트
}
