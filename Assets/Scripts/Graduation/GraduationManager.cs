using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GraduationManager : MonoBehaviour
{
    [SerializeField] private bool _isGraduationSkip = false;

    [SerializeField] private PromotionPanel _promotionPanel;
    [SerializeField] private PassiveBox _passiveBox;

    [SerializeField] private List<Student> _myStudents;

    //졸업생 리스트
    [SerializeField] private List<Student> _graduationStudentList = new List<Student>();

    //진급학생 리스트
    [SerializeField] private List<int> _promotionStudentList = new List<int>();


    private int _turn;
    private int _totalHonor;


    public List<Student> GraduationStudentList => _graduationStudentList;
    public PromotionPanel PromotionPanel => _promotionPanel;
    public List<int> PromotionStudentList => _promotionStudentList;
    public List<Student> MyStudents => _myStudents;
    public int Turn { get { return _turn; } set { _turn = value; } }
    public int TotalHonor => _totalHonor;
    public bool IsGraduationSkip => _isGraduationSkip;

    private void Start()
    {
        _turn = 0;
        _myStudents = StudentManager.Instance.MyStudents;
        if (_myStudents == null)
        {
            Debug.Log("학생 리스트없음");
        }
        if (GameManager.Instance.SaveData.isGraduationPending)
        {
            // 학년 이미 증가된 상태 - ListCreat() 생략
            // pendingPassiveSelection == true 인 학생만 진급 리스트로 복구
            _myStudents = StudentManager.Instance.MyStudents;
            foreach (var s in _myStudents)
            {
                if (s.pendingPassiveSelection)
                    _promotionStudentList.Add(s.StudentId);
            }
        }
        else
        {
            ListCreat();
        }

        _turn = 0;
        //처음 학생 프로필 띄우기
        _promotionPanel.GetList();
        _promotionPanel.UpdateProfile();
    }

    private void ListCreat()
    {
        _totalHonor = 0;

        for (int i = 0; i < _myStudents.Count; i++)
        {
            //3학년이면 졸업생 리스트에 추가
            if (_myStudents[i].Grade == 3)
            {
                _graduationStudentList.Add(_myStudents[i]);
                Debug.Log(_myStudents[i].Name + "추가");
            }
            else
            {
                _promotionStudentList.Add(_myStudents[i].StudentId);
                _myStudents[i].SetGrade(_myStudents[i].Grade + 1);
                _myStudents[i].pendingPassiveSelection = true;
                Debug.Log($"{_myStudents[i].Name} : {_myStudents[i].Grade} 학년 진급생");
            }
        }

        if (_graduationStudentList.Count == 0)
        {
            _isGraduationSkip = true;
        }

        var gameMgr = GameManager.Instance;
        if (gameMgr == null) return;

        ReleaseStudent(gameMgr);

        gameMgr.SetGraduationPending(true);

        var saveData = gameMgr.SaveData;
        if (saveData == null) return;

        // 누적된 명예 계산
        gameMgr.SetHonor(saveData.honor + saveData.totalWinHonor);
        gameMgr.ClearTotalWinHonorData();
    }

    private void ReleaseStudent(GameManager gameMgr)
    {
        if (gameMgr == null) return;

        int totalHonor = 0;
        for (int i = 0; i < _graduationStudentList.Count; i++)
        {
            StudentManager.Instance.ReleaseStudent(_graduationStudentList[i]);

            totalHonor += _graduationStudentList[i].TotalFame;
            gameMgr.AddGraduationCount(_graduationStudentList[i].VisualId);
        }

        // 졸업한 선수의 누적 명예를 다 더해서 실제로 데이터에 갱신시키기
        gameMgr.SetHonor(gameMgr.SaveData.honor + totalHonor);
        StudentManager.Instance.SaveGame();

        if (!_isGraduationSkip)
        {
            var gaMgr = GraduationAlbumManager.Instance;
            if (gaMgr == null) return;

            int year = gameMgr.SaveData.year;   // 현재 연차
            var gStd = new GraduationStudent(year, _graduationStudentList);   // 1기수부터 시작

            gaMgr.Save(gStd);
        }
    }

    public void NextScene()
    {
        _isGraduationSkip = false;
        GameManager.Instance.SetGraduationPending(false);
        _passiveBox.SelectSkillSave.Clear();

        //초기화 하기 전에 명예의전당에 전달
        _graduationStudentList.Clear();

        CalendarManager.Instance.NextTurn();
        GameManager.Instance.GoToLobby();
    }
}
