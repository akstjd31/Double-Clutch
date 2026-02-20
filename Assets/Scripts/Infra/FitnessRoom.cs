using UnityEngine;

public class FitnessRoom : Infra
{
    public override int MaxLevel => 5;
    
    public override int GetUpgradeCost()
    {
        // 데이터 가져오기
        return 0;
    }

    protected override void OnUpgrade() { } // 업그레이드 시 해야할 일 (ex. 차감되는 비용 계산)
}
