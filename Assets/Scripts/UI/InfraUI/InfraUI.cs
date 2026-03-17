using UnityEngine;

public class InfraUI : MonoBehaviour
{
    [Header("UpgradePanel")]
    [SerializeField] private InfraUpgradeUI _infraUpgradeUI;

    public void SetInfraUpgradePanelUI(InfraController controller, Infra infra)
    {
        _infraUpgradeUI.gameObject.SetActive(true);
        _infraUpgradeUI.Init(controller, infra);
    }
}

