using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfraUI : MonoBehaviour
{
    [Header("CurrentClickInfra")]

    [Header("UpgradePanel")]
    [SerializeField] private InfraUpgradeUI _infraUpgradeUI;

    [SerializeField] private GameObject _reconfirmPanelObj;

    public void SetInfraUpgradePanelUI(Infra infra)
    {
        _infraUpgradeUI.gameObject.SetActive(true);
        _infraUpgradeUI.Init(infra);
    }
}
