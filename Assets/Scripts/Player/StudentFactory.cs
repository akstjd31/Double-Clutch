using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 역할: 테이블로부터 데이터 묶음을 받아온 뒤,
/// 데이터 묶음에서 랜덤한 값을 뽑아내어 학생을 생성.
/// 
/// 할당해야 할 학생 필드
/// 이름, 종족, 성격, 패시브, 특성1, 특성2, 학년, 스탯
/// </summary>
public class StudentFactory : MonoBehaviour
{
    [Header("<size=18>데이터 리더 SO 모음</size>")]
    [Header("Player_SpeciesDataReader(종족 데이터)")]
    [SerializeField] Player_SpeciesDataReader _speciesDataReader; //종족 데이터
    [Header("Player_PersonalityDataReader(성격 데이터)")]
    [SerializeField] Player_PersonalityDataReader _personalityDataReader; //성격 데이터
    [Header("Player_TraitDataReader(특성 데이터)")]
    [SerializeField] Player_TraitDataReader _traitDataReader; //특성 데이터
    [Header("Player_PassiveDataReader(패시브 데이터)")]
    [SerializeField] Player_PassiveDataReader _passiveDataReader; //패시브 데이터
    [Header("Player_NameDataReader(이름 데이터)")]
    [SerializeField] Player_NameDataReader _nameDataReader; //이름 데이터
    [Header("Player_VisualDataReader(외형 데이터)")]
    [SerializeField] Player_VisualDataReader _visualDataReader; //외형 데이터
    [Header("Player_StartingStateDataReader(능력치 시작 범위 데이터)")]
    [SerializeField] Player_StartingStateDataReader _startingStateDataReader; //능력치 시작 범위 데이터
    [Header("Player_MaxPotentialDataReader(능력치 성장 최대값 데이터)")]
    [SerializeField] Player_MaxPotentialDataReader _maxPotentialDataReader; //능력치 성장 최대값 데이터
            
    const float FIRST_GRADE_RATE = 0.6f;
    const float SECOND_GRADE_RATE = 0.2f;
    const float THIRD_GRADE_RATE = 0.2f;

    List<Player_StartingStateData> _startingStates = new List<Player_StartingStateData>(); //스탯 최소값
    Player_MaxPotentialData _maxPotential; //스탯 최댓값

    //이름 데이터 타입별(namePart) 분류 묶음
    List<string> _firstNames = new List<string>(); //성
    List<string> _middleNames = new List<string>(); //이름 중간자
    List<string> _lastNames = new List<string>(); //이름 끝자
    
    //비주얼 데이터 종족별(int specie) 분류 묶음
    Dictionary<int, List<Player_VisualData>> _visualDataDict = new Dictionary<int, List<Player_VisualData>>();

    private void Start()
    {
        InitDatas();
    }

    public Student MakeRandomStudent()
    {
        Student newStudent = new Student();
        
        newStudent.SetSpecie(GetRandomSpecie()); //종족 생성
        newStudent.SetVisual(GetRandomVisual(newStudent.SpecieId));
        newStudent.SetGrade(GetrRandomGrade()); //학년 생성
        newStudent.SetPersonality(GetRandomPersonality()); //성격 생성
        newStudent.SetTrait(GetRandomTrait()); //특성 생성
        newStudent.SetName(GetRandomName()); //이름 생성
        SetRandomPassive(newStudent); //패시브 생성        
        newStudent.SetStat(GetRandomStats(newStudent.Grade)); //스탯 생성

        newStudent.Init(_speciesDataReader, _personalityDataReader, _passiveDataReader, _traitDataReader);

        return newStudent;
    }

    private void InitDatas() //NameData만 타입별로 분류
    {
        for (int i = 0; i < _nameDataReader.DataList.Count; i++)
        {
            //이름 데이터 desc 부분 추후 namekey로 변경 및 스트링 데이터 테이블 참조 필요.
            Player_NameData nameData = _nameDataReader.DataList[i];
            switch (nameData.namePart)
            {
                case namePart.FirstName:
                    _firstNames.Add(nameData.desc);
                    break;
                case namePart.MiddleName:
                    _middleNames.Add(nameData.desc);
                    break;
                case namePart.LastName:
                    _lastNames.Add(nameData.desc);
                    break;
            }
        }

        foreach (var visualData in _visualDataReader.DataList)
        {
            int specieId = visualData.species; // 데이터에 포함된 종족 ID

            // 딕셔너리에 해당 종족 키가 없으면 리스트를 새로 만들어줌
            if (!_visualDataDict.ContainsKey(specieId))
            {
                _visualDataDict[specieId] = new List<Player_VisualData>();
            }

            // 해당 종족 리스트에 추가
            _visualDataDict[specieId].Add(visualData);
        }
    }

    



