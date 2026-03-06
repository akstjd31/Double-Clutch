using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;

/// <summary>
/// 인프라 관련 클래스 (해당 스크립트 추가 시 infraEffectType은 시설에 맞게 지정해줘야 함.)
/// </summary>
public class Infra : MonoBehaviour
{
    private InfraUI _infraUi;
    private Button button;
    [SerializeField] private infraEffectType _infraEffectType;
    [SerializeField] private int _currentLevel = 0;               // 현재 레벨
    public int CurrentLevel => _currentLevel;
    private string _name;
    public string Name => _name;
    private string _desc;
    public string Desc => _desc;
    private int _maxLevel = -1;                  // 최대 레벨
    public int MaxLevel => _maxLevel;
    private int _groupId;
    private List<int> _needCost;
    [SerializeField] private List<int> _infraEffectValue;  // 레벨(인덱스)에 따른 인프라 이펙트 벨류 
    public event Action<int> Upgraded;
    private bool initComplete = false;                     // 초기 세팅이 완료되었는지 여부 확인

    private void Awake()
    {
        _needCost = new List<int>();
        _infraUi = this.transform.parent.GetComponent<InfraUI>();
        button = this.GetComponent<Button>();

        if (button == null) return;
        button.onClick.AddListener(OnClickInfraButton);
    }
    
    private void OnEnable() 
    {
        Init();
    }

    private void OnDisable()
    {
    }

    // 본인의 그룹 ID랑 최대 레벨 세팅
    private void Init()
    {
        if (initComplete) return;

        var infraMgr = InfraManager.Instance;
        if (infraMgr == null) return;

        // 각 데이터 갱신
        _maxLevel = infraMgr.GetMaxLevelByEffectType(_infraEffectType);
        _needCost = infraMgr.GetCostListByEffectType(_infraEffectType);
        _infraEffectValue = infraMgr.GetValueListByEffectType(_infraEffectType);
        _desc = infraMgr.GetInfraDescByEffectType(_infraEffectType);

        var data = infraMgr.GetDataByEffectType(_infraEffectType);
        if (data == null) return;

        _groupId = data.Value.group;
        _name = data.Value.desc;

        Debug.Log($"[{_name}] 기초 세팅 완료!");
        initComplete = true;
    }     

    private void OnClickInfraButton()
    {
        if (_infraUi == null) return;

        // UI 초기 세팅
        _infraUi.SetInfraUpgradePanelUI(this);
    }

    public void Upgrade()
    {
        var gameMgr = GameManager.Instance;
        if (gameMgr == null) return;

        gameMgr.SetMoney(gameMgr.SaveData.money - GetCostByNextLevel());

        _currentLevel++;
        Upgraded?.Invoke(_currentLevel);
    }



    public int GetValueByLevel() => _infraEffectValue[_currentLevel];
    public int GetCostByNextLevel() => _currentLevel + 1 < _maxLevel ? _needCost[_currentLevel + 1] : _needCost[_maxLevel];

    public bool IsReachedMaxLevel() => _currentLevel >= _maxLevel;
    public bool HasEnoughUpgradeCost()
    {
        if (GameManager.Instance == null) return false;
        
        var money = GameManager.Instance.SaveData.money;
        return money > GetCostByNextLevel(); 
    }
}
