using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 각 시설별 존재하는 스크립트. Infra 클래스의 정보 담당, UI 갱신 (옵저버) 등
/// </summary>
public class InfraController : MonoBehaviour
{
    private InfraUI _infraUi;
    private Button button;
    [SerializeField] private infraEffectType _infraEffectType;
    [SerializeField] private List<int> _needCost;

    [SerializeField] private Infra infra;

    public event Action<int> Upgraded;
    private bool initComplete = false;                     // 초기 세팅이 완료되었는지 여부 확인
    private Coroutine _warningCoroutine;

    private void Awake()
    {
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
        if (InfraManager.Instance == null) return;
        if (infra == null) return;

        InfraManager.Instance.SetInfra(infra);
        InfraManager.Instance.SaveData();
    }

    // 본인의 그룹 ID랑 최대 레벨 세팅
    private void Init()
    {
        if (initComplete) return;

        var infraMgr = InfraManager.Instance;
        if (infraMgr == null) return;

        var data = infraMgr.GetDataByEffectType(_infraEffectType);
        if (data == null) return;

        var saveData = infraMgr.GetInfraDataByGroupId(data.Value.group);

        // 저장된 데이터 유무에 따른 불러오는 방식
        if (saveData == null)
        {
            infra = new Infra
            (
                name: data.Value.desc,
                desc: infraMgr.GetInfraDescByEffectType(_infraEffectType),
                nameKey: data.Value.infraNameKey,
                descKey: data.Value.infraDescKey,
                maxLevel: infraMgr.GetMaxLevelByEffectType(_infraEffectType),
                groupId: data.Value.group
            );

            infra.SetInfraEffectValueList(infraMgr.GetValueListByEffectType(_infraEffectType));
        }
        else
        {
            infra = saveData;
        }

        _needCost = infraMgr.GetCostListByEffectType(_infraEffectType);

        Debug.Log($"[{infra.name}] 기초 세팅 완료!");
        infraMgr.SetInfra(infra);
        initComplete = true;
    }

    private void OnClickInfraButton()
    {
        if (_infraUi == null) return;
        if (infra == null) return;

        // UI 초기 세팅
        _infraUi.SetInfraUpgradePanelUI(this, infra);
    }

    public void Upgrade()
    {
        var gameMgr = GameManager.Instance;
        if (gameMgr == null) return;
        if (infra == null) return;

        // 최대 레벨 체크
        if (infra.currentLevel >= infra.maxLevel)
        {
            Debug.Log("이미 최대 레벨입니다.");
            return;
        }

        // 비용 체크
        int cost = GetCostByNextLevel();
        if (gameMgr.SaveData.money < cost)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        // 돈 차감
        gameMgr.SetMoney(gameMgr.SaveData.money - cost);

        // 레벨 업
        infra.currentLevel++;
        Upgraded?.Invoke(infra.currentLevel);

        if (InfraManager.Instance != null)
            InfraManager.Instance.UpdateInfraLevel(infra);

        if (infra.infraEffectType == infraEffectType.AddTactic && StudentManager.Instance != null)
            StudentManager.Instance.OnInfraUpdated();
    }

    public int GetCurrentInfraEffectValue()
    {
        if (infra.infraEffectValue == null) return -1;
        return infra.infraEffectValue[infra.currentLevel];
    }

    public int GetCostByNextLevel()
    {
        if (_needCost == null || infra == null) return -1;

        int nextLevel = infra.currentLevel + 1;
        if (nextLevel < 0 || nextLevel >= _needCost.Count) return -1;

        return _needCost[nextLevel];
    }

    // 업그레이드 할 정도의 비용이 있는지?
    public bool HasEnoughUpgradeCost()
    {
        if (GameManager.Instance == null) return false;

        var money = GameManager.Instance.SaveData.money;
        return money >= GetCostByNextLevel();
    }
}
