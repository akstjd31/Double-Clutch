using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfraUI : MonoBehaviour
{
    [Header("UpgradePanel")]
    [SerializeField] private InfraUpgradeUI _infraUpgradeUI;

    public void SetInfraUpgradePanelUI(Infra infra)
    {
        _infraUpgradeUI.gameObject.SetActive(true);
        _infraUpgradeUI.Init(infra);
    }
}
