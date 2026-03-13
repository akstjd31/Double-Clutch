using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FightingPower : MonoBehaviour
{
    [SerializeField] CharacterList _characterList;
    [SerializeField] MercenaryMaker _mercenaryMaker;

    [SerializeField] TextMeshProUGUI _mySchoolName;
    [SerializeField] TextMeshProUGUI _myFightingPowerText;

    [SerializeField] TextMeshProUGUI _rivalSchoolName;
    [SerializeField] TextMeshProUGUI _rivalFightingPowerText;


    [SerializeField] CharacterPowerBox[] _fightingList = new CharacterPowerBox[5];
    [SerializeField] CharacterPowerBox[] _rivalList = new CharacterPowerBox[5];

    int _myTotalFightingPower = 0;
    int _rivalTotalFightingPower = 0;

    private List<Student> _myMatchingStudentList = new List<Student>();
    private List<Student> _rivalMatchingStudentList = new List<Student>();
    public List<Student> MyMatchingStudentList => _myMatchingStudentList;
    public List<Student> RivalMatchingStudentList => _rivalMatchingStudentList;

    public void Init()
    {
        if (SaveLoadManager.Instance != null)
        {
            var myData = new StudentSaveData();
            if (SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, out myData))
                _myMatchingStudentList = myData.studentList;

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
        var leagueId = CalendarManager.Instance.GetCurrentLeagueId();
        MatchTeam generatedAwayTeam = EnemyTeamFactory.Instance.CreateEnemyTeam("Team_DOM_03", leagueId);
        
        if (generatedAwayTeam == null)
        {
            Debug.LogError("?? ?? ???? ????!");
            return;
        }
        Debug.Log($"[???? ???] ???? 1?? ???? 2???? ????: {generatedAwayTeam.Roster[0].GetStat(MatchStatType.TwoPoint)}");

        if (_rivalMatchingStudentList != null && _rivalMatchingStudentList.Count > 0)
        {
            for (int i = 0; i < _rivalMatchingStudentList.Count; i++)
            {
                var r = _rivalMatchingStudentList[i];
                r.RebuildStatDict();
                _rivalList[i].Init(r);
                _rivalTotalFightingPower += (r.Attack + r.Defense);
            }
        }
        else
        {
            for (int i = 0; i < _rivalList.Length; i++)
            {
                MatchPlayer mp = generatedAwayTeam.Roster[i];
                Student rivalStudent = new Student();

                rivalStudent.SetName(mp.PlayerName[0], mp.PlayerName[1], mp.PlayerName[2]);
                rivalStudent.SetPosition(mp.MainPosition);

                var mappedStats = new List<Stat>
        {
            new Stat(potential.Stat2pt, mp.GetStat(MatchStatType.TwoPoint), 99, 1),
            new Stat(potential.Stat3pt, mp.GetStat(MatchStatType.ThreePoint), 99, 1),
            new Stat(potential.StatPass, mp.GetStat(MatchStatType.Pass), 99, 1),
            new Stat(potential.StatBlock, mp.GetStat(MatchStatType.Block), 99, 1),
            new Stat(potential.StatSteal, mp.GetStat(MatchStatType.Steal), 99, 1),
            new Stat(potential.StatRebound, mp.GetStat(MatchStatType.Rebound), 99, 1)
        };

                rivalStudent.SetStat(mappedStats);
                rivalStudent.OnStatChanged();

                _rivalList[i].Init(rivalStudent);
                _rivalMatchingStudentList.Add(rivalStudent);

                _rivalTotalFightingPower += (rivalStudent.Attack + rivalStudent.Defense);
            }
        }

        _rivalSchoolName.text = generatedAwayTeam.TeamName;
        _rivalFightingPowerText.text = _rivalTotalFightingPower.ToString();
        SetText();
    }

    public void SetText()
    {
        _mySchoolName.text = GameManager.Instance.SaveData.schoolName;
        _myFightingPowerText.text = _myTotalFightingPower.ToString();
    }

    public void SaveRivalMachingStudentData()
    {
        if (_rivalMatchingStudentList.Count < 1 || _rivalMatchingStudentList == null) return;

        int rivalCnt = _rivalMatchingStudentList.Count;

        var rivalData = new StudentSaveData(rivalCnt, _rivalMatchingStudentList);

        if (SaveLoadManager.Instance == null) return;
        SaveLoadManager.Instance.Save(FilePath.RIVAL_STUDENT_MATCHING_PATH, rivalData);
    }

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
}
