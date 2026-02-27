using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StudentState
{
    None, OverWorked, Injured
}

[Serializable]
public class Student
{
    //저장되는 데이터(-1은 미할당을 의미)
    [SerializeField] int _studentId = -1; //학생 식별용 고유 id(학생 영입 확정 후 부여)
    [SerializeField] string _name; //이름
    [SerializeField] string _specieId = string.Empty; // 종족
    [SerializeField] string _visualId = string.Empty; // 비주얼 Id
    [SerializeField] string _personalityId = string.Empty; // 성격 Id
    [SerializeField] List<string> _passiveIdList = new List<string>(); //패시브 Id
    [SerializeField] string _traitId = string.Empty; //특성 Id
    [SerializeField] int _grade = -1; //학년
    [SerializeField] List<Stat> _stats = new List<Stat>(); //스탯(잠재력)    

    [SerializeField] Position _position;
    [SerializeField] StudentState _state;
    [SerializeField] int _condition = 100;
    [SerializeField] int _cureCount = 0;


    //게임 실행 후 불러오는 데이터
    Player_SpeciesData _specieData; //종족
    Player_VisualData _visualData; //비주얼
    Player_PersonalityData _personalityData; //성격
    List<Player_PassiveData> _passiveDataList = new List<Player_PassiveData>(); //패시브 스킬
    Player_TraitData _traitData; //특성
    Dictionary<potential, Stat> _statDict = new Dictionary<potential, Stat>(); //스탯(잠재력)목록
    int _attack;
    int _defense;
    int _attackChange;
    int _defenseChange;
    Position _matchPosition;
    ITraining _currentTraining;
    



    //외부 호출용 프로퍼티(조회용)
    public int StudentId => _studentId;
    public string Name => _name;
    public string SpecieId => _specieId;
    public Player_SpeciesData SpecieData => _specieData;
    public string VisualId => _visualId;
    public Player_VisualData VisualData => _visualData;
    public string PersonalityId => _personalityId;
    public Player_PersonalityData PersonalityData => _personalityData;
    public List<string> PassiveId => _passiveIdList;
    public List<Player_PassiveData> Passive => _passiveDataList;
    public string TraitId => _traitId;
    public Player_TraitData TraitData => _traitData;
    public int Grade => _grade;
    public int Attack => _attack;
    public int Defense => _defense;
    public int AttackChange => _attackChange;
    public int DefenseChange => _defenseChange;
    public Position Position => _position;
    public Position MatchPosition => _matchPosition;
    public StudentState State => _state;
    public int Condition => _condition;
    public int CureCount => _cureCount;
    public ITraining CurrentTraining => _currentTraining;
    public void ResetTrainingSchedule()
    {
        _currentTraining = null;
    }
    public void SetCurrentTraining(ITraining training)
    {
        _currentTraining = training;
    }
    public int GetCurrentStat(potential type) //현재 스탯 수치 반환(바로가기) 매서드
    {
        if (type == potential.None || !_statDict.ContainsKey(type))
        {
            return 0;
        }
        return _statDict[type].Current;
    }

    public Stat GetStat(potential type) //원하는 타입의 스탯 반환
    {
        if (type == potential.None || !_statDict.ContainsKey(type))
        {
            Debug.LogWarning("potential이 None으로 설정된 데이터가 있습니다. 데이터를 확인해주세요.");
            return null;
        }
        return _statDict[type];
    }

    public float GetPositionScore(Position position)
    {
        PositionOfferData offer = new PositionOfferData(position);
        int m1 = GetCurrentStat(offer.MainPotential1);
        int m2 = GetCurrentStat(offer.MainPotential2);
        int sub = GetCurrentStat(offer.SubPotential);

        return (m1 * 3f) + (m2 * 3f) + (sub * 1f);
    }



    #region 데이터 할당용 함수

    public void SetStudentId(int id)
    {
        _studentId = id;
    }

    public void SetName(string name)
    {
        _name = name;
    }
    public void SetSpecieId(string specieId)
    {
        _specieId = specieId;
    }
    public void SetSpecie(Player_SpeciesData data)
    {
        _specieData = data;
        if (string.IsNullOrEmpty(_specieId))
        {
            _specieId = data.speciesId;
        }
    }

    public void SetVisual(Player_VisualData data)
    {
        _visualData = data;
    }

