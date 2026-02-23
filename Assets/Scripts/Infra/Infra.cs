using UnityEngine;

public abstract class Infra : MonoBehaviour
{
    [SerializeField] protected int _currentLevel = 0;               // 현재 레벨
    [SerializeField] protected infraEffectType _infraEffectType;
    public int CurrentLevel => _currentLevel;
    [SerializeField] protected int _maxLevel = -1;                  // 최대 레벨
    
    // 최대 레벨 주입
    public void SetMaxLevel()
    {
        var infraMgr = InfraManager.Instance;
        if (infraMgr == null) return;

        if (_maxLevel == -1) _maxLevel = infraMgr.MaxLevelData[_infraEffectType];
    }              

    public bool CanUpgrade()
    {
        if (_maxLevel == -1) return false;
        return _currentLevel < _maxLevel;
    }

    public void Upgrade(int money)
    {
        if (!CanUpgrade())
        {
            Debug.Log("이미 최대 레벨에 도달했습니다!");
            return;
        }

        SetMaxLevel();
        int need = _maxLevel;
        if (money < need)
        {
            Debug.Log("업그레이드를 하기 위한 비용이 부족합니다!");
            return;
        }

        OnUpgrade();
        _currentLevel++;
    }

    protected abstract void OnUpgrade();
}
