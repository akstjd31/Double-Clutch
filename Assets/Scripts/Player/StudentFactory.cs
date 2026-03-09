using System.Collections.Generic;
using UnityEngine;

public class StudentFactory : MonoBehaviour
{
    [Header("<size=18>Player Data SO 모음</size>")]
    [Header("Player_SpeciesDataReader(종족 데이터")]
    [SerializeField] Player_SpeciesDataReader _speciesDataReader; 
    [Header("Player_PersonalityDataReader(성격 데이터)")]
    [SerializeField] Player_PersonalityDataReader _personalityDataReader; 
    [Header("Player_TraitDataReader(특성 데이터)")]
    [SerializeField] Player_TraitDataReader _traitDataReader; 
    [Header("Player_PassiveDataReader(패시브 스킬 데이터)")]
    [SerializeField] Player_PassiveDataReader _passiveDataReader; 
    [Header("Player_NameDataReader(이름 데이터)")]
    [SerializeField] Player_NameDataReader _nameDataReader; 
    [Header("Player_VisualDataReader(비주얼 데이터)")]
    [SerializeField] Player_VisualDataReader _visualDataReader; 
    [Header("Player_StartingStateDataReader(시작 능력치 데이터)")]
    [SerializeField] Player_StartingStateDataReader _startingStateDataReader; 
    [Header("Player_MaxPotentialDataReader(최대 능력치 데이터)")]
    [SerializeField] Player_MaxPotentialDataReader _maxPotentialDataReader; 
    [Header("Player_GrowthRateDataReader(성장률 데이터)")]
    [SerializeField] Player_GrowthRateDataReader _growthRateDataReader;
    [Header("Player_PositionData(포지션 추천 가중치 데이터)")]
    [SerializeField] Player_PositionDataReader _player_PositionDataReader;
    [Header("Player_PassiveGradeData(패시브 등장 확률 데이터)")]
    [SerializeField] Player_PassiveGradeDataReader _passiveGradeDataReader;

    const float FIRST_GRADE_RATE = 0.6f;
    const float SECOND_GRADE_RATE = 0.2f;
    const float THIRD_GRADE_RATE = 0.2f;

    List<Player_StartingStateData> _startingStates = new List<Player_StartingStateData>();
    Player_MaxPotentialData _maxPotential;
    
    List<string> _firstNames = new List<string>();
    List<string> _middleNames = new List<string>();
    List<string> _lastNames = new List<string>();    
    
    Dictionary<string, List<Player_VisualData>> _visualDataDict = new Dictionary<string, List<Player_VisualData>>();
    Dictionary<int, List<Player_PassiveData>> _gradePool = new Dictionary<int, List<Player_PassiveData>>();



    public Student MakeRandomStudent()
    {
        Student newStudent = new Student();
        
        newStudent.SetSpecie(GetRandomSpecie());
        newStudent.SetVisual(GetRandomVisual(newStudent.SpecieId));
        newStudent.SetGrade(GetrRandomGrade());
        newStudent.SetPersonality(GetRandomPersonality());
        newStudent.SetTrait(GetRandomTrait()); 
        newStudent.SetName(GetRandomName());
        SetPassives(newStudent, GetRandomPassive(newStudent));  
        newStudent.SetStat(GetRandomStats(newStudent.Grade));

        InitStudent(newStudent);        

        return newStudent;
    }

    public void InitStudent(Student target) 
    {
        target.Init(_speciesDataReader, _personalityDataReader, _passiveDataReader, _traitDataReader, _player_PositionDataReader);
        Position bestPosition = DecideBestPosition(target);
        target.SetPosition(bestPosition);
    }

    public void InitDatas() 
    {
        for (int i = 0; i < _nameDataReader.DataList.Count; i++)
        {
            
            Player_NameData nameData = _nameDataReader.DataList[i];
            switch (nameData.namePart)
            {
                case namePart.FirstName:
                    _firstNames.Add(StringManager.Instance.GetString(nameData.nameKey));
                    break;
                case namePart.MiddleName:
                    _middleNames.Add(StringManager.Instance.GetString(nameData.nameKey));
                    break;
                case namePart.LastName:
                    _lastNames.Add(StringManager.Instance.GetString(nameData.nameKey));
                    break;
            }
        }

        foreach (var visualData in _visualDataReader.DataList)
        {
            string specieId = visualData.speciesId; 

            
            if (!_visualDataDict.ContainsKey(specieId))
            {
                _visualDataDict[specieId] = new List<Player_VisualData>();
            }

            
            _visualDataDict[specieId].Add(visualData);
        }

        _maxPotential = _maxPotentialDataReader.DataList[0];


    }

    private Position DecideBestPosition(Student student)
    {
        Position bestPos = Position.C;
        float maxScore = -1f;

        foreach (var data in _player_PositionDataReader.DataList)
        {            
            int m1 = student.GetCurrentStat(data.stat1);
            int m2 = student.GetCurrentStat(data.stat2);
            int sub = student.GetCurrentStat(data.stat3);

            float currentScore = (m1 * data.recommendation1) + (m2 * data.recommendation2) + (sub * data.recommendation3);

            if (currentScore > maxScore)
            {
                maxScore = currentScore;
                bestPos = data.recommendId;
            }
        }

        return bestPos;
    }


