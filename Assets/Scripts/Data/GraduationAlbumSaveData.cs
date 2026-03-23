using System.Collections.Generic;
using UnityEngine;

// 졸업 앨범에 포함될 데이터
public class GraduationAlbumSaveData : SaveBase
{
    List<GraduationStudent> graduationStudentList;
}

// 기수별 학생 목록 데이터
public class GraduationStudent
{
    public int graduatingClass;         // 기수
    public List<Student> studentList;
}
