using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����: ���̺��κ��� ������ ������ �޾ƿ� ��,
/// ������ �������� ������ ���� �̾Ƴ��� �л��� ����.
/// 
/// �Ҵ��ؾ� �� �л� �ʵ�
/// �̸�, ����, ����, �нú�, Ư��1, Ư��2, �г�, ����
/// </summary>
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

    const float FIRST_GRADE_RATE = 0.6f;
    const float SECOND_GRADE_RATE = 0.2f;
    const float THIRD_GRADE_RATE = 0.2f;

    List<Player_StartingStateData> _startingStates = new List<Player_StartingStateData>(); //���� �ּҰ�
    Player_MaxPotentialData _maxPotential; //���� �ִ�

    //�̸� ������ Ÿ�Ժ�(namePart) �з� ����
    List<string> _firstNames = new List<string>(); //��
    List<string> _middleNames = new List<string>(); //�̸� �߰���
    List<string> _lastNames = new List<string>(); //�̸� ����
    
    //���־� ������ ������(string specie) �з� ����
    Dictionary<string, List<Player_VisualData>> _visualDataDict = new Dictionary<string, List<Player_VisualData>>();

    
    

    public Student MakeRandomStudent()
    {
        Student newStudent = new Student();
        
        newStudent.SetSpecie(GetRandomSpecie()); //���� ����
        newStudent.SetVisual(GetRandomVisual(newStudent.SpecieId));
        newStudent.SetGrade(GetrRandomGrade()); //�г� ����
        newStudent.SetPersonality(GetRandomPersonality()); //���� ����
        newStudent.SetTrait(GetRandomTrait()); //Ư�� ����
        newStudent.SetName(GetRandomName()); //�̸� ����
        SetRandomPassive(newStudent); //�нú� ����        
        newStudent.SetStat(GetRandomStats(newStudent.Grade)); //���� ����

        InitStudent(newStudent);        

        return newStudent;
    }

    public void InitStudent(Student target) //���� ������ & ���� ������ �ҷ����� �� ȣ��
    {
        target.Init(_speciesDataReader, _personalityDataReader, _passiveDataReader, _traitDataReader, _player_PositionDataReader);
        Position bestPosition = DecideBestPosition(target);
        target.SetPosition(bestPosition);
    }

    public void InitDatas() //NameData�� Ÿ�Ժ��� �з�
    {
        for (int i = 0; i < _nameDataReader.DataList.Count; i++)
        {
            //�̸� ������ desc �κ� ���� namekey�� ���� �� ��Ʈ�� ������ ���̺� ���� �ʿ�.
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
            string specieId = visualData.speciesId; // �����Ϳ� ���Ե� ���� ID

            // ��ųʸ��� �ش� ���� Ű�� ������ ����Ʈ�� ���� �������
            if (!_visualDataDict.ContainsKey(specieId))
            {
                _visualDataDict[specieId] = new List<Player_VisualData>();
            }

            // �ش� ���� ����Ʈ�� �߰�
            _visualDataDict[specieId].Add(visualData);
        }

        _maxPotential = _maxPotentialDataReader.DataList[0]; //���� �ִ� ����� ������ ����


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


    private string GetRandomName() //������ �̸� �����ؼ� ��ȯ
    {
        string first = _firstNames[Random.Range(0, _firstNames.Count)];
        string middle = _middleNames[Random.Range(0, _middleNames.Count)];
        string last = _lastNames[Random.Range(0, _lastNames.Count)];

        return first + middle + last;
    }

    private Player_SpeciesData GetRandomSpecie() //������ ���� ��ȯ
    {        
        return _speciesDataReader.DataList[Random.Range(0, _speciesDataReader.DataList.Count)];
    }

    private Player_VisualData GetRandomVisual(string specieId) //������ ���� ������ ���־� ��ȯ
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

    private Player_PersonalityData GetRandomPersonality() //������ ���� ��ȯ
    {
        return _personalityDataReader.DataList[Random.Range(0, _personalityDataReader.DataList.Count)];
    }    
    private Player_TraitData GetRandomTrait() //������ Ư�� ��ȯ
    {
        return _traitDataReader.DataList[Random.Range(0, _traitDataReader.DataList.Count)];
    }
    private int GetrRandomGrade() //���� �г��� ����ġ�� ���� ��ȯ
    {
        float random = Random.value; //0~1 ���� ���� �� ����

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

    private int GetRandomGrowthRate(int grade) //�г⿡ ���� ���� ����� ��ȯ
    {
        int min = _growthRateDataReader.DataList[grade - 1].minGrowthRate;
        int max = _growthRateDataReader.DataList[grade - 1].maxGrowthRate;
        
        return Random.Range(min, max);
    }


    private void SetRandomPassive(Student student) //�������� ���� �нú긦 �ߺ����� �ο�(�ٸ� ���� �Լ��� �޸� �ο����� �Կ� ����)
    {
        List<Player_PassiveData> availablePool = student.GetAvailablePassives(_passiveDataReader.DataList); //�������� �ο� ������ ���� �нú� ��� �޾ƿ���
        int currentPassiveCount = student.PassiveId.Count;
        int targetCount = student.Grade;
        int needCount = targetCount - currentPassiveCount;

        for (int i = 0; i < needCount; i++)
        {
            if (availablePool.Count == 0)
            {
                break;
            }

            int randomIndex = Random.Range(0, availablePool.Count);
            Player_PassiveData data = availablePool[randomIndex];

            student.SetPassive(data);
            availablePool.RemoveAt(randomIndex); // �̹� ���� �� �ߺ� ����
        }
    }

    private List<Stat> GetRandomStats(int grade)
    {
        List<Stat> newStat = new List<Stat>();
        Player_StartingStateData stateSetting = _startingStateDataReader.DataList[grade - 1]; //�ش� �г��� ���� ���� ��������
        
        foreach (potential type in System.Enum.GetValues(typeof(potential)))
        {            
            if (type == potential.None)
            {
                continue;
            }

            int currentValue = Random.Range(stateSetting.startMin, stateSetting.startMax + 1); //���� ���� �Ҵ�
            int limitValue = Random.Range(_maxPotential.minPotentialValue, _maxPotential.maxPotentialValue + 1); //���� �ִ�ġ �Ҵ�
            int growthRate = GetRandomGrowthRate(grade);
            int safetyNet = 0;
            while (limitValue <= currentValue && safetyNet < 100) //���� ���� ������ �ִ� ����ġ ���� ���� ������ �ִ� 100������ �ִ� ������ �ٽ� ����
            {
                limitValue = Random.Range(_maxPotential.minPotentialValue, _maxPotential.maxPotentialValue + 1);
                safetyNet++;
            }
            if (limitValue <= currentValue) //100�� ���ȴµ��� ���� �ȵǾ����� ���� ����
            {
                limitValue = currentValue + Random.Range(5, 15); //���� ����ġ. ���̺����� ū ���� ���� �� Ȯ���ؼ� �ݿ� �ʿ�!
            }
            Stat stat = new Stat(type, currentValue, limitValue, growthRate);
            newStat.Add(stat);
        }
        return newStat;
    }
}
