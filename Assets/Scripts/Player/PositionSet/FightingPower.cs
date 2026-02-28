using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// FightingPower에 부착할 스크립트
/// 배치된 선수 정보를 PositionSet에서 받아와서 공격력 합산 및 UI에 표기
/// </summary>
public class FightingPower : MonoBehaviour
{
    /// <summary>    
    /// CharacterBox의 Init 메서드 인자값으로 학생을 넣으면 해당 CharacterBox UI는 자동으로 표시됩니다.
    /// 다시 말해, 이 FightingPower의 Init 메서드 안에서, _rivalList에 들어있는 CharacterBox를 각각 모두 Init해주시면 됩니다.
    /// _rivalList는 하이어라키에서 이미 할당 채워놓았습니다. 
    /// </summary>
    [SerializeField] CharacterList _characterList; // 배치된 학생 정보 받아올 배치 화면 UI
    [SerializeField] MercenaryMaker _mercenaryMaker; // 용병 생성기

    [SerializeField] TextMeshProUGUI _mySchoolName; //UI(우리 편 학교 이름)
    [SerializeField] TextMeshProUGUI _myFightingPowerText; //UI(우리 편 전투력 (공격력 + 수비력 총합산))

    [SerializeField] TextMeshProUGUI _rivalSchoolName;//UI(상대 편 학교 이름)
    [SerializeField] TextMeshProUGUI _rivalFightingPowerText;//UI(상대 편 학교 이름)

    
    [SerializeField] CharacterPowerBox[] _fightingList = new CharacterPowerBox[5]; //우리편 전력 비교창 UI(각각 학생 한 명)
    [SerializeField] CharacterPowerBox[] _rivalList  = new CharacterPowerBox[5]; //상대편 전력 비교창 UI(각각 학생 한 명)

    int _myTotalFightingPower = 0; //우리편 전력 계산용
    int _rivalTotalFightingPower = 0; //상대편 전력 계산용

    private List<Student> _myMatchingStudentList = new List<Student>();
    private List<Student> _rivalMatchingStudentList = new List<Student>();
    public List<Student> MyMatchingStudentList => _myMatchingStudentList; // 경기에 참여하는 우리 학생 리스트 프로퍼티(외부 호출용)
    public List<Student> RivalMatchingStudentList => _rivalMatchingStudentList; // 경기에 참여하는 상대편 학생 리스트 프로퍼티(외부 호출용)


    

    public void Init()
    {
        _myTotalFightingPower = 0;

        // CharacterList에서 현재 배치된 카드 배열을 가져옴
        PlayerCard[] placedCards = _characterList.PositionCards;

        for (int i = 0; i < _fightingList.Length; i++)
        {
            Student targetStudent = null;

            // 1. 해당 슬롯에 유저가 배치한 카드가 있는지 확인
            if (placedCards[i] != null && placedCards[i].Player != null)
            {
                targetStudent = placedCards[i].Player;
            }
            // 2. 카드가 없다면 용병 생성
            else
            {
                // 인덱스 i를 포지션으로 변환 (0:PG, 1:SG, 2:SF, 3:PF, 4:C 라고 가정)
                Position targetPos = (Position)i + 1;
                targetStudent = _mercenaryMaker.MakeMercenary(targetPos);
                targetStudent.OnStatChanged();                
            }

            // 3. CharacterPowerBox에 정보 주입 (용병 포함)
            if (targetStudent != null)
            {
                _fightingList[i].Init(targetStudent);
                _myTotalFightingPower += (_fightingList[i].Attack + _fightingList[i].Defense);

                _myMatchingStudentList.Add(targetStudent);
            }
        }

        //여기서 _rivalList 배열 내 모든 CharacterBox를 상대편 학생(Student 클래스)으로 Init해주시면 됩니다.
        //시뮬레이터에 가져가려면 Init 후 _rivalMatchingStudentList에 Add도 해주시면 됩니다.

        //시뮬레이터에서 경기에 실제로 참여하는 학생 정보를 가져가기 위해서는 MyMatchingStudentList 와 RivalMatchingStudentList를 각각 참조하시면 됩니다.

        _rivalMatchingStudentList.Clear();
        _rivalTotalFightingPower = 0;

        // 적 팀 생성 (테스트용 ID 입력)
        MatchTeam generatedAwayTeam = EnemyTeamFactory.Instance.CreateEnemyTeam("Team_DOM_03", "LV_Swiss_03");

        // 만약 데이터 오류 등으로 팀 생성이 안됐다면 중지
        if (generatedAwayTeam == null)
        {
            Debug.LogError("적 팀 생성 실패!");
            return;
        }

        // 2. 생성된 MatchPlayer(시뮬레이터용) 데이터를 Student(UI 표시용)로 포장
        for (int i = 0; i < _rivalList.Length; i++)
        {
            MatchPlayer mp = generatedAwayTeam.Roster[i];
            Student rivalStudent = new Student();

            rivalStudent.SetName(mp.PlayerName);
            rivalStudent.SetPosition(mp.MainPosition);

            // 팩토리에서 뽑힌 스탯 그대로 이식
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
            rivalStudent.OnStatChanged(); // 공격력, 방어력 갱신

            // UI에 정보 주입
            _rivalList[i].Init(rivalStudent);
            _rivalMatchingStudentList.Add(rivalStudent);

            _rivalTotalFightingPower += (rivalStudent.Attack + rivalStudent.Defense);
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
    public void OnClickStartMatch()
    {
        GameManager.Instance.LoadMatchSceneWithData("Test_Simul", MyMatchingStudentList, RivalMatchingStudentList);
    }
}
