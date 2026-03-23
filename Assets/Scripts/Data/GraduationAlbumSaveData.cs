using System;
using System.Collections.Generic;
using UnityEngine;

// 졸업 앨범에 포함될 데이터
[Serializable]
public class GraduationAlbumSaveData : SaveBase
{
    public List<GraduationStudent> graduationStudentList = new List<GraduationStudent>();

    public GraduationAlbumSaveData() {}
}

// 기수별 학생 목록 데이터
[Serializable]
public class GraduationStudent
{
    public int graduatingClass;         // 기수
    public List<Student> studentList = new List<Student>();

    public GraduationStudent(int gClass, List<Student> stuList)
    {
        graduatingClass = gClass;
        studentList = stuList;
    }
}
