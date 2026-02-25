using UnityEngine;
using System.Collections.Generic;

public class Infra : MonoBehaviour
{
    [SerializeField] private int _currentLevel = 0;               // 현재 레벨
    public int CurrentLevel => _currentLevel;
    [SerializeField] private infraEffectType _infraEffectType;
    [SerializeField] private int _maxLevel = -1;                  // 최대 레벨
    private List<int> needCostByLevel;

    private void Awake()
    {
        needCostByLevel = new List<int>();
    }

    // 최대 레벨 주입
    public void SetMaxLevel(InfraManager infraMgr)
    {
        if (_maxLevel == -1) _maxLevel = infraMgr.MaxLevelData[_infraEffectType];
    }              

    public bool CanUpgrade()
    {
        if (_maxLevel == -1) return false;
        return _currentLevel < _maxLevel;
    }

    public void Upgrade(int money)
    {
        var infraMgr = InfraManager.Instance;
        if (infraMgr == null) return;

        if (!CanUpgrade())
        {
            Debug.Log("이미 최대 레벨에 도달했습니다!");
            return;
        }

        SetMaxLevel(infraMgr);
        OnUpgrade(infraMgr, money);
        _currentLevel++;
    }

    public void OnUpgrade(InfraManager infraMgr, int money)
    {
        if (needCostByLevel == null)
            needCostByLevel = infraMgr.GetCostListByEffectType(_infraEffectType);

        if (GameManager.Instance == null) return;
        var gameMgr = GameManager.Instance;

        if (gameMgr.SaveData.money < money)
        {
            Debug.Log("업그레이드를 하기 위한 비용이 부족합니다!");
            return;
        }

        gameMgr.SetMoney(gameMgr.SaveData.money - money);
    }
}