    private string GetRandomName() //랜덤한 이름 조합해서 반환
    {
        // [방어 코드 추가] 이름 데이터가 하나라도 비어있으면 터지지 않게 임시 이름 반환
        if (_firstNames.Count == 0 || _middleNames.Count == 0 || _lastNames.Count == 0)
        {
            Debug.LogWarning("[데이터 누락] 이름 데이터가 없습니다! 임시 이름을 부여합니다.");
            return "임시선수_" + Random.Range(1000, 9999);
        }
        string first = _firstNames[Random.Range(0, _firstNames.Count)];
        string middle = _middleNames[Random.Range(0, _middleNames.Count)];
        string last = _lastNames[Random.Range(0, _lastNames.Count)];

        return first + middle + last;
    }

    private Player_SpeciesData GetRandomSpecie() //랜덤한 종족 반환
    {
        return _speciesDataReader.DataList[Random.Range(0, _speciesDataReader.DataList.Count)];
    }

    private Player_VisualData GetRandomVisual(int specieId) //종족에 따라 랜덤한 비주얼 반환
    {
        // 1. 딕셔너리에 해당 종족 ID가 있으면 정상 반환
        if (_visualDataDict.ContainsKey(specieId))
        {
            return _visualDataDict[specieId][Random.Range(0, _visualDataDict[specieId].Count)];
        }
        else
        {
            // 2. 데이터가 없으면 에러 로그 출력
            Debug.LogError($"[데이터 누락] 종족 ID '{specieId}'의 비주얼 데이터가 없습니다! 임시 데이터를 사용합니다.");

            // 3. 임시 방편: 딕셔너리에 있는 '다른 종족'의 비주얼이라도 가져와서 반환
            foreach (var key in _visualDataDict.Keys)
            {
                if (_visualDataDict[key].Count > 0)
                    return _visualDataDict[key][0];
            }

            // 4. 진짜 아무것도 없으면 '빈 구조체' 반환 (null 대신 이거 써야 함!)
            return new Player_VisualData();
        }
    }

    private Player_PersonalityData GetRandomPersonality() //랜덤한 성격 반환
    {
        return _personalityDataReader.DataList[Random.Range(0, _personalityDataReader.DataList.Count)];
    }    
    private Player_TraitData GetRandomTrait() //랜덤한 특성 반환
    {
        return _traitDataReader.DataList[Random.Range(0, _traitDataReader.DataList.Count)];
    }
    private int GetrRandomGrade() //랜덤 학년을 가중치에 따라 반환
    {
        float random = Random.value; //0~1 사이 랜덤 값 생성

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

    private void SetRandomPassive(Student student) //선수에게 랜덤 패시브를 중복없이 부여(다른 랜덤 함수와 달리 부여까지 함에 주의)
    {
        List<Player_PassiveData> availablePool = student.GetAvailablePassives(_passiveDataReader.DataList); //선수에게 부여 가능한 남은 패시브 목록 받아오기
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
            availablePool.RemoveAt(randomIndex); // 이번 루프 내 중복 방지
        }
    }

    private List<Stat> GetRandomStats(int grade)
    {
        List<Stat> newStat = new List<Stat>();
        Player_StartingStateData stateSetting = _startingStateDataReader.DataList[grade - 1]; //해당 학년의 스탯 범위 가져오기
        Player_MaxPotentialData maxPotentialData = _maxPotentialDataReader.DataList[0];
        foreach (potential type in System.Enum.GetValues(typeof(potential)))
        {            
            int currentValue = Random.Range(stateSetting.startMin, stateSetting.startMax + 1); //현재 스탯 할당
            int limitValue = Random.Range(maxPotentialData.minPotentialValue, maxPotentialData.maxPotentialValue + 1); //스탯 최대치 할당
            int safetyNet = 0;
            while (limitValue <= currentValue && safetyNet < 100) //만약 시작 스탯이 최대 성장치 보다 높게 뽑히면 최대 100번까지 최대 스탯을 다시 리롤
            {
                limitValue = Random.Range(_maxPotential.minPotentialValue, _maxPotential.maxPotentialValue + 1);
                safetyNet++;
            }
            if (limitValue <= currentValue) //100번 돌렸는데도 보정 안되었으면 강제 보정
            {
                limitValue = currentValue + Random.Range(5, 15); //테이블에서 큰 변동 있을 시 확인해서 반영 필요!
            }
            Stat stat = new Stat(type, currentValue, limitValue);
            newStat.Add(stat);
        }
        return newStat;
    }
}
