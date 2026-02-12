using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 테스트용 데이터 매니저 클래스
/// 테스트용으로 제작된 '테스트데이터 클래스'를 모두 생성하여 보유하고, Get00 매서드로 데이터클래스를 반환해주는 역할(데이터 테이블 작성 및 파싱 절차를 임시로 모방)
/// </summary>
public class TestDataManager_Song : MonoBehaviour
{
    const int MAGIC_NUMBER = 16;
    public static TestDataManager_Song Instance;
    Dictionary<int, TestStartingState> _startingStates = new Dictionary<int, TestStartingState>() { { 1, new TestStartingState(1) }, { 2, new TestStartingState(2)}, { 3, new TestStartingState(3)} };
    Dictionary<int, TestMaxPotentialData> _maxPotentials = new Dictionary<int, TestMaxPotentialData>() { { 1, new TestMaxPotentialData()} };

    Dictionary<int, TestNameData> _nameDatas = new Dictionary<int, TestNameData>();
    Dictionary<int, TestSpecieData> _specieDatas = new Dictionary<int, TestSpecieData>();
    Dictionary<int, TestPassiveData> _passiveDatas = new Dictionary<int, TestPassiveData>();
    Dictionary<int, TestPersonalityData> _personalityDatas = new Dictionary<int, TestPersonalityData>();
    Dictionary<int, TestTraitData> _traitDatas = new Dictionary<int, TestTraitData>();
    

    

    private void Awake()
    {
        Instance = this;
        //Todo : 테스트용 데이터를 모두 생성해서 반복문 i값으로 채우기

        for (int i = 0; i < MAGIC_NUMBER; i++)
        {
            _nameDatas.Add(i, new TestNameData(i));
            _specieDatas.Add(i, new TestSpecieData(i));
            _passiveDatas.Add(i, new TestPassiveData(i));
            _personalityDatas.Add(i, new TestPersonalityData(i));
            _traitDatas.Add(i, new TestTraitData(i));
        }



    }
    public TestStartingState GetStartingState(int grade)
    {
        return _startingStates[grade];
    }

    public TestMaxPotentialData GetMaxPotential(int key)
    {
        return _maxPotentials[key];
    }

    public TestNameData GetName(int id)
    {
        return _nameDatas[id];
    }

    public TestSpecieData GetSpecie(int id)
    {
        return _specieDatas[id];
    }

    public TestPassiveData GetPassive(int id)
    {
        return _passiveDatas[id];
    }

    public TestPersonalityData GetPersonality(int id)
    {
        return _personalityDatas[id];
    }

    public TestTraitData GetTrait(int id)
    {
        return _traitDatas[id];
    }
}