    private string GetRandomName() 
    {
        string first = _firstNames[Random.Range(0, _firstNames.Count)];
        string middle = _middleNames[Random.Range(0, _middleNames.Count)];
        string last = _lastNames[Random.Range(0, _lastNames.Count)];

        return first + middle + last;
    }

    private Player_SpeciesData GetRandomSpecie()
    {        
        return _speciesDataReader.DataList[Random.Range(0, _speciesDataReader.DataList.Count)];
    }

    private Player_VisualData GetRandomVisual(string specieId)
    {
        if (_visualDataDict.TryGetValue(specieId, out var value))
        {
            return _visualDataDict[specieId][Random.Range(0, _visualDataDict[specieId].Count)];
        }
        else
        {
            return new Player_VisualData();
        }
    }

    private Player_PersonalityData GetRandomPersonality()
    {
        return _personalityDataReader.DataList[Random.Range(0, _personalityDataReader.DataList.Count)];
    }    
    private Player_TraitData GetRandomTrait() 
    {
        return _traitDataReader.DataList[Random.Range(0, _traitDataReader.DataList.Count)];
    }
    private int GetrRandomGrade() 
    {
        float random = Random.value; 

        if (random < FIRST_GRADE_RATE)
        {
            return 1;
        }
        else if (random < FIRST_GRADE_RATE + SECOND_GRADE_RATE)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    private int GetRandomGrowthRate(int grade)
    {
        int min = _growthRateDataReader.DataList[grade - 1].minGrowthRate;
        int max = _growthRateDataReader.DataList[grade - 1].maxGrowthRate;
        
        return Random.Range(min, max);
    }

    private void SetPassives(Student student, List<Player_PassiveData> passives)
    {
        for(int i = 0; i < passives.Count; i++)
        {
            student.SetPassive(passives[i]);
        }
    }

    private List<Player_PassiveData> GetRandomPassive(Student student)
    {
        List<Player_PassiveData> selectedPassives = new List<Player_PassiveData>();
        Dictionary<int, List<Player_PassiveData>> localGradePool = new Dictionary<int, List<Player_PassiveData>>();
        List<Player_PassiveData> availableAll = student.GetAvailablePassives(_passiveDataReader.DataList);

        // 등급별로 분류 (이 부분은 유지)
        foreach (var p in availableAll)
        {
            if (!localGradePool.ContainsKey(p.grade))
                localGradePool[p.grade] = new List<Player_PassiveData>();
            localGradePool[p.grade].Add(p);
        }

        int targetCount = student.Grade; // 학년만큼 뽑기

        for (int i = 0; i < targetCount; i++)
        {            
            int selectedGrade = GetWeightedRandomPassiveGrade(_passiveGradeDataReader.DataList);

            if (!localGradePool.ContainsKey(selectedGrade) || localGradePool[selectedGrade].Count == 0)
            {
                if (availableAll.Count == 0) break;
                var fallback = availableAll[Random.Range(0, availableAll.Count)];
                selectedPassives.Add(fallback);
                RemoveFromLocalPools(fallback, localGradePool, availableAll);
            }
            else
            {
                int randomIndex = Random.Range(0, localGradePool[selectedGrade].Count);
                var finalData = localGradePool[selectedGrade][randomIndex];
                selectedPassives.Add(finalData);
                RemoveFromLocalPools(finalData, localGradePool, availableAll);
            }
        }
        return selectedPassives;
    }
    private void RemoveFromLocalPools(Player_PassiveData data, Dictionary<int, List<Player_PassiveData>> pool, List<Player_PassiveData> all)
    {
        all.Remove(data);
        if (pool.ContainsKey(data.grade)) pool[data.grade].Remove(data);
    }

    private int GetWeightedRandomPassiveGrade(List<Player_PassiveGradeData> gradeDataList)
    {
        float randomPoint = Random.value;
        float cumulative = 0;

        // 리스트를 돌면서 각 등급의 고유한 spawnRate를 누적 합산
        for (int i = 0; i < gradeDataList.Count; i++)
        {
            cumulative += gradeDataList[i].spawnRate;
            if (randomPoint <= cumulative)
            {
                return gradeDataList[i].gradeId; // 해당 데이터의 gradeId(1~4) 반환
            }
        }
        return 1;
    }

    private List<Stat> GetRandomStats(int grade)
    {
        List<Stat> newStat = new List<Stat>();
        Player_StartingStateData stateSetting = _startingStateDataReader.DataList[grade - 1];
        
        foreach (potential type in System.Enum.GetValues(typeof(potential)))
        {            
            if (type == potential.None)
            {
                continue;
            }

            int currentValue = Random.Range(stateSetting.startMin, stateSetting.startMax + 1); 
            int limitValue = Random.Range(_maxPotential.minPotentialValue, _maxPotential.maxPotentialValue + 1); 
            int growthRate = GetRandomGrowthRate(grade);
            int safetyNet = 0;
            while (limitValue <= currentValue && safetyNet < 100) 
            {
                limitValue = Random.Range(_maxPotential.minPotentialValue, _maxPotential.maxPotentialValue + 1);
                safetyNet++;
            }
            if (limitValue <= currentValue) 
            {
                limitValue = currentValue + Random.Range(5, 15); 
            }
            Stat stat = new Stat(type, currentValue, limitValue, growthRate);
            newStat.Add(stat);
        }
        return newStat;
    }
}