    public void SetPersonalityId(string personalityId)
    {
        _personalityId = personalityId;
    }
    public void SetPersonality(Player_PersonalityData data)
    {
        _personalityData = data;
        if (string.IsNullOrEmpty(_personalityId))
        {
            _personalityId = data.personalityId;
        }
    }

    public void SetPassiveId(string passiveId)
    {
        if (!_passiveIdList.Contains(passiveId))
        {
            _passiveIdList.Add(passiveId);
        }
    }

    public void SetPassive(Player_PassiveData data)
    {
        if (!_passiveDataList.Contains(data))
        {
            _passiveDataList.Add(data);
        }
        if (!_passiveIdList.Contains(data.skillId))
        {
            _passiveIdList.Add(data.skillId);
        }
    }
    public bool HasPassive(string skillId)
    {
        return _passiveIdList.Contains(skillId);
    }

    public List<Player_PassiveData> GetAvailablePassives(List<Player_PassiveData> totalPool)
    {
        List<Player_PassiveData> available = new List<Player_PassiveData>();
        foreach (var data in totalPool)
        {
            if (!HasPassive(data.skillId))
            {
                available.Add(data);
            }
        }
        return available;
    }

    public void SetTraitsId(string traitId)
    {
        _traitId = traitId;        
    }
    public void SetTrait(Player_TraitData data)
    {
        _traitData = data;
        if (string.IsNullOrEmpty(_traitId))
        {
            _traitId = data.traitId;
        }
    }

    public void SetGrade(int grade)
    {
        _grade = grade;
    }

    public void SetStat(List<Stat> stat)
    {        
        _stats.Clear();
        _stats.AddRange(stat);
    }

    public void SetPosition(Position position)
    {
        _position = position;        
    }

    public void ChangeCondition(int amount)
    {
        _condition = Mathf.Clamp(_condition += amount, 0, 100);
    }

    public void ChangeState(StudentState newState)
    {
        _state = newState;
        if (newState == StudentState.None)
        {
            _cureCount = 0;
        }
    }

    public void SetCureCount(int count)
    {
        _cureCount = count;
    }

    public void SetMatchPosition(Position position)
    {
        _matchPosition = position;
    }

    #endregion



    public void Init(Player_SpeciesDataReader specieDb, Player_PersonalityDataReader personalityDb, Player_PassiveDataReader passiveDb, Player_TraitDataReader traitDb) //Id 기반으로 데이터 연결하기
    {
        InitStat();
        InitSpecies(specieDb);
        InitPersonality(personalityDb);
        InitPassive(passiveDb);
        InitTrait(traitDb);
    }

    private void InitStat()
    {
        _statDict.Clear();
        foreach (var stat in _stats) //스탯 리스트를 딕셔너리에 할당(조회 편의성)
        {
            _statDict[stat.Type] = stat;
        }
        OnStatChanged(); //공격력 & 방어력 계산
    }

    public void PrepareStatChange()
    {
        _attackChange = _attack;  // 현재 공격력을 임시 저장
        _defenseChange = _defense; // 현재 수비력을 임시 저장
    }

    public void OnStatChanged() //스탯 기반 공격력 & 방어력 계산
    {
        int newAttack = 0;
        int newDefense = 0;
        foreach (var stat in _stats)
        {
            switch (stat.Type)
            {
                case potential.None:
                    break;
                case potential.Stat2pt:
                case potential.Stat3pt:
                case potential.StatPass:
                    newAttack += stat.Current;
                    break;
                case potential.StatBlock:
                case potential.StatSteal:
                case potential.StatRebound:
                    newDefense += stat.Current;
                    break;
            }
        }
        _attackChange = newAttack - _attackChange;
        _defenseChange = newDefense - _defenseChange;        
    }

    private void InitSpecies(Player_SpeciesDataReader db)
    {
        _specieData = db.DataList.Find(data => data.speciesId == SpecieId);
    }
    private void InitPersonality(Player_PersonalityDataReader db)
    {
        _personalityData = db.DataList.Find(data => data.personalityId == PersonalityId);
    }

    private void InitPassive(Player_PassiveDataReader db)
    {
        _passiveDataList.Clear();
        for (int i = 0; i < _passiveIdList.Count; i++)
        {
            Player_PassiveData data = db.DataList.Find(data => data.skillId == _passiveIdList[i]);
            _passiveDataList.Add(data);
        }        
    }

    private void InitTrait(Player_TraitDataReader db)
    {
        _traitData = db.DataList.Find(data => data.traitId == TraitId);
    }
}
