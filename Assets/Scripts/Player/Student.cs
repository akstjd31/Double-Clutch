using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StudentState
{
    None, Tired, Injured
}

[Serializable]
public class Student
{
    //저장되는 데이터(-1은 미할당을 의미)
    [SerializeField] int _studentId = -1; //학생 식별용 고유 id(학생 영입 확정 후 부여)
    [SerializeField] string _name; //이름
    [SerializeField] int _specieId = -1; // 종족
    [SerializeField] int _visualId = -1; // 비주얼 Id
    [SerializeField] int _personalityId = -1; // 성격 Id
    [SerializeField] List<int> _passiveIdList = new List<int>(); //패시브 Id
    [SerializeField] int _traitId = -1; //특성 Id
    [SerializeField] int _grade = -1; //학년
    [SerializeField] List<Stat> _stats = new List<Stat>(); //스탯(잠재력)
    
    [SerializeField] Position _position;
    [SerializeField] StudentState _state;
    [SerializeField] int _condition = 100;


    //게임 실행 후 불러오는 데이터
    Player_SpeciesData _specieData; //종족
    Player_VisualData _visualData; //비주얼
    Player_PersonalityData _personalityData; //성격
    List<Player_PassiveData> _passiveDataList = new List<Player_PassiveData>(); //패시브 스킬
    Player_TraitData _traitData; //특성
    Dictionary<potential, Stat> _statDict = new Dictionary<potential, Stat>(); //스탯(잠재력)목록
    int _attack;
    int _defense;

    



    //외부 호출용 프로퍼티(조회용)
    public int StudentId => _studentId;
    public string Name => _name;
    public int SpecieId => _specieId;
    public Player_SpeciesData SpecieData => _specieData;
    public int VisualId => _visualId;
    public Player_VisualData VisualData => _visualData;
    public int PersonalityId => _personalityId;
    public Player_PersonalityData PersonalityData => _personalityData;
    public List<int> PassiveId => _passiveIdList;
    public List<Player_PassiveData> Passive => _passiveDataList;
    public int TraitId => _traitId;
    public Player_TraitData TraitData => _traitData;
    public int Grade => _grade;
    public int Attack => _attack;
    public int Defense => _defense;
    public Position Position => _position;
    public StudentState State => _state;
    public int Condition => _condition;

    public int GetCurrentStat(potential type)
    {
        return _statDict[type].Current;
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
    public void SetSpecieId(int specieId)
    {
        _specieId = specieId;
    }
    public void SetSpecie(Player_SpeciesData data)
    {
        _specieData = data;
        if (_specieId == -1)
        {
            _specieId = data.speciesId;
        }
    }

    public void SetVisual(Player_VisualData data)
    {
        _visualData = data;
    }

    public void SetPersonalityId(int personalityId)
    {
        _personalityId = personalityId;
    }
    public void SetPersonality(Player_PersonalityData data)
    {
        _personalityData = data;
        if (_personalityId == -1)
        {
            _personalityId = data.personalityId;
        }
    }    

    public void SetPassiveId(int passiveId)
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
    public bool HasPassive(int skillId)
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

    public void SetTraitsId(int traitId)
    {
        _traitId = traitId;        
    }
    public void SetTrait(Player_TraitData data)
    {
        _traitData = data;
        if (_traitId  == -1)
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
        foreach (var stat in _stats) //스탯 리스트를 딕셔너리에 할당(조회 편의성)
        {
            _statDict[stat.Type] = stat;
        }
        OnStatChanged(); //공격력 & 방어력 계산
    }

    private void OnStatChanged() //스탯 기반 공격력 & 방어력 계산
    {
        int attack = 0;
        int defense = 0;
        foreach (var stat in _stats)
        {
            switch (stat.Type)
            {
                case potential.Stat2pt:
                case potential.Stat3pt:
                case potential.StatPass:
                    attack += stat.Current;
                    break;
                case potential.StatBlock:
                case potential.StatSteal:
                case potential.StatRebound:
                    defense += stat.Current;
                    break;
            }
        }            
        _attack = attack;
        _defense = defense;
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
