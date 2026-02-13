using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Student
{
    //저장되는 데이터(-1은 미할당을 의미)
    [SerializeField] string _name; //이름
    [SerializeField] int _specieId = -1; // 종족
    [SerializeField] int _personalityId = -1; // 성격 Id
    [SerializeField] List<int> _passiveId = new List<int>(); //패시브 Id
    [SerializeField] int _traitId = -1; //특성 Id
    
    [SerializeField] int _grade = -1; //학년
    List<Stat> _stats = new List<Stat>(); //스탯(Stat 클래스는 직렬화 되어 있음)

    //게임 실행 후 id 기반으로 불러오는 데이터
    Player_SpeciesData _specieData; //종족
    Player_PersonalityData _personalityData; //성격
    List<Player_PassiveData> _passive = new List<Player_PassiveData>(); //패시브 스킬
    Player_TraitData _traitData; //특성1
    Dictionary<StatType, Stat> _statDict = new Dictionary<StatType, Stat>();


    //외부 호출용 프로퍼티
    public string Name => _name;
    public int SpecieId => _specieId;
    public int PersonalityId => _personalityId;
    public List<int> PassiveId => _passiveId;
    public int TraitId => _traitId;    
    public int Grade => _grade;
    public List<Player_PassiveData> Passive => _passive;

    public int GetCurrentStat(StatType type)
    {
        return _statDict[type].Current;
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
        if (!_passiveId.Contains(passiveId))
        {
            _passiveId.Add(passiveId);
        }        
    }

    public void SetPassive(Player_PassiveData data)
    {
        if (!_passive.Contains(data))
        {
            _passive.Add(data);
        }
        if (!_passiveId.Contains(data.skillId))
        {
            _passiveId.Add(data.skillId);
        }
    }
    public bool HasPassive(int skillId)
    {
        return _passiveId.Contains(skillId);
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

    public void Init() //Id 기반으로 데이터 연결하기
    {
        foreach (var stat in _stats)
        {
            _statDict[stat.Type] = stat;
        }
    }

}
