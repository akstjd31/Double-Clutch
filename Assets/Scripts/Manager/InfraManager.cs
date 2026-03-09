using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 리더기 호출, 데이터 관리
/// </summary>
public class InfraManager : Singleton<InfraManager>
{
    private const int MAX_INFRA_COUNT = 5;
    [SerializeField] private Infra_DataReader _reader;
    [SerializeField] private SerializedDictionary<infraEffectType, string> _infraDescData; // 시설 효과 설명 관련 딕셔너리
    private Infra[] infras;     // 인덱스는 그룹 ID로 받자
    private InfraSaveData _myInfraData;

    protected override void Awake()
    {
        base.Awake();
        _infraDescData = new SerializedDictionary<infraEffectType, string>();
        infras = new Infra[MAX_INFRA_COUNT];
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_reader == null) return;

        var stringMgr = StringManager.Instance;
        if (stringMgr == null) return;

        foreach (var data in _reader.DataList)
        {
            if (_infraDescData.ContainsKey(data.infraEffectType)) continue;
            string desc = stringMgr.GetString(data.infraDescKey);
            _infraDescData[data.infraEffectType] = desc;
        }

        if (SaveLoadManager.Instance == null) return;
        SaveLoadManager.Instance.TryLoad<InfraSaveData>(FilePath.INFRA_PATH, out _myInfraData);
    }

    // 그룹 ID 중 맥스 레벨 반환
    public int GetMaxLevelByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return -1;
        if (infraET.Equals(infraEffectType.None)) return -1;

        int maxLv = 0;
        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            if (maxLv < data.infraLevel) maxLv = data.infraLevel;
        }

        return maxLv;
    }

    // 같은 타입 중에 비용 관련된 리스트 반환
    public List<int> GetCostListByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return null;
        if (infraET.Equals(infraEffectType.None)) return null;

        var costList = new List<int>();
        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            costList.Add(data.infraCost);
        }

        return costList;
    }

    // 같은 타입 중에 벨류값 리스트 반환
    public List<int> GetValueListByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return null;
        if (infraET.Equals(infraEffectType.None)) return null;

        var valueList = new List<int>();
        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            valueList.Add(data.infraEffectValue);
        }

        return valueList;
    }

    // 해당 데이터 반환 (구조체라 Nullable로 바꿔줌)
    public Infra_Data? GetDataByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return null;
        if (infraET.Equals(infraEffectType.None)) return null;

        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            return data;
        }

        return null;
    }

    public string GetInfraDescByEffectType(infraEffectType infraET) => _infraDescData[infraET];

    public Infra GetInfraDataByGroupId(int groupId)
    {
        if (_myInfraData == null) return null;
        if (_myInfraData.infraList == null) return null;
        return _myInfraData.infraList[groupId - 1];
    }

    public void SaveData()
    {
        if (SaveLoadManager.Instance == null) return;

        var saveData = new InfraSaveData();

        for (int i = 0; i < MAX_INFRA_COUNT; i++)
        {
            if (infras[i] == null) continue;
            saveData.infraList.Add(infras[i]);
        }

        SaveLoadManager.Instance.Save<InfraSaveData>(FilePath.INFRA_PATH, saveData);
    }

    public void SetInfra(Infra infra) => infras[infra.groupId - 1] = infra;

    public void UpdateInfraLevel(Infra infra) => infras[infra.groupId - 1].currentLevel = infra.currentLevel;

    public int GetInfraEffectValueByEffectType(infraEffectType type)
    {
        if (infras == null) return -1;
        if (type.Equals(infraEffectType.None)) return -1;

        for (int i = 0; i < MAX_INFRA_COUNT; i++)
        {
            if (infras[i] == null) continue;
            if (!infras[i].infraEffectType.Equals(type)) continue;
            
            return infras[i].infraEffectValue[infras[i].currentLevel];
        }

        return -1;
    }
}
