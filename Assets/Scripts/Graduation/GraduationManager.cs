using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;

public class GraduationManager : MonoBehaviour
{
    [SerializeField] private PromotionPanel _promotionPanel;

    [SerializeField] private List<Student> _myStudents;

    //졸업생 리스트
    [SerializeField] private List<Student> _graduationStudentList = new List<Student>();
    //[SerializeField] private List<TestStudent> _testGraduationStudentList = new List<TestStudent>();

    //진급학생 리스트
    [SerializeField] private List<Student> _promotionStudentList = new List<Student>();

    //[SerializeField] private List<TestStudent> _myTestStudents = new List<TestStudent>();

    private int _turn;
    private int _totalHonor;


    public List<Student> GraduationStudentList => _graduationStudentList;
    public List<Student> PromotionStudentList => _promotionStudentList;
    public List<Student> MyStudents => _myStudents;

    //public List<TestStudent> TestGraduationStudentList => _testGraduationStudentList;
    //public List<TestStudent> TestPromotionStudentList => _testPromotionStudentList;
    public int Turn => _turn;
    public int TotalHonor => _totalHonor;
    private void OnEnable()
    {
        
        //var test = new TestStudent(0, "테스트1", 0, 100);
        //_myTestStudents.Add(test);
        //test = new TestStudent(1, "테스트2", 1, 200);
        //_myTestStudents.Add(test);
        //test = new TestStudent(2, "테스트3", 2, 300);
        //_myTestStudents.Add(test);
        //test = new TestStudent(3, "테스트4", 2, 400);
        //_myTestStudents.Add(test);
        //test = new TestStudent(4, "테스트5", 2, 500);
        //_myTestStudents.Add(test);
        ListCreat();

        _turn = 0;
        //처음 학생 프로필 띄우기
        //_promotionPanel.Profile(_promotionStudentList[_turn]);
    }

    private void ListCreat()
    {
        _totalHonor = 0;

        for (int i = 0; i < _myStudents.Count; i++)
        {
            //테스트용_3학년이면 졸업생 리스트에 추가
            //if (_myTestStudents[i].Grade == 2)
            //{
            //    _testGraduationStudentList.Add(_myTestStudents[i]);
            //    _totalHonor += _myTestStudents[i].Honor;
            //    Debug.Log(_myTestStudents[i].Name + "추가");
            //}
            //else
            //{
            //    _testPromotionStudentList.Add(_myTestStudents[i]);
            //    Debug.Log($"{_myTestStudents[i].Name} : {_myTestStudents[i].Grade} 학년 진급생");
            //}

            //3학년이면 졸업생 리스트에 추가
            if (_myStudents[i].Grade == 2)
            {
                _graduationStudentList.Add(_myStudents[i]);
                Debug.Log(_myStudents[i].Name + "추가");
            }
            else
            {
                _promotionStudentList.Add(_myStudents[i]);
                Debug.Log($"{_myStudents[i].Name} : {_myStudents[i].Grade} 학년 진급생");
            }
        }
    }

}
