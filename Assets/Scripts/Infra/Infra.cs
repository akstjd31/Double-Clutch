using UnityEngine;

public abstract class Infra : MonoBehaviour
{
    [SerializeField] protected int _currentLevel = 0;   // 현재 레벨
    public int CurrentLevel => _currentLevel;
    public abstract int MaxLevel { get; }               // 최대 레벨
    
    public abstract int GetUpgradeCost();               // 업그레이드에 드는 비용인데, 이건 테이블 나와야 알듯? 일단 내 생각은 매개변수로 해당 데이터 관련된 구조체를 받고, 선언해놓은 변수들로 할당해주는 방식? 게터 -> 세터

    public bool CanUpgrade() => _currentLevel < MaxLevel;

    public void Upgrade(int money)
    {
        if (!CanUpgrade())
        {
            Debug.Log("이미 최대 레벨에 도달했습니다!");
            return;
        }

        int need = GetUpgradeCost();
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
