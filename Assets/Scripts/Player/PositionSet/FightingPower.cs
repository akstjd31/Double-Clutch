using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// FightingPower�� ������ ��ũ��Ʈ
/// ��ġ�� ���� ������ PositionSet���� �޾ƿͼ� ���ݷ� �ջ� �� UI�� ǥ��
/// </summary>
public class FightingPower : MonoBehaviour
{
    /// <summary>    
    /// CharacterBox�� Init �޼��� ���ڰ����� �л��� ������ �ش� CharacterBox UI�� �ڵ����� ǥ�õ˴ϴ�.
    /// �ٽ� ����, �� FightingPower�� Init �޼��� �ȿ���, _rivalList�� ����ִ� CharacterBox�� ���� ��� Init���ֽø� �˴ϴ�.
    /// _rivalList�� ���̾��Ű���� �̹� �Ҵ� ä�����ҽ��ϴ�. 
    /// </summary>
    [SerializeField] CharacterList _characterList; // ��ġ�� �л� ���� �޾ƿ� ��ġ ȭ�� UI
    [SerializeField] MercenaryMaker _mercenaryMaker; // �뺴 ������

    [SerializeField] TextMeshProUGUI _mySchoolName; //UI(�츮 �� �б� �̸�)
    [SerializeField] TextMeshProUGUI _myFightingPowerText; //UI(�츮 �� ������ (���ݷ� + ����� ���ջ�))

    [SerializeField] TextMeshProUGUI _rivalSchoolName;//UI(��� �� �б� �̸�)
    [SerializeField] TextMeshProUGUI _rivalFightingPowerText;//UI(��� �� �б� �̸�)


    [SerializeField] CharacterPowerBox[] _fightingList = new CharacterPowerBox[5]; //�츮�� ���� ��â UI(���� �л� �� ��)
    [SerializeField] CharacterPowerBox[] _rivalList = new CharacterPowerBox[5]; //����� ���� ��â UI(���� �л� �� ��)

    int _myTotalFightingPower = 0; //�츮�� ���� ����
    int _rivalTotalFightingPower = 0; //����� ���� ����

    private List<Student> _myMatchingStudentList = new List<Student>();
    private List<Student> _rivalMatchingStudentList = new List<Student>();
    public List<Student> MyMatchingStudentList => _myMatchingStudentList; // ��⿡ �����ϴ� �츮 �л� ����Ʈ ������Ƽ(�ܺ� ȣ���)
    public List<Student> RivalMatchingStudentList => _rivalMatchingStudentList; // ��⿡ �����ϴ� ����� �л� ����Ʈ ������Ƽ(�ܺ� ȣ���)

    public void Init()
    {
        // ���� ���� �����Ͱ� �����Ѵٸ�?
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
        // �����Ͱ� ����� �ε�Ǿ��ٸ� �Ʒ� �۾��� �� �ʿ� ����.
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
        // �����Ͱ� ���� ��
        // else
        // {
        //     // CharacterList���� ���� ��ġ�� ī�� �迭�� ������
        //     PlayerCard[] placedCards = _characterList.PositionCards;

        //     for (int i = 0; i < _fightingList.Length; i++)
        //     {
        //         Student targetStudent = null;

        //         // 1. �ش� ���Կ� ������ ��ġ�� ī�尡 �ִ��� Ȯ��
        //         if (placedCards[i] != null && placedCards[i].Player != null)
        //         {
        //             targetStudent = placedCards[i].Player;
        //         }
        //         // 2. ī�尡 ���ٸ� �뺴 ����
        //         else
        //         {
        //             // �ε��� i�� ���������� ��ȯ (0:PG, 1:SG, 2:SF, 3:PF, 4:C ��� ����)
        //             Position targetPos = (Position)i + 1;
        //             targetStudent = _mercenaryMaker.MakeMercenary(targetPos);
        //             targetStudent.OnStatChanged();
        //         }

        //         // 3. CharacterPowerBox�� ���� ���� (�뺴 ����)
        //         if (targetStudent != null)
        //         {
        //             _rivalList[i].Init(targetStudent);
        //             _myTotalFightingPower += (_rivalList[i].Attack + _rivalList[i].Defense);

        //             _myMatchingStudentList.Add(targetStudent);

        //             Debug.Log($"[�Ʊ� ����] {targetStudent.Name}({targetStudent.Position}) | 2��:{targetStudent.GetCurrentStat(potential.Stat2pt)}, 3��:{targetStudent.GetCurrentStat(potential.Stat3pt)}, ����:{targetStudent.GetCurrentStat(potential.StatBlock)}, ��ƿ:{targetStudent.GetCurrentStat(potential.StatSteal)}, ����:{targetStudent.GetCurrentStat(potential.StatRebound)}");
        //         }
        //     }
        // }


        //���⼭ _rivalList �迭 �� ��� CharacterBox�� ����� �л�(Student Ŭ����)���� Init���ֽø� �˴ϴ�.
        //�ùķ����Ϳ� ���������� Init �� _rivalMatchingStudentList�� Add�� ���ֽø� �˴ϴ�.

        //�ùķ����Ϳ��� ��⿡ ������ �����ϴ� �л� ������ �������� ���ؼ��� MyMatchingStudentList �� RivalMatchingStudentList�� ���� �����Ͻø� �˴ϴ�.

        _rivalTotalFightingPower = 0;

        // �� �� ���� (�׽�Ʈ�� ID �Է�)
        MatchTeam generatedAwayTeam = EnemyTeamFactory.Instance.CreateEnemyTeam("Team_DOM_03", "LV_Swiss_03");
        
        // ���� ������ ���� ������ �� ������ �ȵƴٸ� ����
        if (generatedAwayTeam == null)
        {
            Debug.LogError("�� �� ���� ����!");
            return;
        }
        Debug.Log($"[���丮 Ȯ��] ���� 1�� ���� 2���� ����: {generatedAwayTeam.Roster[0].GetStat(MatchStatType.TwoPoint)}");

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

                rivalStudent.SetName(mp.PlayerName);
                rivalStudent.SetPosition(mp.MainPosition);

                // ���丮���� ���� ���� �״�� �̽�
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
                rivalStudent.OnStatChanged(); // ���ݷ�, ���� ����

                // UI�� ���� ����
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

    // ���̹��� ��ġ�� �� ���� ��ϵ��� �����ϴ� �޼���
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
        // // �׽�Ʈ ��
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
