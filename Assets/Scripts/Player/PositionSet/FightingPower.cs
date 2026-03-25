using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FightingPower : MonoBehaviour
{
    [SerializeField] CharacterList _characterList;
    //[SerializeField] MercenaryMaker _mercenaryMaker;

    [SerializeField] TextMeshProUGUI _mySchoolName;
    [SerializeField] TextMeshProUGUI _myFightingPowerText;

    [SerializeField] TextMeshProUGUI _rivalSchoolName;
    [SerializeField] TextMeshProUGUI _rivalFightingPowerText;


    [SerializeField] CharacterPowerBox[] _fightingList = new CharacterPowerBox[5];
    [SerializeField] CharacterPowerBox[] _rivalList = new CharacterPowerBox[5];

    // 뒤로가기 버튼
    [SerializeField] private GameObject _backButtonObj;

    private MatchTeam _generatedAwayTeam;

    int _myTotalFightingPower = 0;
    int _rivalTotalFightingPower = 0;

    private List<Student> _myMatchingStudentList = new List<Student>();
    private List<Student> _rivalMatchingStudentList = new List<Student>();
    public List<Student> MyMatchingStudentList => _myMatchingStudentList;
    public List<Student> RivalMatchingStudentList => _rivalMatchingStudentList;

    public void OnClickSetUIIndex()
    {
        PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 0);
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += RefreshUI;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= RefreshUI;
    }

    public void Init()
    {
        // Init이 호출될 때 가장 먼저 뒤로가기 버튼 상태를 결정합니다.
        CheckBackButtonVisibility();
        _rivalMatchingStudentList.Clear();
        if (SaveLoadManager.Instance != null)
        {
            var myData = new StudentSaveData();
            if (SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, out myData))
            {
                _myMatchingStudentList = myData.studentList;
                StudentManager.Instance.InitStudenList(_myMatchingStudentList);
            }
                

            var rivalData = new StudentSaveData();
            if (SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.RIVAL_STUDENT_MATCHING_PATH, out rivalData))
                _rivalMatchingStudentList = rivalData.studentList;
        }
        
        _myTotalFightingPower = 0;
        if (_myMatchingStudentList.Count > 0 && _myMatchingStudentList != null)
        {
            for (int i = 0; i < Mathf.Min(_myMatchingStudentList.Count, _fightingList.Length); i++)
            {
                var s = _myMatchingStudentList[i];
                // s.OnStatChanged();
                s.RebuildStatDict();
                _fightingList[i].Init(s);
                _myTotalFightingPower += (s.Attack + s.Defense);
            }
        }

        _rivalTotalFightingPower = 0;

        // 현 week ID 행에 저장된 league ID를 받아온다.
        // var leagueId = CalendarManager.Instance.GetCurrentLeagueId();
        var leagueMgr = LeagueManager.Instance;
        if (leagueMgr == null) return;

        var currentLeague = leagueMgr.CurrentLeague;
        if (currentLeague == null)
        {
            Debug.Log("현 리그 데이터가 널임");
            return;    
        }

        string opponentTeamId = leagueMgr.GetOpponentTeamId(currentLeague.matchRecords);
        if (opponentTeamId == null)
        {
            Debug.Log("현 상대 팀 데이터가 없음! (레코드 데이터가 없음)");
            return;
        }

        if (currentLeague != null)
        {
            string myTeamId = StudentManager.TEAM_ID;

            // 현재 라운드의 내 매치 기록 찾기
            var myMatch = currentLeague.matchRecords.Find(m =>
                m.roundIndex == currentLeague.currentRoundIndex &&
                (m.homeTeamId == myTeamId || m.awayTeamId == myTeamId));

            if (myMatch != null)
            {
                // 내가 홈이면 어웨이가 적, 내가 어웨이면 홈이 적
                opponentTeamId = myMatch.homeTeamId == myTeamId ? myMatch.awayTeamId : myMatch.homeTeamId;
            }
        }

        MatchTeam homeTeam = EnemyTeamFactory.Instance.ConvertToTeam(TeamSide.Home, StudentManager.Instance.CurrentTeam);
        _generatedAwayTeam = EnemyTeamFactory.Instance.ConvertToTeam(TeamSide.Away, LeagueTeamManager.Instance.GetTeamById(opponentTeamId));
        if (_rivalMatchingStudentList == null || _rivalMatchingStudentList.Count == 0)
        {
            _rivalMatchingStudentList = new List<Student>();

            // LeagueTeamManager에서 상대 팀의 실제 Team 객체를 가져옵니다.
            Team actualRivalTeam = LeagueTeamManager.Instance.GetTeamById(opponentTeamId);

            if (actualRivalTeam != null)
            {
                for (int i = 0; i < actualRivalTeam.Members.Length; i++)
                {
                    Student realStudent = actualRivalTeam.Members[i];

                    // 이미 매니저가 종족/비주얼을 다 채워놨으므로 딕셔너리만 재구성해주면 됩니다.
                    realStudent.RebuildStatDict();

                    _rivalMatchingStudentList.Add(realStudent);
                    _rivalList[i].Init(realStudent);
                    _rivalTotalFightingPower += (realStudent.Attack + realStudent.Defense);
                }
            }
        }
        //if (generatedAwayTeam == null)
        //{
        //    Debug.LogError("적 팀 생성 실패");
        //    return;
        //}
        //Debug.Log($"[생성 확인] 상대 1번 선수 2점슛 스탯: {generatedAwayTeam.Roster[0].GetStat(MatchStatType.TwoPoint)}");

        //if (_rivalMatchingStudentList != null && _rivalMatchingStudentList.Count > 0)
        //{
        //    for (int i = 0; i < _rivalMatchingStudentList.Count; i++)
        //    {
        //        var r = _rivalMatchingStudentList[i];
        //        r.RebuildStatDict();
        //        _rivalList[i].Init(r);
        //        _rivalTotalFightingPower += (r.Attack + r.Defense);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < _rivalList.Length; i++)
        //    {
        //        MatchPlayer mp = generatedAwayTeam.Roster[i];
        //        Student rivalStudent = new Student();

        //        rivalStudent.SetName(mp.PlayerName[0], mp.PlayerName[1], mp.PlayerName[2]);
        //        rivalStudent.SetPosition(mp.MainPosition);

        //        var mappedStats = new List<Stat>
        //{
        //    new Stat(potential.Stat2pt, mp.GetStat(MatchStatType.TwoPoint), 99, 1),
        //    new Stat(potential.Stat3pt, mp.GetStat(MatchStatType.ThreePoint), 99, 1),
        //    new Stat(potential.StatPass, mp.GetStat(MatchStatType.Pass), 99, 1),
        //    new Stat(potential.StatBlock, mp.GetStat(MatchStatType.Block), 99, 1),
        //    new Stat(potential.StatSteal, mp.GetStat(MatchStatType.Steal), 99, 1),
        //    new Stat(potential.StatRebound, mp.GetStat(MatchStatType.Rebound), 99, 1)
        //};

        //        rivalStudent.SetStat(mappedStats);
        //        rivalStudent.OnStatChanged();

        //        _rivalList[i].Init(rivalStudent);
        //        _rivalMatchingStudentList.Add(rivalStudent);

        //        _rivalTotalFightingPower += (rivalStudent.Attack + rivalStudent.Defense);
        //    }
        //}

        _rivalSchoolName.text = StringManager.Instance.GetString(_generatedAwayTeam.TeamName);
        StringManager.Instance.ApplyFont(_rivalSchoolName);
        _rivalFightingPowerText.text = _rivalTotalFightingPower.ToString();
        SetText();
    }

    public void SetText()
    {
        _mySchoolName.text = GameManager.Instance.SaveData.schoolName;
        _myFightingPowerText.text = _myTotalFightingPower.ToString();
    }

    //public void SaveRivalMachingStudentData()
    //{
    //    if (_rivalMatchingStudentList.Count < 1 || _rivalMatchingStudentList == null) return;

    //    int rivalCnt = _rivalMatchingStudentList.Count;

    //    var rivalData = new StudentSaveData(rivalCnt, _rivalMatchingStudentList);

    //    if (SaveLoadManager.Instance == null) return;
    //    SaveLoadManager.Instance.Save(FilePath.RIVAL_STUDENT_MATCHING_PATH, rivalData);
    //}

    public void OnClickStartMatch()
    {
        // CalendarManager.Instance.NextTurn();

        // [디버그] GameManager로 넘기기 직전에 스탯이 살아있는지 확인
        if (MyMatchingStudentList != null && MyMatchingStudentList.Count > 0)
        {
            var testStd = MyMatchingStudentList[0];
            Debug.Log($"<color=yellow>[씬 전환 직전 확인]</color> {testStd.Name} 선수를 시뮬레이터로 보냅니다! 현재 2점슛 스탯: {testStd.GetCurrentStat(potential.Stat2pt)}");
        }

        GameManager.Instance.LoadMatchSceneWithData("Test_Simul", MyMatchingStudentList, RivalMatchingStudentList);
    }
    
    // 리그 진행 상태에 따라 뒤로가기 버튼 켜기/끄기
    private void CheckBackButtonVisibility()
    {
        if (_backButtonObj == null) return;

        bool isMidLeague = false;
        var currentLeague = LeagueManager.Instance.CurrentLeague;

        // 현재 진행 중인 리그가 있고, 아직 종료되지 않았다면
        if (currentLeague != null && !currentLeague.isFinished)
        {
            // 0라운드가 아니다 = 이미 1라운드(첫 경기)를 치르고 2라운드 이상 진행 중이다
            if (currentLeague.currentRoundIndex > 0)
            {
                isMidLeague = true;
            }
        }

        // 리그 중간(isMidLeague == true)이면 버튼을 숨기고(false), 첫 경기거나 리그 중이 아니면 버튼을 보입니다(true).
        _backButtonObj.SetActive(!isMidLeague);
    }

    private void RefreshUI()
    {
        _rivalSchoolName.text = StringManager.Instance.GetString(_generatedAwayTeam.TeamName);
        StringManager.Instance.ApplyFont(_rivalSchoolName);
    }
}
